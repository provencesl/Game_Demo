using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TCP客户端
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建客户端Socket
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //远程连接的终点Ip地址
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("192.168.56.1"), 6890);
            //连接服务端的Ip地址和端口
            clientSocket.Connect(ipEndPoint);

            //接收服务端的消息
            byte[] buffer = new byte[1024];
            int count = clientSocket.Receive(buffer);
            string bufferStr = Encoding.UTF8.GetString(buffer, 0, count);
            Console.WriteLine("服务端发送过来的消息：" + bufferStr);

            //while (true)
            //{
            //    //向服务端发送消息
            //    string s = Console.ReadLine();
            //    //断开与服务端链接
            //    if (s == "c")
            //    {
            //        clientSocket.Close();
            //        return;
            //    }
            //    clientSocket.Send(Encoding.UTF8.GetBytes(s));
            //}

            for (int i = 0; i < 100; i++)
            {
                clientSocket.Send(Message.GetBytes(i.ToString()));
            }

            Console.ReadKey();
            clientSocket.Close();
        }
    }
}
