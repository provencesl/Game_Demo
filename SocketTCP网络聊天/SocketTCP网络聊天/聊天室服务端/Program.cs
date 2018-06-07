using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server("172.21.0.16", 6688);
            server.StartSocket();

            Console.ReadKey();
        }
    }
}
