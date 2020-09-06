using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Dto.Multi
{
    /// <summary>
    /// 房间数据对应的传输模型
    /// </summary>
    [Serializable]
    public class MutiRoomDto
    {
        /// <summary>
        /// 准备的玩家id列表
        /// </summary>
        public List<int> ReadyUIdList;

        public Dictionary<int, int> UIdPositionDict { get; set; }
        /// <summary>
        /// 中途退出的玩家id列表
        /// </summary>
        public List<int> LeaveUIdList { get; set; }

        /// <summary>
        /// 用户id对应的用户数据的传输模型
        /// </summary>
        public Dictionary<int, UserDto> UIdUserDict;

        public MutiRoomDto()
        {
            this.ReadyUIdList = new List<int>();
            this.UIdPositionDict = new Dictionary<int, int>();
            this.LeaveUIdList = new List<int>();
            this.UIdUserDict = new Dictionary<int, UserDto>();
        }

        public void Add(UserDto newUser,int position)
        {
            this.UIdUserDict.Add(newUser.Id, newUser);
            UIdPositionDict.Add(newUser.Id, position);
            
        }

        public void Leave(int userId)
        {
            this.UIdUserDict.Remove(userId);
            this.ReadyUIdList.Remove(userId);
            this.UIdPositionDict.Remove(userId);
            
        }

        public void Ready(int userId)
        {
            this.ReadyUIdList.Add(userId);
        }

    }
}
