using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using MySql.Data;

public class UserDAO
{
    /// <summary>
    /// 校验用户名和密码
    /// </summary>
    public User VerifyUser(MySqlConnection conn, string userName, string password)
    {
        MySqlDataReader reader = null;
        try
        {
            //根据用户输入的用户名和密码在数据库中进行查询
            string cmdStr = "select * from user where username = @username and password = @password";
            MySqlCommand cmd = new MySqlCommand(cmdStr, conn);

            cmd.Parameters.AddWithValue("username", userName);
            cmd.Parameters.AddWithValue("password", password);

            reader = cmd.ExecuteReader();
            //在数据库中读取到该记录数据
            if (reader.Read())
            {
                int id = reader.GetInt32("userid");
                //构造用户对象
                User user = new User(id, userName, password);
                return user;
            }
            else
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("执行VerifyUser出现程序异常" + e);
        }
        finally
        {
            if (reader != null) reader.Close();
        }
        return null;
    }

    /// <summary>
    /// 在数据库中是否存在该用户
    /// </summary>
    public bool GetUserInSQL(MySqlConnection conn, string userName)
    {
        MySqlDataReader reader = null;
        try
        {
            //根据用户输入的用户名和密码在数据库中进行查询
            string cmdStr = "select * from user where username = @username";
            MySqlCommand cmd = new MySqlCommand(cmdStr, conn);

            cmd.Parameters.AddWithValue("username", userName);
            reader = cmd.ExecuteReader();
            //在数据库中存在该用户数据
            if (reader.HasRows)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("执行GetUserInSQL出现程序异常" + e);
        }
        finally
        {
            if (reader != null) reader.Close();
        }
        return false;
    }

    /// <summary>
    /// 在数据库中加入该用户信息
    /// </summary>
    public void AddUserToSQL(MySqlConnection conn, string userName, string password)
    {
        try
        {
            //根据用户输入的用户名和密码在数据库中进行查询
            string cmdStr = "insert into user set username = @username , password = @password";
            MySqlCommand cmd = new MySqlCommand(cmdStr, conn);

            cmd.Parameters.AddWithValue("username", userName);
            cmd.Parameters.AddWithValue("password", password);

            cmd.ExecuteNonQuery();
            
        }
        catch (Exception e)
        {
            Console.WriteLine("执行AddUserToSQL出现程序异常" + e);
        }
    }
}

