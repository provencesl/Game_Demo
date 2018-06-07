using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Common;

public class ControllerManager
{
    //客户端请求对应的控制器字典
    private Dictionary<RequestCode, BaseController> controllerDict = new Dictionary<RequestCode, BaseController>();

    private Server server;   //服务端
    public ControllerManager(Server server)
    {
        this.server = server;
        InitController();
    }

    /// <summary>
    /// 初始化控制器
    /// </summary>
    private void InitController()
    {
        //创建所需的控制并添加到字典中
        DefaultController defaultController = new DefaultController();
        controllerDict.Add(defaultController.RequestCode, defaultController);
        controllerDict.Add(RequestCode.User, new UserController());
        controllerDict.Add(RequestCode.Room, new RoomController());
        controllerDict.Add(RequestCode.Game, new GameController());
    }

    /// <summary>
    /// 处理客户端请求
    /// </summary>
    /// <param name="requestCode">客户端请求</param>
    /// <param name="actionCode">对应的执行方法</param>
    /// <param name="data">客户端发送的数据</param>
    public void HandleRequest(RequestCode requestCode, ActionCode actionCode,Client client, string data)
    {
        //基础控制器
        BaseController controller = null;   
        //根据请求获取到该请求对应的控制器
        bool isGet = controllerDict.TryGetValue(requestCode, out controller);

        if (isGet == false)
        {
            Console.WriteLine("无法根据请求[" + requestCode + "]获取其对应的Controller");           
        } 

        //获取执行方法名称(解析对应枚举)
        string methodName = Enum.GetName(typeof(ActionCode), actionCode);
        //根据执行方法名称找到控制器中对应的方法
        MethodInfo mi = controller.GetType().GetMethod(methodName);

        if (mi == null)
        {
            Console.WriteLine("[警告]在Controller[" + controller.GetType() + "]中没有对应的处理方法[" + methodName + "]");
        }

        object[] parameters = new object[] { data, client, server };

        //传递数据参数，执行对应方法
        object o = mi.Invoke(controller, parameters);
        //当方法为空时，返回
        if (o == null || string.IsNullOrEmpty(o as string))
            return;

        //服务端将处理完的数据发送响应给客户端
        server.SendResponse(client, actionCode, o as string);
    }
}

