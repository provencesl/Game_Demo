using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using MySql.Data.MySqlClient;
using Common;

public class Client
{
    #region 字段
    private Socket clientSocket;   //客户端Socket
    private Server server;         //与之相连的服务端
    private MySqlConnection mysqlConnect;   //数据库连接

    private Message message = new Message();    //信息解析类

    private User user;        //用户基础信息
    private Result result;    //用户战绩信息
    private ResultDAO resDAO = new ResultDAO();

    private Room room;     //当前客户端所在的房间

    private bool isLoginClient = false;   //该客户端是否已登录
   
    #endregion


    #region 属性
    public bool IsLoginClient
    {
        get { return isLoginClient; }
    }
    public MySqlConnection MysqlConnect
    {
        get { return mysqlConnect; }
    }

    public Room Room
    {
        set { room = value; }
        get { return room; }
    }

    public int UserId
    {
        get { return user.Id; }
    }

    public bool IsRoomOwner
    {
        get
        {

            return room.IsRoomOwner(this);
        }
    }

    public int Player_Hp
    {
        get;set;
    }
    #endregion


    #region 构造方法
    public Client() { }

    public Client(Socket clientSocket, Server server)
    {
        //建立数据库连接
        mysqlConnect = ConnHelper.Connect();
        this.clientSocket = clientSocket;
        this.server = server;
    }
    #endregion

    #region 常规方法
    /// <summary>
    ///  当前客户端玩家承受伤害
    /// </summary>
    /// <param name="damage">受到的伤害</param>
    /// <returns>此时该客户端玩家生命值是否为0</returns>
    public bool TakeDamage(int damage)
    {
        Player_Hp -= damage;
        //生命值不能小于0
        Player_Hp = Math.Max(Player_Hp, 0);
        //当前生命值是否为0
        return Player_Hp <= 0;
    }

    /// <summary>
    /// 玩家是否死亡
    /// </summary>
    public bool IsDie()
    {
        return Player_Hp <= 0;
    }

    /// <summary>
    /// 设置该客户端用户的数据信息
    /// </summary>
    public void SetUserData(User user, Result result)
    {
        this.user = user;
        this.result = result;
    }

    /// <summary>
    /// 获取该客户端的用户数据信息
    /// </summary>
    public string GetUserData()
    {
        return user.Id + "," + user.UserName + "," + result.TotalAmount + "," + result.WinAmount;
    }
    /// <summary>
    /// 启动客户端
    /// </summary>
    public void Start()
    {
        if (clientSocket == null || clientSocket.Connected == false)
        {
            CloseClient();
            return;
        }
        //客户端开始开启接收服务端的消息
        clientSocket.BeginReceive(
            message.Data,
            message.StartIndex,
            message.RemainLength,
            SocketFlags.None,
            ReceiveCallBack, null);
    }

    /// <summary>
    /// 向客户端发送消息
    /// </summary>
    public void Send(ActionCode actionCode, string data)
    {
        try
        {
            byte[] bytesBuffer = Message.PackData(actionCode, data);
            clientSocket.Send(bytesBuffer);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
    }

    /// <summary>
    /// 更新当前客户端战绩
    /// </summary>
    /// <param name="isVictory">是否取得胜利</param>
    public void UpdateResult(bool isVictory)
    {
        UpdateResultToDB(isVictory);
        UpdateResultToClient();
    }

    /// <summary>
    /// 更新数据库中的战绩数据
    /// </summary>
    void UpdateResultToDB(bool isVictory)
    {
        result.TotalAmount++;
        if (isVictory == true)
        {
            result.WinAmount++;
        }
        //数据库处理更新战绩信息
        resDAO.HandleUpdateResult(mysqlConnect, result);
    }

    /// <summary>
    /// 更新客户端的玩家战绩数据
    /// </summary>
    void UpdateResultToClient()
    {
        string resStr = string.Format("{0},{1}", result.TotalAmount, result.WinAmount);
        Send(ActionCode.UpdateResult, resStr);
    }
    #endregion

    #region 回调方法
    /// <summary>
    /// 客户端接收信息回调
    /// </summary>
    private void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            if (clientSocket == null || clientSocket.Connected == false)
            {
                CloseClient();
                return;
            }
            int count = clientSocket.EndReceive(ar);
           
            if (count == 0)
            {
                //断开连接
                CloseClient();
            }
            //解析接收到的消息
            message.ReadMessage(count, MessageCallBack);
            //再次开启接收
            Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            CloseClient();
        }       
    }

    /// <summary>
    /// 消息处理回调
    /// </summary>
    private void MessageCallBack(RequestCode requestCode, ActionCode actionCode, string data)
    {
        //使用服务端处理
        server.HandleRequest(requestCode, actionCode, this, data);
    }
    #endregion

    #region 帮助方法
    /// <summary>
    /// 关闭客户端与服务端的连接
    /// </summary>
    private void CloseClient()
    {
        //关闭与数据库的连接
        ConnHelper.CloseConnect(mysqlConnect);

        if (room != null)
        {
            server.HandleRequest(RequestCode.Room, ActionCode.QuitRoom, this, "");
        }
        
        if (clientSocket != null)
            clientSocket.Close();       
        //服务端移除该客户端
        server.RemoveClient(this);
    }
    #endregion


}

