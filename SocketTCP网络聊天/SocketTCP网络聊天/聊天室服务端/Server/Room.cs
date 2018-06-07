using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

public enum RoomState
{
    WaitingJoin,     //等待加入
    WaitingBattle,   //等待战斗
    Battle,          //战斗
    End              //战斗结束
}

public class Room
{
    //加入该房间的客户端集合
    private List<Client> clientRoom = new List<Client>();

    private RoomState roomState = RoomState.WaitingJoin;

    private Server server;

    private int roomId;

    private const int MAX_HP = 30;   //玩家的最大血量

    public Room(Server server)
    {
        this.server = server;
    }

    #region 属性方法
    public int RoomId
    {
        get { return roomId; }
    }

    public RoomState RoomState
    {
        get { return roomState; }
    }
    /// <summary>
    /// 该房间当前是否为等待加入状态
    /// </summary>
    public bool IsWaitingJion()
    {
        return roomState == RoomState.WaitingJoin;
    }

    /// <summary>
    /// 获取房主的信息
    /// </summary>
    public string GetRoomOwnerData()
    {
        return clientRoom[0].GetUserData();
    }

    /// <summary>
    /// 该客户端是否为房主
    /// </summary>
    public bool IsRoomOwner(Client client)
    {
        return client == clientRoom[0];
    }
    #endregion


    /// <summary>
    /// 为房间添加用户客户端
    /// </summary>
    public void AddClient(Client client)
    {
        client.Room = this;
        //为此客户端玩家设置最大血量
        client.Player_Hp = MAX_HP;
        clientRoom.Add(client);
        //以房主的用户Id为该房间Id
        if (clientRoom.Count > 0)
        {
            roomId = clientRoom[0].UserId;
        }

        //当房间中存在2个客户端时，房间为等待战斗状态
        if (clientRoom.Count >= 2)
        {
            roomState = RoomState.WaitingBattle;
        }
    }

    /// <summary>
    /// 从房间中移除该客户端
    /// </summary>
    /// <param name="client"></param>
    public void RemoveClient(Client client)
    {
        client.Room = null;
        clientRoom.Remove(client);
        //房间的状态随房间中人数而变化
        if (clientRoom.Count >= 2)
        {
            roomState = RoomState.WaitingBattle;
        }
        else
        {
            roomState = RoomState.WaitingJoin;
        }
    }

    /// <summary>
    /// 获取当前房间中所有客户端的数据
    /// </summary>
    public string GetAllPlayerDataInRoom()
    {
        StringBuilder sb = new StringBuilder();
        if (clientRoom.Count > 0)
        {
            foreach (Client userClient in clientRoom)
            {
                sb.Append(userClient.GetUserData() + "|");
            }
        }
        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }

    /// <summary>
    /// 给在该房间中的所有客户端发送消息
    /// </summary>
    /// <param name="excludeClient">此客户端除外</param>
    public void BroadCastAllClientInRoom(Client excludeClient, ActionCode actionCode, string data)
    {
        foreach (Client client in clientRoom)
        {
            if (client != excludeClient)
            {
                //向客户端发送更新房间的响应
                server.SendResponse(client, actionCode, data);
            }
        }
    }

    /// <summary>
    /// 销毁该房间
    /// </summary>
    public void CloseRoom()
    {
        foreach (Client client in clientRoom)
        {
            client.Room = null;
        }
        server.HandleRemoveRoom(this);
    }

    /// <summary>
    /// 开始倒计时
    /// </summary>
    public void StartTimer()
    {
        //开始计时线程
        new Thread(CountTime).Start();
    }

    public void CountTime()
    {
        //房间状态改为战斗
        roomState = RoomState.Battle;
        for (int i = 3; i > 0; i--)
        {
            Thread.Sleep(1000);
            //向所有客户端广播计时消息
            BroadCastAllClientInRoom(null, ActionCode.ShowTimer, i.ToString());
            //休眠1秒
            Thread.Sleep(1000);
        }

        //所有玩家开始战斗
        BroadCastAllClientInRoom(null, ActionCode.StartPlay, "sp");      
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="excludeClient">发送伤害的客户端除外</param>
    public void PlayerTakeDamage(int damage,Client excludeClient)
    {
        bool isDie = false;
        foreach (Client client in clientRoom)
        {
            if (client != excludeClient)
            {
                //玩家受到伤害且当其生命值为0时
                if(client.TakeDamage(damage))
                {
                    isDie = true;
                }
            }
        }
        if (isDie == true)
        {
            //客户端游戏结束
            SendGameOverToClient();    
        }         
    }

    /// <summary>
    /// 给客户端发送游戏结束请求
    /// </summary>
    public void SendGameOverToClient()
    {
        //当有一名玩家死亡时
        foreach (Client client in clientRoom)
        {
            if (client.IsDie())
            {
                client.UpdateResult(false);
                //此玩家死亡，向该客户端发送游戏结束和失败的消息
                client.Send(ActionCode.GameOver, ((int)(RetrunCode.Fail)).ToString());
            }
            else
            {
                client.UpdateResult(true);
                //此玩家未死亡，向该客户端发送游戏结束和胜利的消息
                client.Send(ActionCode.GameOver, ((int)(RetrunCode.Success)).ToString());
            }
        }
        //销毁该房间
        CloseRoom();
    }

}

