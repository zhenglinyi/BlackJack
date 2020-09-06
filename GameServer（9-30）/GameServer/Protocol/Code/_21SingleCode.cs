using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code
{
    public class _21SingleCode
    {
        public const int MATCH_CREQ = 0;//匹配
        public const int START_CREQ = 1;//开始
        public const int START_SRES = 2; 
        public const int GET_CREQ = 3;//要牌
        public const int GET_SRES = 4;
        public const int NGET_CREQ = 5;//不要
        public const int NGET_SRES = 6;
        public const int NEXT_CREQ = 7;//下一局
        public const int NEXT_SRES = 8;
        public const int LEAVE_CREQ = 9;//离开
        public const int LEAVE_SRES = 10;

        public const int OVER_SRES = 11;//服务器给客户端发送游戏结果

        public const int GET_PCARD_SRES = 12;//服务器给客户端闲家初始卡牌的响应
        public const int GET_DCARD_SRES = 13;//服务器给客户端庄家初始卡牌的响应
        public const int RESHUFF_SRES = 14;//服务器给客户端重新洗牌的响应
        public const int ADD_DCARD_SRES = 15;//给庄家发牌

        public const int SET_WAGER_CREQ = 16;//玩家下注

        public const int DOUBLE_CREQ = 17;//加倍

        public const int CAN_SPLIT_SRES = 18;//能够分牌
        public const int SPLIT_CREQ = 19;//分牌

        public const int SPLIT_PROMST_SRES = 20;//分牌后每一把牌的提示 只提示爆牌和21点
        public const int SPLIT_CAN_NEXT_SRES = 21;//还有牌，让客户端去点next
        public const int SPLIT_NEXT_CREQ = 22;//客户端请求next split


        public const int COUNT_STRATEGY_SRES = 23;
        public const int BASIC_STRATEGY_SRES = 24;



    }
}
