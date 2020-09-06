using Protocol.Dto.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Constant
{
    /// <summary>
    /// 21点卡牌权值
    /// </summary>
    public class CardWeight21
    {
        public const int ONE = 1;
        public const int TWO = 2;
        public const int THREE = 3;
        public const int FOUR = 4;
        public const int FIVE = 5;
        public const int SIX = 6;
        public const int SEVEN = 7;
        public const int EIGHT = 8;
        public const int NINE = 9;
        public const int TEN = 10;

        public const int JACK = 11;
        public const int QUEEN = 12;
        public const int KING = 13;

        public static string GetString(int weight)
        {
            switch (weight)
            {
                case 3:
                    return "Three";
                case 4:
                    return "Four";
                case 5:
                    return "Five";
                case 6:
                    return "Six";
                case 7:
                    return "Seven";
                case 8:
                    return "Eight";
                case 9:
                    return "Nine";
                case 10:
                    return "Ten";
                case 11:
                    return "Jack";
                case 12:
                    return "Queen";
                case 13:
                    return "King";
                case 1:
                    return "One";
                case 2:
                    return "Two";
                
                default:
                    throw new Exception("不存在这样的权值");
            }
        }

        /// <summary>
        /// 获取总的点数
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static int GetWeight(List<CardDto> cardList)
        {
            int totalWeight = 0;
            foreach(CardDto dto in cardList)
            {
                int w = dto.Weight;
                if(w>=10)
                {
                    totalWeight += 10;
                }
                else
                {
                    totalWeight += w;
                }
            }
            foreach (CardDto dto in cardList)
            {
                int w = dto.Weight;
                if(w==1&& totalWeight+10<=21)
                {
                    totalWeight += 10;
                }

            }
            return totalWeight;
        }


    }
}
