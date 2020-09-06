using AhpilyServer;
using AhpilyServer.Concurrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache._21mutiFight
{
    public class _21MutiFightCache
    {
        /// <summary>
        /// 代表  用户id 和 房间id  的映射
        /// </summary>
        private Dictionary<int, int> uidRoomIdDict = new Dictionary<int, int>();

        /// <summary>
        /// 代表  房间id  和 房间的数据模型 的映射 已经开始游戏了
        /// </summary>
        private Dictionary<int, _21MutiFightRoom> idModelPlayingDict = new Dictionary<int, _21MutiFightRoom>();

        /// <summary>
        /// 代表  房间id  和 房间的数据模型 的映射 准备阶段
        /// </summary>
        private Dictionary<int, _21MutiFightRoom> idModelWaitingDict = new Dictionary<int, _21MutiFightRoom>();



        /// <summary>
        /// 代表 重用的房间 队列
        /// </summary>
        private Queue<_21MutiFightRoom> roomQueue = new Queue<_21MutiFightRoom>();

        /// <summary>
        /// 代表 房间的id
        /// </summary>
        private ConcurrentInt id = new ConcurrentInt(-1);

        /// <summary>
        /// 判断用户是否在房间内
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsInRoom(int userId)
        {
            return uidRoomIdDict.ContainsKey(userId);
        }

        /// <summary>
        /// 进入匹配队列 进入匹配房间
        /// </summary>
        /// <returns></returns>
        public _21MutiFightRoom Match(int userId, ClientPeer client)
        {
            //遍历一下等待的房间 看一下 有没有正在等待的 如果有 我们就把这玩家加进去
            foreach (_21MutiFightRoom mr in idModelWaitingDict.Values)
            {
                //房间满了 继续
                if (mr.IsFull())
                    continue;
                //没满的话
                mr.Enter(userId, client);
                uidRoomIdDict.Add(userId, mr.Id);
                return mr;
            }
            //如果调用到这里 代表 没进去  为什么？因为没有等待的房间了
            //自己开个房
            _21MutiFightRoom room = null;
            //判断一下是否有重用的房间
            if (roomQueue.Count > 0)
                room = roomQueue.Dequeue();
            else
                room = new _21MutiFightRoom(id.Add_Get());

            room.Enter(userId, client);
            idModelWaitingDict.Add(room.Id, room);
            uidRoomIdDict.Add(userId, room.Id);
            return room;
        }

        /// <summary>
        /// 获取玩家所在的房间
        /// </summary>
        /// <returns></returns>
        public _21MutiFightRoom GetRoom(int userId)
        {
            int roomId = uidRoomIdDict[userId];

            _21MutiFightRoom room;
            if (idModelWaitingDict.ContainsKey(roomId))
                room = idModelWaitingDict[roomId];
            else
                room = idModelPlayingDict[roomId];
            return room;
        }

        public _21MutiFightRoom GetRoomByRoomId(int roomId)
        {
            _21MutiFightRoom room;
            if (idModelWaitingDict.ContainsKey(roomId))
                room = idModelWaitingDict[roomId];
            else
                room = idModelPlayingDict[roomId];
            return room;
        }


        /// <summary>
        /// 获取玩家所在的房间
        /// </summary>
        /// <returns></returns>
        public int GetRoomId(int userId)
        {
            int roomId = uidRoomIdDict[userId];
            
            return roomId;
        }

        public bool IsInPlaying(int userId)
        {
            int roomId = GetRoomId(userId);
            return idModelPlayingDict.ContainsKey(roomId);
        }

        /// <summary>
        /// 离开匹配房间  等待的时候离开
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public _21MutiFightRoom Leave(int userId)
        {

            int roomId = uidRoomIdDict[userId];
            _21MutiFightRoom room = idModelWaitingDict[roomId];

            //_21MutiFightRoom room;
            //if (idModelWaitingDict.ContainsKey(roomId))
            //    room = idModelWaitingDict[roomId];
            //else
            //    room = idModelPlayingDict[roomId];
            room.Leave(userId);
            //还需要进一步的处理
            uidRoomIdDict.Remove(userId);
            if (room.IsEmpty())
            {
                //如果房间空了 那就放到重用队列里面
                if (idModelWaitingDict.ContainsKey(roomId))
                    idModelWaitingDict.Remove(roomId);
                else
                    idModelPlayingDict.Remove(roomId);
                roomQueue.Enqueue(room);
            }
            return room;
        }

        /// <summary>
        /// 根据用户id获取所在的房间
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public _21MutiFightRoom GetRoomByUId(int uid)
        {
            if (uidRoomIdDict.ContainsKey(uid) == false)
            {
                throw new Exception("当前用户不在房间");
            }
            int roomId = uidRoomIdDict[uid];
            _21MutiFightRoom room = GetRoom(uid);
            return room;
        }
        /// <summary>
        /// 设置房间为战斗房间，把房间加入战斗队列
        /// </summary>
        /// <param name="userId"></param>
        public void SetRoomPlay(int userId)
        {
            int roomId = GetRoomId(userId);
            _21MutiFightRoom room = GetRoom(userId);
            idModelWaitingDict.Remove(roomId);
            idModelPlayingDict.Add(roomId, room);


        }
        /// <summary>
        /// 通过房间Id获取房间对象
        /// </summary>
        /// <param name="roomId"></param>
        public void SetRoomPlayByRoomId(int roomId)
        {
            _21MutiFightRoom room = idModelWaitingDict[roomId];
            idModelWaitingDict.Remove(roomId);
            idModelPlayingDict.Add(roomId, room);
        }

        /// <summary>
        /// 设置房间为等待房间
        /// </summary>
        /// <param name="room"></param>
        public void SetRoomWait(_21MutiFightRoom room)
        {
            int roomId = room.Id;

            
            idModelPlayingDict.Remove(roomId);
            idModelWaitingDict.Add(roomId, room);


        }

        public List<int> GetAllWaitingRoomId()
        {
            return idModelWaitingDict.Keys.ToList<int>();
        }

         
    }
}
