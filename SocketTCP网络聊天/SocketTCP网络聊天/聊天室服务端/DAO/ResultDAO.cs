using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using MySql.Data;

public class ResultDAO
{
    /// <summary>
    /// 根据用户ID获取用户战绩数据
    /// </summary>
    public Result GetResultByUserId(MySqlConnection conn, int userId)
    {
        MySqlDataReader reader = null;
        try
        {
            //根据用户输入的用户名和密码在数据库中进行查询
            string cmdStr = "select * from result where userId = @userId";
            MySqlCommand cmd = new MySqlCommand(cmdStr, conn);

            cmd.Parameters.AddWithValue("userId", userId);

            reader = cmd.ExecuteReader();
            //在数据库中读取到该记录数据
            if (reader.Read())
            {
                int id = reader.GetInt32("id");
                int totalAmount = reader.GetInt32("totalamount");
                int winAmount = reader.GetInt32("winamount");
                //构造用户战绩对象
                Result res = new Result(id, userId, totalAmount, winAmount);
                return res;
            }
            else
            {
                Result res = new Result(-1, userId, 0, 0);
                return res;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("执行GetResultByUserId出现程序异常" + e);
        }
        finally
        {
            if (reader != null)
                reader.Close();
        }
        return null;
    }

    public void HandleUpdateResult(MySqlConnection conn, Result res)
    {
        try
        {
            MySqlCommand cmd = null;
            string cmdStr = "";
            if (res.Id <= -1)
            {
                //更新数据库中的战绩
                cmdStr = "insert into result set totalamount = @totalamount, winamount = @winamount, userId = @userId";
                cmd = new MySqlCommand(cmdStr, conn);
            }
            else
            {
                //更新数据库中的战绩
                cmdStr = "update result set totalamount = @totalamount, winamount = @winamount where userId = @userId";
                cmd = new MySqlCommand(cmdStr, conn);
            }
            cmd.Parameters.AddWithValue("totalamount", res.TotalAmount);
            cmd.Parameters.AddWithValue("winamount", res.WinAmount);
            cmd.Parameters.AddWithValue("userId", res.UserId);
            cmd.ExecuteNonQuery();

            //根据用户ID获取用户战绩数据
            if (res.Id <= -1)
            {
                Result tempRes = GetResultByUserId(conn, res.UserId);
                res.Id = tempRes.Id;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("执行HandleUpdateResult出现程序异常" + e);
        }
    }
}

