using AhpilyServer.Concurrent;
using Protocol.Dto.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache._21single
{
    /// <summary>
    /// 战斗的缓存
    /// </summary>
    public class _21SingleCache
    {
        /// <summary>
        /// 用户id  对应的  房间id
        /// </summary>
        private Dictionary<int, int> uidRoomIDict = new Dictionary<int, int>();
        /// <summary>
        /// 房间id   对应的  房间模型对象
        /// </summary>
        private Dictionary<int, SingleRoom> idRoomDict = new Dictionary<int, SingleRoom>();
        /// <summary>
        /// 重用房间队列
        /// </summary>
        private Queue<SingleRoom> roomQueue = new Queue<SingleRoom>();

        /// <summary>
        /// 房间的id
        /// </summary>
        private ConcurrentInt id = new ConcurrentInt(-1);

        /// <summary>
        /// 创建战斗房间
        /// </summary>
        /// <returns></returns>
        public SingleRoom Create(int uid)
        {
            SingleRoom room = null;
            //先检测有没有可重用的房间
            if (roomQueue.Count > 0)
            {
                room = roomQueue.Dequeue();
                //fixbug923
                room.Init(uid);
            }
            else//没有就直接创建
                room = new SingleRoom(id.Add_Get(), uid);

            //绑定映射关系
            uidRoomIDict.Add(uid, room.Id);
            idRoomDict.Add(room.Id, room);
            return room;
        }

        /// <summary>
        /// 获取房间
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SingleRoom GetRoom(int id)
        {
            if (idRoomDict.ContainsKey(id) == false)
            {
                // return null;
                throw new Exception("不存在这个房间");
            }
            SingleRoom room = idRoomDict[id];
            return room;
        }

        public bool IsFighting(int userId)
        {
            return uidRoomIDict.ContainsKey(userId);
        }

        /// <summary>
        /// 根据用户id获取所在的房间
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public SingleRoom GetRoomByUId(int uid)
        {
            if (uidRoomIDict.ContainsKey(uid) == false)
            {
                throw new Exception("当前用户不在房间");
            }
            int roomId = uidRoomIDict[uid];
            SingleRoom room = GetRoom(roomId);
            return room;
        }

        /// <summary>
        /// 摧毁房间
        /// </summary>
        /// <param name="room"></param>
        public void Destroy(SingleRoom room)
        {
            //移除映射关系
            idRoomDict.Remove(room.Id);

            uidRoomIDict.Remove(room.player.UserId);
            
            //初始化房间数据
            room.player=null;
            room.DealerCardList.Clear();
            room.libraryModel.Init();
            room.Multiple = 1;
            room.Wager = 10;
            //添加到重用队列里面等待重用
            roomQueue.Enqueue(room);
        }

    }
}
