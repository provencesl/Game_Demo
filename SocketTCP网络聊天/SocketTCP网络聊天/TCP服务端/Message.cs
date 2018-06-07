using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

public class Message
{

    #region 字段
    private byte[] data = new byte[1024];

    private int startIndex = 0;    //开始存储的字节所在字节数组中的位置
    #endregion

    #region 属性
    public byte[] Data
    {
        get { return data; }
    }

    public int StartIndex
    {
        get { return startIndex; }
    }

    //当前字节数组中剩余的可存储空间
    public int RemainLength
    {
        get { return data.Length - startIndex; }
    }
    #endregion

    #region 方法
    /// <summary>
    /// 更新存储字节索引，数量增加
    /// </summary>
    public void AddCount(int count)
    {
        startIndex += count;
    }
    #endregion

    /// <summary>
    /// 读取(解析)客户端发送过来的数据
    /// </summary>
    public void ReadMessage()
    {
        while (true)
        {
            //当发送过来的一条数据长度小于四位即没有数据头时
            if (startIndex < 4) return;

            //获取数据总长度
            int count = BitConverter.ToInt32(data, 0);
            //按数据头逐步解析每个字节数组
            if ((startIndex - 4) >= count)
            {
                Console.WriteLine("总长度：" + startIndex + "    当前包的长度" + count);
                string s = Encoding.UTF8.GetString(data, 4, count);
                Console.WriteLine("该包中的数据：" + s);
                //未解析的数据前移
                Array.Copy(data, count + 4, data, 0, startIndex - count - 4);
                startIndex -= (4 + count);
            }
            else
            {
                break;
            }
        }
    }
    

}

