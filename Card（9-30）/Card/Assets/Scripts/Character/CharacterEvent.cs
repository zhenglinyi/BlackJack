using System;
using System.Collections.Generic;



public class CharacterEvent
{
    public const int INIT_MY_CARD = 0;//初始化自身卡牌
    public const int INIT_LEFT_CARD = 1;//初始化左边玩家的卡牌
    public const int INIT_RIGHT_CARD = 2;//初始化右边玩家的卡牌

    public const int ADD_MY_CARD = 3;//自身玩家添加底牌
    public const int ADD_LEFT_CARD = 4;//左边玩家添加底牌
    public const int ADD_RIGHT_CARD = 5;//右边玩家添加底牌

    public const int DEAL_CARD = 6;//出牌

    public const int REMOVE_MY_CARD = 7;//移除自身手牌
    public const int REMOVE_LEFT_CARD = 8;//移除左边手牌
    public const int REMOVE_RIGHT_CARD = 9;//移除右边手牌

    public const int UPDATE_SHOW_DESK = 10;//更新桌面的显示

    //21点的事件
    public const int INIT_PLAYER_CARD = 21;
    public const int INIT_DEALER_CARD = 22;
    public const int GET_PLAYER_CARD = 23;
    public const int GET_DEALER_CARD = 24;
    public const int CLEAR_PLAYER_CARD = 25;
    public const int CLEAR_DEALER_CARD = 26;
    public const int CONVERT_FIRST_CARD = 27;

    //21点 多人
    public const int MULTI_INIT_DEALER_CARD = 42;
    public const int MULTI_INIT_PLAYER1_CARD = 43;
    public const int MULTI_INIT_PLAYER2_CARD = 44;
    public const int MULTI_INIT_PLAYER3_CARD = 45;
    public const int MULTI_INIT_PLAYER4_CARD = 46;

    public const int MULTI_CLEAR_PLAYER1_CARD = 47;
    public const int MULTI_HIT_PLAYER1_CARD = 48;
    public const int MULTI_CLEAR_PLAYER2_CARD = 49;
    public const int MULTI_HIT_PLAYER2_CARD = 50;
    public const int MULTI_CLEAR_PLAYER3_CARD = 51;
    public const int MULTI_HIT_PLAYER3_CARD = 52;
    public const int MULTI_CLEAR_PLAYER4_CARD = 53;
    public const int MULTI_HIT_PLAYER4_CARD = 54;
    public const int MULTI_HIT_DEALER_CARD = 55;
    public const int MULTI_CLEAR_DEALER_CARD = 56;
    public const int MULTI_CONVERT_FIRST_CARD = 57;
    //训练模式
    public const int RUNCOUNT_INIT_CARD = 58;
    public const int RUNCOUNT_CLEAR_CARD = 59;

    public const int BASIC_PLAYER_INIT_CARD = 60;
    public const int BASIC_PLAYER_CLEAR_CARD = 61;
    public const int BASIC_DEALER_INIT_CARD = 62;
    public const int BASIC_DEALER_CLEAR_CARD = 63;
    public const int BASIC_CONVERT_FIRST_CARD = 64;
}
