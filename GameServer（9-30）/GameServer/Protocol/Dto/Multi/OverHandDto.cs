using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Dto.Multi
{
    /// <summary>
    /// 一把牌的结束
    /// </summary>
    [Serializable]
    public class OverHandDto
    {
        public int userId;
        public int position;
        public int type;

        public OverHandDto(int userId,int position,int type)
        {
            this.userId = userId;
            this.position = position;
            this.type = type;//1 21点 2 爆牌 3 停牌 4加倍后爆牌 5加倍
        }
    }
}
