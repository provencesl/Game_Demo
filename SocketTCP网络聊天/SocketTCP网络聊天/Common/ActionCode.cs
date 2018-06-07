using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 操作
/// </summary>
public enum ActionCode
{
    None,
    Login,         //登录
    Register,      //注册

    ListRoom,       //列出房间列表
    CreateRoom,      //创建房间
    JoinRoom,        //加入房间
    UpdateRoom,       //更新房间信息，当有新玩家加入该房间时
    GlobalChatting,   //世界聊天
    QuitRoom,        //退出房间
    OnlineAmount,    //显示在线总人数

    StartGame,        //进入预备
    ShowTimer,        //显示计时器
    StartPlay,        //开始一对一聊天
    Move,              //角色移动
    Shoot,            //角色射击
    AttackDamage,     //攻击造成伤害
    GameOver,          //一对一聊天结束
    UpdateResult,       //更新数据
    QuitBattle,         //直接退出
    Chatting            //发起聊天
}

