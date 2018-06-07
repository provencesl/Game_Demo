using System;
using System.Collections.Generic;
using System.Text;
using Common;

public class UserController:BaseController
{
    private UserDAO userDAO = new UserDAO();
    private ResultDAO resultDAO = new ResultDAO();

    public UserController()
    {
        requestCode = RequestCode.User;
    }

    /// <summary>
    /// 处理登录请求
    /// </summary>
    public string Login(string data, Client client, Server server)
    {
        
        //分隔用户名和密码
        string[] loginInfo = data.Split(',');
        
        string userName = loginInfo[0];
        string password = loginInfo[1];
        
        //校验用户登录信息
        User user = userDAO.VerifyUser(client.MysqlConnect, userName, password);
        if (user == null)
        {
            //返回执行失败结果
            return ((int)RetrunCode.Fail).ToString();
        }
        else
        {
            //获取对应用户战绩数据
            Result res = resultDAO.GetResultByUserId(client.MysqlConnect, user.Id);
            //该客户端持有该用户数据
            client.SetUserData(user, res);

            if (!server.onlineClinetList.Contains(client))
                server.onlineClinetList.Add(client);
            //需发送的数据进行拼接
            string allData = string.Format("{0},{1},{2},{3}", ((int)RetrunCode.Success).ToString(), user.UserName, res.TotalAmount, res.WinAmount);
            return allData;
        }
    }

    /// <summary>
    /// 处理注册请求
    /// </summary>
    public string Register(string data, Client client, Server server)
    {
        //分隔用户名和密码
        string[] loginInfo = data.Split(',');

        string userName = loginInfo[0];
        string password = loginInfo[1];

        //在数据库中是否存在该用户
        bool isExistUser = userDAO.GetUserInSQL(client.MysqlConnect, userName);
        if (isExistUser)
        {
            //返回执行失败结果
            return ((int)RetrunCode.Fail).ToString();
        }
        else
        {
            //该用户不存在，在数据库中添加该用户信息
            userDAO.AddUserToSQL(client.MysqlConnect, userName, password);
            return ((int)RetrunCode.Success).ToString();
        }
    }


}

