using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Dto.Multi
{
    /// <summary>
    /// 玩家的传输模型
    /// </summary>
    [Serializable]
    public class MultiPlayerDto
    {
        public int UserId;//用户id
        public int Wager;//赌金
        public int position;//1 2 3 4
        public bool isReady;//是否准备
        public List<CardDto> CardList;//自己拥有的牌
        public Queue<List<CardDto>> SpliteCardListQueue;//分牌后有一个队列
        public List<PlayerCardDto> CardListList;

        public MultiPlayerDto(int userId)
        {
            this.UserId = userId;
            this.Wager = 10;
            this.position = 0;
            this.isReady = false;
            this.CardList = new List<CardDto>();
            this.SpliteCardListQueue = new Queue<List<CardDto>>();
            this.CardListList = new List<PlayerCardDto>();
        }

        public void ResetPlayer()
        {
            this.isReady = false;
            this.CardList.Clear();
            this.SpliteCardListQueue.Clear();
            this.CardListList.Clear();
        }

        

    }
}
