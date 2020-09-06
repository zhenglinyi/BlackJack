using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code
{
    public class _21MultiCode
    {
        public const int MATCH_CREQ = 0;//匹配
        public const int MATCH_SRES = 1;

        public const int ENTER_CREQ = 2;
        public const int ENTER_SRES = 3;
        public const int ENTER_BRO = 4;

        //准备
        public const int READY_CREQ = 5;
        public const int READY_BRO = 6;

        //开始游戏
        public const int START_BRO = 7;

        //离开
        public const int LEAVE_CREQ = 8;
 
        public const int LEAVE_BRO = 9;

        public const int SET_WAGER_CREQ = 10;

        public const int INIT_CARD_BRO = 11;
        public const int TURN_HS_BRO = 12;

        public const int OVER_HAND_BRO = 13;

        public const int SPLIT_CREQ = 14;
        public const int SPLIT_BRO = 15;

        public const int SPLIT_CAN_NEXT_SRES = 16;

        public const int SPLIT_NEXT_CREQ = 17;

        public const int HIT_CREQ = 18;//要牌
        public const int HIT_BRO = 19;

        public const int STAND_CREQ = 20;//不要

        public const int DOUBLE_CREQ = 21;//加倍

        public const int TURN_DEALER_BRO = 22;

        public const int DEALER_HIT_BRO = 23;

        public const int OVER_BRO = 25;

        public const int COUNT_DOWN_BRO = 26;

        public const int STOP_COUNT_DOWN_BRO = 27;


    }
}
