using System;
using System.Collections.Generic;
using System.Text;
using Common;

public class RoomController:BaseController
{
    public RoomController()
    {
        requestCode = RequestCode.Room;
    }

    /// <summary>
    /// 创建房间
    /// </summary>
    public string CreateRoom(string data, Client client, Server server)
    {
        server.HandleCreateRoom(client);
        //返回创建成功结果和房主的角色类型
        return ((int)RetrunCode.Success).ToString() + "," + ((int)RoleType.BlueRole).ToString();
    }

    /// <summary>
    /// 列出房间列表
    /// </summary>
    public string ListRoom(string data, Client client, Server server)
    {
        StringBuilder sb = new StringBuilder();

        foreach (Room room in server.GetRoomList())
        {
            //当房间为等待加入状态
            if (room.IsWaitingJion())
            {
                //等到该房间房主信息
                sb.Append(room.GetRoomOwnerData() + "|");
            }
        }
        if (sb.Length == 0)
        {
            sb.Append("0");
        }
        else
        {
            //移除最后一位"|"字符
            sb.Remove(sb.Length - 1, 1);
        }
        return sb.ToString();
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    public string JoinRoom(string data, Client client, Server server)
    {
        StringBuilder sb = new StringBuilder();

        int roomId = int.Parse(data);
        //获取需加入的房间
        Room room = server.GetRoomByRoomId(roomId);
        if (room != null)
        {
            //并且此时房间为等待加入状态
            if (room.IsWaitingJion())
            {
                //自身客户端加入到房间
                room.AddClient(client);
                string returnSuccess = ((int)RetrunCode.Success).ToString();
                //加入房间的角色类型
                string returnRoleType = ((int)RoleType.RedRole).ToString();
                string allData = room.GetAllPlayerDataInRoom();

                //房间向所有在房间中的客户端广播消息：更新房间信息
                room.BroadCastAllClientInRoom(client, ActionCode.UpdateRoom, allData);

                //数据格式：retrunCode,returnRoleType-id,userName,totalAmount,winAmount|retrunCode,returnRoleType-id,userName,totalAmount,winAmount...
                sb.Append(returnSuccess + "," + returnRoleType + "-" + allData); ;               
            }
            else
            {
                //加入失败
                sb.Append(((int)RetrunCode.Fail).ToString());
            }
        }
        else        
        {
            sb.Append(((int)RetrunCode.NotFound).ToString());
        }

        return sb.ToString();
    }

    /// <summary>
    /// 退出房间
    /// </summary>
    public string QuitRoom(string data, Client client, Server server)
    {
        bool isRoomOwner = client.IsRoomOwner;
        Room room = client.Room;


        if (room != null && room.RoomState == RoomState.Battle)
        {
            server.HandleRequest(RequestCode.Game, ActionCode.QuitBattle, client, "");
         
            return null;
        }
            
        if (isRoomOwner == true)
        {           
            //当退出房间的客户端为房主时 
            room.BroadCastAllClientInRoom(client, ActionCode.QuitRoom, ((int)RetrunCode.Success).ToString());
            //房主所在的房间销毁
            client.Room.CloseRoom();
            return ((int)RetrunCode.Success).ToString();
        }
        else
        {         
            //向房间广播更新信息
            room.BroadCastAllClientInRoom(client, ActionCode.UpdateRoom, room.GetRoomOwnerData());
            client.Room.RemoveClient(client);
            return ((int)RetrunCode.Success).ToString();
        }
    }

    /// <summary>
    /// 给所有存在的客户端发送全局聊天信息
    /// </summary>
    public string GlobalChatting(string data, Client client, Server server)
    {
        foreach (Client itemClient in server.clientList)
        {
            server.SendResponse(itemClient, ActionCode.GlobalChatting, data);
        }

        return null;
    }

    /// <summary>
    /// 当前在线的客户端数量
    /// </summary>
    public string OnlineAmount(string data, Client client, Server server)
    {
        string amountStr = server.onlineClinetList.Count.ToString();
        return amountStr;
    }
}

