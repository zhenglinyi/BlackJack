using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Dto.Single
{
    [Serializable]
    public class SingleOverDto
    {
        public UserDto userDto;
        //public int whichwin; 
        public int dealerState;//1.21点(前两张牌就是21点）2.爆牌了吧 3.正常
        public int dealerWeight;
        public List<int> playerWeightList;
        public List<int> playerStateList;//0 21点平 ，1 21点赢，2 爆牌，3 加倍爆牌 ，4 加倍 赢 ，5 加倍 输，6赢 ，7 输,8 平
        public List<int> playerWinBeenList;

        //public SingleOverDto(UserDto userDto, int whichwin)
        //{
        //    this.userDto = userDto;
        //    this.whichwin = whichwin;
        //}

        public SingleOverDto()
        {
            this.userDto = new UserDto();
            this.playerWeightList = new List<int>();
            this.playerStateList = new List<int>();
            this.playerWinBeenList = new List<int>();
        }


    }
}
