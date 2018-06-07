using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 数据转换成字节数组
{
    class Program
    {
        static void Main(string[] args)
        {

            //byte[] dataBuffer = Encoding.UTF8.GetBytes("z");
            byte[] dataBuffer = BitConverter.GetBytes(256*256*256*127);

            foreach (byte item in dataBuffer)
            {
                Console.Write(item + ":");
            }
            Console.ReadKey();
        }
    }
}
