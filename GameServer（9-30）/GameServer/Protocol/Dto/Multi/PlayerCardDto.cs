using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Dto.Multi
{
    [Serializable]
    public class PlayerCardDto
    {
        public List<CardDto> CardList;
        public int CardState;
        public bool isDouble;
        public int Weight;

        public PlayerCardDto()
        {
            this.CardList = new List<CardDto>();
            this.CardState = -1;
            this.isDouble = false;
            this.Weight = -1;
        }
    }
}
