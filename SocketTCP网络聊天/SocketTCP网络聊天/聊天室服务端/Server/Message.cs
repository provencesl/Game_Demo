using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using Common;

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
    /// 读取(解析)客户端发送过来的数据
    /// </summary>
    public void ReadMessage(int newDataAmount, Action<RequestCode, ActionCode, string> ProcessDataCallBack)
    {
        //更新存储字节索引，数量增加
        startIndex += newDataAmount;
        while (true)
        {
            //当发送过来的一条数据长度小于四位即没有数据头时
            if (startIndex <= 4) return;

            //获取数据总长度
            int count = BitConverter.ToInt32(data, 0);
           
            //按数据头逐步解析每个字节数组
            if ((startIndex - 4) >= count)
            {
                //解析数据前的RequestCode，ActionCode
                RequestCode requestCode = (RequestCode)BitConverter.ToInt32(data, 4);
                ActionCode actionCode = (ActionCode)BitConverter.ToInt32(data, 8);
                //解析获取数据
                string s = Encoding.UTF8.GetString(data, 12, count - 8);
  
                ProcessDataCallBack(requestCode, actionCode, s);
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

    /// <summary>
    /// 打包数据(服务端发给客户端的相应数据)
    /// </summary>
    public static byte[] PackData(ActionCode actionCode, string data)
    {
        //将所需发送数据转化成字节数组
        byte[] actionBytes = BitConverter.GetBytes((int)actionCode);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        byte[] dataAmount = BitConverter.GetBytes(actionBytes.Length + dataBytes.Length);
        //合并数据
        byte[] allData = dataAmount.Concat(actionBytes).ToArray<byte>()
                            .Concat(dataBytes).ToArray<byte>();
        return allData;
    }

    #endregion
}

