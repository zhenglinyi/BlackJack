using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Dto.Single
{
    /// <summary>
    /// 玩家的传输模型
    /// </summary>
    [Serializable]
    public class SinglePlayerDto
    {
        public int UserId;//用户id
        public List<CardDto> CardList;//自己拥有的牌
        public Queue<List<CardDto>> SpliteCardListQueue;//分牌后有一个队列
        public List<PlayerCardDto> SpliteCardListList;//每次玩家结束一手牌后保存下来
        public List<int> weightList;//保存一下不是爆牌和21点情况下，闲家手牌的权值
        public List<int> multiList;//加倍情况是2，其余是1
        public int SplitNum;
        public List<double> winOrLose;//保存的赌金输赢赔率



        public SinglePlayerDto(int userId)
        {
            this.UserId = userId;
            this.CardList = new List<CardDto>();
            this.SpliteCardListQueue = new Queue<List<CardDto>>();
            this.SpliteCardListList = new List<PlayerCardDto>();
            this.weightList = new List<int>();
            this.multiList = new List<int>();
            this.SplitNum = 1;
            this.winOrLose = new List<double>();
        }

        /// <summary>
        /// 添加卡牌/
        /// </summary>
        /// <param name="card"></param>
        public void Add(CardDto card)
        {
            CardList.Add(card);
        }

        public void Remove(int index)
        {
            CardList.RemoveAt(index);
        }
    }
}
