using System;
using System.Collections.Generic;
using System.Text;


public class User
{
   
    public User(int id, string userName, string password)
    {
        this.Id = id;
        this.UserName = userName;
        this.Password = password;
    }

    public int Id
    {
        set;
        get;
    }

    public string UserName
    {
        set;
        get;
    }

    public string Password
    {
        set;
        get;
    }
    
}

