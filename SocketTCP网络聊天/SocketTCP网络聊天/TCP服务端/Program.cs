using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TCP服务端
{
    class Program
    {
        
        #region 字段
        //服务端接收到的数据
        private static byte[] dataBuffer = new byte[1024];   

        //消息处理类
        private static Message m_Message = new Message();
        #endregion


        #region 方法
        //执行程序入口
        static void Main(string[] args)
        {
            StartServerAsync();
            Console.ReadKey();
        }

        /// <summary>
        /// 服务端异步客户端
        /// </summary>
        static void StartServerAsync()
        {
            //新建服务端Socket
            //AddressFamily.InterNetwork  IP4类型的网络地址
            //SocketType.Stream    使用流传输
            //ProtocolType.Tcp     使用TCP协议
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //获取本地IP地址
            IPAddress ipAdress = IPAddress.Parse("192.168.56.1");
            //指定访问点，Ip地址与端口号
            IPEndPoint ipEndPoint = new IPEndPoint(ipAdress, 6890);
            //服务端绑定访问点
            serverSocket.Bind(ipEndPoint);
            //服务端监听该端口，并设置等待队列最大长度
            serverSocket.Listen(10);

            //有客户端访问该服务端(异步)
            serverSocket.BeginAccept(AcceptCallBack, serverSocket);

        }
        #endregion

        #region 回调事件
        /// <summary>
        /// 服务端异步接收客户端连接
        /// </summary>
        static void AcceptCallBack(IAsyncResult asyncResoult)
        {
            Socket serverSocket = asyncResoult.AsyncState as Socket;
            //获取一个连接的客户端,结束接收
            Socket clientSocket = serverSocket.EndAccept(asyncResoult);

            //向客户端发送消息
            string str = "Hellow client, I am server. 你好客户端，我是服务端";
            byte[] bytStr = Encoding.UTF8.GetBytes(str);
            clientSocket.Send(bytStr);

            //服务端接收客户端发送过来的消息（异步）
            clientSocket.BeginReceive(
                m_Message.Data,
                m_Message.StartIndex,
                m_Message.RemainLength,
                SocketFlags.None,
                RecevieCallBack, clientSocket);
            //再次开启接收客户端
            serverSocket.BeginAccept(AcceptCallBack, serverSocket);
        }

        /// <summary>
        /// 服务端异步获取客户端发送的消息
        /// </summary>
        static void RecevieCallBack(IAsyncResult asyncResoult)
        {
            Socket clientSocket = null;
            try
            {
                clientSocket = asyncResoult.AsyncState as Socket;
                int count = clientSocket.EndReceive(asyncResoult);
                //当客户端发送消息为空时
                if (count == 0)
                {
                    clientSocket.Close();
                    return;
                }
                //消息中字节存储数量及索引增加
                m_Message.AddCount(count);

                string s = Encoding.UTF8.GetString(m_Message.Data, 0, count);
                m_Message.ReadMessage();
                Console.WriteLine("接收客户端传递过来的数据：" + s + ",");
                //再次开启异步接收（回调）
                //服务端接收客户端发送过来的消息（异步）
                clientSocket.BeginReceive(
                    m_Message.Data,
                    m_Message.StartIndex,
                    m_Message.RemainLength,
                    SocketFlags.None,
                    RecevieCallBack, clientSocket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                clientSocket.Close();
            }
            //结束接收数据

        }
        #endregion
                
    }
}
