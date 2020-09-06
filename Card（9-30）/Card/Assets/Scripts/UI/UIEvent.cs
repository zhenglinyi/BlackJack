using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
/// 存储所有的UI事件码
/// </summary>
public class UIEvent
{
    public const int START_PANEL_ACTIVE = 0;//设置开始面板的显示
    public const int REGIST_PANEL_ACTIVE = 1;//设置注册面板的显示

    public const int REFRESH_INFO_PANEL = 2;//刷新信息面板 参数：目前为止 是服务器定的
    public const int SHOW_ENTER_ROOM_BUTTON = 3;//显示进入房间按钮
    public const int CREATE_PANEL_ACTIVE = 4;//设置创建面板的显示

    public const int SET_TABLE_CARDS = 5;//设置底牌
    public const int SET_LEFT_PLAYER_DATA = 6;//设置左边的角色的数据
    public const int SET_RIGHT_PLAYER_DATA = 13;//设置右边角色数据
    //fixbug923
    //public const int SET_MY_PLAYER_DATA = 15;//设置自身角色数据

    public const int PLAYER_READY = 7;//角色准备
    public const int PLAYER_ENTER = 8;//角色进入
    public const int PLAYER_LEAVE = 9;//角色离开
    public const int PLAYER_CHAT = 10;//角色聊天
    public const int PLAYER_CHANGE_IDENTITY = 11;//角色更改身份
    public const int PLAYER_HIDE_STATE = 12;//开始游戏 角色隐藏状态面板
    public const int PLAYER_HIDE_READY_BUTTON = 17;//玩家准备 隐藏准备按钮

    public const int SHOW_GRAB_BUTTON = 16;//开始抢地主 显示抢地主的按钮
    public const int SHOW_DEAL_BUTTON = 14;//开始出牌 显示出牌按钮

    public const int CHANGE_MUTIPLE = 15;//改变倍数

    public const int SHOW_OVER_PANEL = 18;//显示结束面板

    public const int SHOW_GIVE_PANEL = 19;//赠送豆子面板

    public const int SHOW_COUNT_DOWN_PANEL = 20;

    public const int STOP_COUNT_DOWN_PANEL = 211;

    //....

    public const int PROMPT_MSG = int.MaxValue;


    //21点单人的事件
    public const int SINGLE_SHOW_GET_BUTTON = 21;
    public const int SINGLE_SHOW_NEXT_BUTTON = 22;
    public const int SINGLE_SHOW_START_BUTTON = 23;
    //public const int SINGLE_SHOW_DOUBLE_BUTTON = 24;
    public const int SINGLE_SHOW_SPLIT_BUTTON = 25;

    public const int SINGLE_SHOW_STARTWAGER = 26;

    public const int SINGLE_CHANGE_BEEN = 27;
    public const int SINGLE_CHANGE_WAGER = 28;

    public const int SINGLE_SHOW_NEXTSPLIT_BUTTON = 29;
    public const int SINGLE_SHOW_OVER_PANEL = 30;
    public const int COUNT_SHOW_TIP_PANEL = 31;
    public const int STRATEGY_SHOW_TIP_PANEL = 32;

    //21点多人事件

    public const int MULTI_SHOW_ENTER_ROOM_BUTTON = 42;
    public const int MULTI_PLAYER_HIDE_STATE = 43;
    public const int MULTI_PLAYER_READY = 44;
    public const int MULTI_PLAYER_LEAVE = 45;
    public const int MULTI_PLAYER_ENTER = 46;
    public const int MULTI_PLAYER_CHANGE_IDENTITY = 47;//标记一下哪个是你
    public const int SET_1_PLAYER_DATA = 48;
    public const int SET_2_PLAYER_DATA = 49;
    public const int SET_3_PLAYER_DATA = 50;
    public const int SET_4_PLAYER_DATA = 51;

    public const int MULTI_PLAYER_HIDE_READY_BUTTON = 52;

    public const int MULTI_CHANGE_WAGER = 53;
    public const int MULTI_CHANGE_BEEN = 54;
    public const int MULTI_SHOW_HSD_BUTTON = 55;
    public const int MULTI_SHOW_SPLIT_BUTTON = 56;
    public const int MULTI_SHOW_SPLIT_NEXT_BUTTON = 57;

    public const int MULTI_SHOW_OVER_PANEL = 58;

    public const int MULTI_SHOW_NEXT_BUTTON = 59;

    //训练模式的事件码

    public const int RUN_SHOW_ERROR_PANEL = 60;
    public const int RUN_SHOW_NEXT_BUTTON = 61;
    public const int RUN_SHOW_TIP_PANEL = 62;
    public const int BASIC_SHOW_NEXT_BUTTON = 63;
    public const int BASIC_SHOW_TIP_PANEL = 64;

    public const int REFRESH_BASIC_STRATEGY = 65;
    public const int REFRESH_COUNT_STRATEGY = 66;


}
