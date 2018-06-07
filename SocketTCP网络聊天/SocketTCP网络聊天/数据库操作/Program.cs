using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace 数据库操作
{
    class Program
    {
        static void Main(string[] args)
        {
            //连接数据库所需参数
            string connStr = "Database = text001; Data Source = 127.0.0.1; port = 3306; User Id = root; Password = root";

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            
            #region 查询
            /*
            //创建数据库命令
            MySqlCommand command = new MySqlCommand("select * from userinfo ", conn);
            //创建读取流并执行命令
            MySqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                string userName = reader.GetString("UserName");
                string userPasswaord = reader.GetString("UserPassword");
                Console.WriteLine(userName + " : " + userPasswaord);
            }
            reader.Close();
             */
            #endregion

            #region 插入
            /*
            string userName = "wwwwwq";
            string password = "pppq1234";

            //字符串拼接注入sql命令
            //MySqlCommand command = new MySqlCommand("insert into userinfo set UserName='" + userName + "'" + ",UserPassword='" + password + "'", conn);

            MySqlCommand cmd = new MySqlCommand("insert into userinfo set UserName=@un, UserPassword=@pwd", conn);

            cmd.Parameters.AddWithValue("un", userName);
            cmd.Parameters.AddWithValue("pwd", password);

            cmd.ExecuteNonQuery();
             */
            #endregion

            #region 删除
            /*
            MySqlCommand cmd = new MySqlCommand("delete from userinfo where id=@userid", conn);
            cmd.Parameters.AddWithValue("userid", 3);
            cmd.ExecuteNonQuery();
             */
            #endregion

            #region 修改
            MySqlCommand cmd = new MySqlCommand("update userinfo set username=@un where id=@userid", conn);
            cmd.Parameters.AddWithValue("userid", 4);
            cmd.Parameters.AddWithValue("un", "困难");
            cmd.ExecuteNonQuery();
            #endregion
            conn.Close();

            Console.ReadKey();
        }
    }
}
