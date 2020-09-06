using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Dto.Multi
{
    [Serializable]
    public class TurnHandDto
    {
        public bool canSplit;
        public int userId;
        public TurnHandDto(bool canSplit, int userId)
        {
            this.canSplit = canSplit;
            this.userId = userId;

        }
    }
}
