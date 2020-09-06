using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Dto.Multi
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class GameOverDto
    {
        public int dealerWeight;
        public List<bool> isLeaveList;
        public List<List<int>> playerWeightListList;
        //1 21点，2 爆牌，3 加倍爆牌 ，4 加倍 赢 ，5 加倍 输，6赢 ，7 输,8 平
        public List<List<int>> playerStateListList;
        public List<List<int>> playerWinBeenListList;
        public List<UserDto> userDtoList;

        public GameOverDto()
        {
            this.dealerWeight = -1;
            this.playerStateListList = new List<List<int>>();
            this.playerWeightListList = new List<List<int>>();
            this.playerWinBeenListList = new List<List<int>>();
            this.userDtoList = new List<UserDto>();
            this.isLeaveList = new List<bool>();
            for(int i=0;i<4;i++)
            {
                isLeaveList.Add(false);
                playerStateListList.Add(null);
                playerWeightListList.Add(null);
                playerWinBeenListList.Add(null);
                userDtoList.Add(null);
            }
        }
    }
}
