using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Protocol.Dto.Multi
{
    /// <summary>
    /// 分牌发送的dto
    /// </summary>
    [Serializable]
    public class SplitBroDto
    {
        public int userId;
        public int position;
        public List<CardDto> cardList;

        public SplitBroDto()
        {
            this.userId = -1;
            this.position = -1;
            this.cardList = new List<CardDto>();
        }

    }
}
