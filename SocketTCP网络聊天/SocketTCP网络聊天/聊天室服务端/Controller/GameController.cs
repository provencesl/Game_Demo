using System;
using System.Collections.Generic;
using System.Text;
using Common;

public class GameController:BaseController
{
    /// <summary>
    /// 开始游戏
    /// </summary>
    public string StartGame(string data, Client client, Server server)
    {
        if (client.IsRoomOwner)
        {//开始游戏的发起者为房主时
            //房主的房间
            Room ownerRoom = client.Room;
            //房间为等待战斗状态
            if (ownerRoom.RoomState == RoomState.WaitingBattle)
            {
                //向房间中其他玩家广播开始游戏
                ownerRoom.BroadCastAllClientInRoom(client, ActionCode.StartGame, ((int)(RetrunCode.Success)).ToString());
                //开始倒计时
                ownerRoom.StartTimer();
                return ((int)(RetrunCode.Success)).ToString();
            }

            //房间中只有房主时无法开始游戏
            return ((int)(RetrunCode.NotFound)).ToString();
        }
        else
        {
            return ((int)(RetrunCode.Fail)).ToString();
        }
    }

    /// <summary>
    /// 玩家移动处理
    /// </summary>
    public string Move(string data, Client client, Server server)
    {
        Room room = client.Room;
        //向房间内其他玩家广播自身的移动动态
        if (room != null)
            room.BroadCastAllClientInRoom(client, ActionCode.Move, data);
        return null;
    }

    /// <summary>
    /// 玩家射击处理
    /// </summary>
    public string Shoot(string data, Client client, Server server)
    {
        Room room = client.Room;
        //向房间内其他玩家广播自身的移动动态
        if (room != null)
            room.BroadCastAllClientInRoom(client, ActionCode.Shoot, data);
        return null;
    }

    /// <summary>
    /// 玩家攻击造成伤害
    /// </summary>
    public string AttackDamage(string data, Client client, Server server)
    {
        Room room = client.Room;
        if(room!=null)
        {
            //房间中处理对应玩家受到伤害
            room.PlayerTakeDamage(int.Parse(data), client);
        }
       
        return null;
    }

    /// <summary>
    /// 有玩家退出战斗
    /// </summary>
    public string QuitBattle(string data, Client client, Server server)
    {
        client.Player_Hp = 0;
        Room room = client.Room;
        if (room != null)
        {
            room.BroadCastAllClientInRoom(null, ActionCode.QuitBattle, "qb");
            //游戏结束，选择退出的客户端玩家失败
            room.SendGameOverToClient();
        }
        return null;
    }

    public string Chatting(string data, Client client, Server server)
    {
        Room room = client.Room;
        if (room != null)
        {
            //向房间中所有玩家发送聊天信息
            room.BroadCastAllClientInRoom(null, ActionCode.Chatting, data);
        }
        return null;
    }
}


