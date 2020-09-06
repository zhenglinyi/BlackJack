using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol.Constant;
using Protocol.Dto.Multi;

namespace GameServer.Cache._21mutiFight
{
    public class MultiLibraryModel
    {
        /// <summary>
        /// 所有牌的队列
        /// </summary>
        public Queue<CardDto> CardQueue { get; set; }

        public MultiLibraryModel()
        {
            //创建牌
            create(4);//用两副牌
            //洗牌
            shuffle();
        }

        public void Init()
        {
            //创建牌
            create(4);
            //洗牌
            shuffle();
        }

        private void create(int CardsNum)
        {
            CardQueue = new Queue<CardDto>();
            while (CardsNum > 0)
            {
                //创建普通的牌
                for (int color = CardColor.CLUB; color <= CardColor.SQUARE; color++)
                {
                    for (int weight = CardWeight21.ONE; weight <= CardWeight21.KING; weight++)
                    {
                        string cardName = CardColor.GetString(color) + CardWeight21.GetString(weight);
                        CardDto card = new CardDto(cardName, color, weight);
                        //添加到 CardQueue  里面
                        CardQueue.Enqueue(card);
                    }
                }
                CardsNum--;

            }


        }

        /// <summary>
        /// 洗牌
        /// </summary>
        private void shuffle()
        {
            List<CardDto> newList = new List<CardDto>();
            Random r = new Random();
            // 1 2 3 4 5 6 7
            foreach (CardDto card in CardQueue)
            {
                int index = r.Next(0, newList.Count + 1);
                // 6 2 5 4 3 7 1...
                newList.Insert(index, card);
            }
            CardQueue.Clear();
            foreach (CardDto card in newList)
            {
                CardQueue.Enqueue(card);
            }
        }

        /// <summary>
        /// 发牌
        /// </summary>
        /// <returns></returns>
        public CardDto Deal()
        {
            return CardQueue.Dequeue();
        }

        public int RemainCardNum()
        {
            return CardQueue.Count;
        }
    }

}
