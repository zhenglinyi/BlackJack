using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Protocol.Constant;

namespace Protocol.Constant.Train
{
    public class TrainLibraryModel
    {
        /// <summary>
        /// 所有牌的队列
        /// </summary>
        public Queue<Card> CardQueue { get; set; }

        public TrainLibraryModel()
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
            CardQueue = new Queue<Card>();
            while (CardsNum > 0)
            {
                //创建普通的牌
                for (int color = CardColor.CLUB; color <= CardColor.SQUARE; color++)
                {
                    for (int weight = CardWeight21.ONE; weight <= CardWeight21.KING; weight++)
                    {
                        string cardName = CardColor.GetString(color) + CardWeight21.GetString(weight);
                        Card card = new Card(cardName, color, weight);
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
            List<Card> newList = new List<Card>();
            Random r = new Random();
            // 1 2 3 4 5 6 7
            foreach (Card card in CardQueue)
            {
                int index = r.Next(0, newList.Count + 1);
                // 6 2 5 4 3 7 1...
                newList.Insert(index, card);
            }
            CardQueue.Clear();
            foreach (Card card in newList)
            {
                CardQueue.Enqueue(card);
            }
        }

        /// <summary>
        /// 发牌
        /// </summary>
        /// <returns></returns>
        public Card Deal()
        {
            return CardQueue.Dequeue();
        }

        public List<Card> MultiDeal(int num)
        {
            List<Card> cardList = new List<Card>();
            for(int i=0;i<num;i++)
            {
                cardList.Add(CardQueue.Dequeue());
            }
            return cardList;
        }

        public int RemainCardNum()
        {
            return CardQueue.Count;
        }
    }

}
