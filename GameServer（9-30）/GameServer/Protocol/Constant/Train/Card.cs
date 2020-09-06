using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Constant.Train
{
    /// <summary>
    /// 表示卡牌
    /// </summary>
    [Serializable]
    public class Card
    {
        public string Name;
        public int Color;//红桃
        public int Weight;//9

        public Card()
        {

        }

        public Card(string name, int color, int weight)
        {
            this.Name = name;
            this.Color = color;
            this.Weight = weight;
        }

    }
}
