using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Dto.Multi
{
    [Serializable]
    public class InitCardDto
    {
        //0d 1 2 3 4
        public List<List<CardDto>> InitCardLists;
        public InitCardDto()
        {
            InitCardLists = new List<List<CardDto>>();
            for (int i = 0; i < 5; i++)
                InitCardLists.Add(null);
        }
    }
}
