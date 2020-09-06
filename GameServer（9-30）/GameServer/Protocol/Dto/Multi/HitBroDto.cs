using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Dto.Multi
{
    /// <summary>
    /// 要牌时的dto
    /// </summary>
    [Serializable]
    public class HitBroDto
    {
        public int userId;
        public int position;
        public CardDto card;

        public HitBroDto(int userId,int position,CardDto card)
        {
            this.userId = userId;
            this.position = position;
            this.card = card;
        }

    }
}
