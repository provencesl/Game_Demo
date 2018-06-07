using System;
using System.Collections.Generic;
using System.Text;
using Common;

public abstract class BaseController
{
    protected RequestCode requestCode = RequestCode.None;

    public RequestCode RequestCode
    {
        get { return requestCode; }
    }
    /// <summary>
    /// 默认处理方法
    /// </summary>
    public virtual string DefaultHandle(string data, Client client, Server server) { return null; }
}

