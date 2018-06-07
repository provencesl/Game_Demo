using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using MySql.Data;

public class ConnHelper
{
    private const string CONNECTIONSTRING = "datasource=127.0.0.1;port=3306;database=gamebase;user=root;pwd=root;";

    /// <summary>
    /// 连接数据库
    /// </summary>
    public static MySqlConnection Connect()
    {
        MySqlConnection connect = new MySqlConnection(CONNECTIONSTRING);
        try
        {         
            connect.Open();
            return connect;
        }
        catch (Exception e)
        {
            Console.WriteLine("没有与数据库建立连接：" + e);
            return null;
        }
    }

    /// <summary>
    /// 关闭相应的数据库连接
    /// </summary>
    /// <param name="connect"></param>
    public static void CloseConnect(MySqlConnection connect)
    {

        if (connect != null)
        {
            connect.Close();
        }
        else
        {
            Console.WriteLine("数据库连接不能为空");
        }
    }
}

