using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Constant.Train
{
    public class RunError
    {
        public int lastCount;
        public int nowCount;
        public List<Card> cardList;
        public RunError()
        {
            this.lastCount = 0;
            this.nowCount = 0;
            this.cardList = new List<Card>();
        }
    }
}
