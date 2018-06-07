using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class Message
{
    public static byte[] GetBytes(string str)
    {
        byte[] data = Encoding.UTF8.GetBytes(str);
        //获取字节长度
        int dataLength = data.Length;
        //将数据字节长度转化为字节数组
        byte[] lengthBuffer = BitConverter.GetBytes(dataLength);
        //数据字节长度与数据粘合在一起
        byte[] allData = lengthBuffer.Concat(data).ToArray();
        return allData;
    }
}

