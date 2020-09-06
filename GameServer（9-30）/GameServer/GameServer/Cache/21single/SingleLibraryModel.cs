using Protocol.Constant;
using Protocol.Dto.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache._21single
{
    /// <summary>
    /// 牌库
    /// </summary>
    public class SingleLibraryModel
    {
        /// <summary>
        /// 所有牌的队列
        /// </summary>
        public Queue<CardDto> CardQueue { get; set; }

        public double trueCount;
        public int runCount;
        


        public SingleLibraryModel()
        {
            //创建牌
            create(4);//用两副牌
            //洗牌
            shuffle();
            trueCount = 0;
            runCount = 0;
        }

        public void Init()
        {
            //创建牌
            create(4);
            //洗牌
            shuffle();
            trueCount = 0;
            runCount = 0;

        }

        private void create(int CardsNum)
        {
            CardQueue = new Queue<CardDto>();
            while (CardsNum>0)
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
            CardDto card = CardQueue.Dequeue();
            refreshCount(card);
            return card;
        }

        public void refreshCount(CardDto card)
        {
            int w = card.Weight;
            if (w >= 10 || w == 1)
            {
                runCount -= 1;
            }
            else if (w < 7 && w > 1)
            {
                runCount += 1;
            }
            else
            {
                runCount += 0;
            }
            trueCount = runCount / ((double)(CardQueue.Count) / 52);

        }

        public CountStrategyDto GetCountStrategyDto()
        {
            CountStrategyDto countStrategyDto = new CountStrategyDto();
            countStrategyDto.runCount = this.runCount;
            countStrategyDto.trueCount = this.trueCount;
            countStrategyDto.containCardCount = CardQueue.Count;
            return countStrategyDto;
        }
    }
}
