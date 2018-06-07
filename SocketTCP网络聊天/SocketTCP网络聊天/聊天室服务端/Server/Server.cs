using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using GameServer;
using Common;

public class Server
{
    #region 字段
    private IPEndPoint ipEndPoint;    //IP终结点
    private Socket serverSocket;

    //控制器管理类
    private ControllerManager controllerManager;

    //可管理的客户端集合
    public List<Client> clientList = new List<Client>();
    //已登录在线的客户端
    public List<Client> onlineClinetList = new List<Client>();

    //房间集合
    private List<Room> roomList = new List<Room>();

    //登录在线的总人数
    private int onlineAmount = 0;

    public int OnlineAmount
    {
        get { return onlineAmount; }
        set { onlineAmount = value; }
    }

    #endregion

    #region 构造方法
    public Server() { }
    public Server(string ipStr, int port)
    {
        controllerManager = new ControllerManager(this);
        SetIpAndProt(ipStr, port);
    }
    #endregion

    #region 常规方法
    /// <summary>
    /// 设置IP地址和端口号
    /// </summary>
    /// <param name="ipStr">ip地址</param>
    /// <param name="port">端口号</param>
    private void SetIpAndProt(string ipStr, int port)
    {
        ipEndPoint = new IPEndPoint(IPAddress.Parse(ipStr), port);
    }

    /// <summary>
    /// 服务端发送响应给客户端
    /// </summary>
    public void SendResponse(Client client, ActionCode actionCode, string data)
    {
        client.Send(actionCode, data);
    }

    /// <summary>
    /// 启动服务端
    /// </summary>
    public void StartSocket()
    {
        //创建服务端Socket，使用TCP协议
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //绑定ip终结点
        serverSocket.Bind(ipEndPoint);
        //开启监听客户端等待连接队列
        serverSocket.Listen(0);

        //服务端开始接收连接的客户端
        serverSocket.BeginAccept(AcceptCallBack, null);
    }
    #endregion

    #region 回调方法
    /// <summary>
    /// 接收客户端响应回调
    /// </summary>
    private void AcceptCallBack(IAsyncResult ar)
    {
        //接收到一个连接的客户端
        Socket clientSocket = serverSocket.EndAccept(ar);
        //获取该客户端
        Client client = new Client(clientSocket, this);
        client.Start();
        clientList.Add(client);        

        serverSocket.BeginAccept(AcceptCallBack, null);
    }

    /// <summary>
    /// 服务端处理客户端请求
    /// </summary>
    public void HandleRequest(RequestCode requestCode, ActionCode actionCode,Client client, string data)
    {
        //使用controllerManager中处理方法
        controllerManager.HandleRequest(requestCode, actionCode, client, data);
    }
    #endregion

    #region 帮助方法
    /// <summary>
    /// 移除管理集合中的客户端
    /// </summary>
    public void RemoveClient(Client client)
    {
        //锁住客户端管理集合
        lock (clientList)
        {
            clientList.Remove(client);
        }
        lock (onlineClinetList)
        {
            onlineClinetList.Remove(client);
        }
    }

    /// <summary>
    /// 服务端创建房间
    /// </summary>
    public void HandleCreateRoom(Client client)
    {
        Room room = new Room(this);
        room.AddClient(client);
        roomList.Add(room);
    }

    /// <summary>
    /// 服务端移除房间
    /// </summary>
    public void HandleRemoveRoom(Room room)
    {
        if (roomList != null && room != null)
        {
            roomList.Remove(room);
        }
    }

    /// <summary>
    /// 在服务端根据房间Id获取房间
    /// </summary>
    public Room GetRoomByRoomId(int roomid)
    {
        if (roomList.Count > 0)
        {
            foreach (Room room in roomList)
            {
                if (room.RoomId == roomid)
                    return room;
            }
           
        }
        return null;
    }
    /// <summary>
    /// 获取当前服务器持有的房间集合
    /// </summary>
    public List<Room> GetRoomList()
    {
        return roomList;
    }
    #endregion 

}

