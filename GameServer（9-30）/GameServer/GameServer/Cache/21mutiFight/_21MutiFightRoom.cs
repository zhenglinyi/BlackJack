using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AhpilyServer;
using Protocol.Constant;
using Protocol.Dto.Multi;

namespace GameServer.Cache._21mutiFight
{
    public class _21MutiFightRoom
    {
        /// <summary>
        /// 房间唯一标识码
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// 正在hsd的人
        /// </summary>
        public int playingUserId { get; set; }

        /// <summary>
        /// 在房间内用户id的列表 和 连接对象的 映射关系
        /// </summary>
        public Dictionary<int, ClientPeer> UIdClientDict { get; private set; }

        /// <summary>
        /// 存储所有玩家  0 1 2 3
        /// </summary>
        public List<MultiPlayerDto> PlayerList { get; set; }

        public Dictionary<int,int> UIdPositionDict { get; set; }
        /// <summary>
        /// 中途退出的玩家id列表
        /// </summary>
        public List<int> LeaveUIdList { get; set; }

        /// <summary>
        /// 牌库
        /// </summary>
        public MultiLibraryModel libraryModel { get; set; }

        /// <summary>
        /// 准备的玩家id列表
        /// </summary>
        public List<int> ReadyUIdList;

        

        /// <summary>
        /// 庄家的牌
        /// </summary>
        public List<CardDto> DealerCardList { get; set; }
        public long countDownTime;

        public bool isStart;

        public _21MutiFightRoom(int id)
        {
            this.Id = id;
            this.playingUserId = -1;
            this.PlayerList = new List<MultiPlayerDto>();
            for (int i = 0; i < 4; i++)
                PlayerList.Add(null);
            this.LeaveUIdList = new List<int>();
            this.libraryModel = new MultiLibraryModel();
            this.UIdPositionDict = new Dictionary<int, int>();
            this.UIdClientDict = new Dictionary<int, ClientPeer>();
            this.ReadyUIdList = new List<int>();
            this.DealerCardList = new List<CardDto>();
            this.isStart = false;
            this.countDownTime = 0;

        }

        public void resetRoom()
        {
            this.playingUserId = -1;
            this.LeaveUIdList.Clear();
            this.ReadyUIdList.Clear();
            this.DealerCardList.Clear();
            //重新洗牌
            if(this.libraryModel.RemainCardNum()<30)
            {
                this.libraryModel.Init();
            }
            //玩家数据重置
            for(int i=0;i<4;i++)
            {
                if(PlayerList[i]!=null)
                {
                    PlayerList[i].ResetPlayer();
                }

            }
        }

        /// <summary>
        /// 房间是否满了 1 2 3 4
        /// </summary>
        /// <returns>true代表满了 false代表还有位置</returns>
        public bool IsFull()
        {
            return UIdPositionDict.Count == 4;
            /*
            for (int i = 0; i < 4; i++)
                if (PlayerList[i] == null)
                    return false;
            return true;
            */
            
        }

        /// <summary>
        /// 房间是否空了
        /// </summary>
        /// <returns>true代表空了 false代表还有人</returns>
        public bool IsEmpty()
        {
            return UIdPositionDict.Count == 0;
            /*
            for (int i = 0; i < 4; i++)
                if (PlayerList[i] != null)
                    return false;
            return true;
            */
        }

        /// <summary>
        /// 是否所有人都准备了
        /// </summary>
        /// <returns></returns>
        public bool IsAllReady()
        {
            for (int i = 0; i < 4; i++)
                if (PlayerList[i]!=null&&PlayerList[i].isReady == false)
                    return false;
            return true;
        }

        /// <summary>
        /// 进入房间
        /// </summary>
        /// <param name="userId">用户id</param>
        public void Enter(int userId,ClientPeer client)
        {
            UIdClientDict.Add(userId, client);
            MultiPlayerDto mpdto = new MultiPlayerDto(userId);
            for(int i=0;i<4;i++)
            {
                if(PlayerList[i]==null)
                {
                    mpdto.position = i+1;
                    PlayerList[i] = mpdto;
                    UIdPositionDict.Add(userId, i + 1);
                    
                    break;
                }
                    
            }
            
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param name="userId"></param>
        public void Leave(int userId)
        {
            int i = UIdPositionDict[userId] - 1;
            PlayerList[i] = null;
            UIdPositionDict.Remove(userId);
            UIdClientDict.Remove(userId);
            if(ReadyUIdList.Contains(userId))
            { 
                ReadyUIdList.Remove(userId);
            }
               
            /*
            for (int i = 0; i < 4; i++)
            {
                if (PlayerList[i] != null&& PlayerList[i].UserId==userId)
                {
                    PlayerList[i] = null;
                    break;
                }
            }*/
        }

        /// <summary>
        /// 玩家准备
        /// </summary>
        /// <param name="userId"></param>
        public void Ready(int userId)
        {
            int i = UIdPositionDict[userId] - 1;
            PlayerList[i].isReady = true;
            ReadyUIdList.Add(userId);
        }

        /// <summary>
        /// 广播房间内的所有玩家信息
        /// </summary>
        /// <param name="opCode"></param>
        /// <param name="subCode"></param>
        /// <param name="value"></param>
        public void Brocast(int opCode, int subCode, object value, ClientPeer exClient = null)
        {
            SocketMsg msg = new SocketMsg(opCode, subCode, value);
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);

            foreach (var client in UIdClientDict.Values)
            {
                if (client == exClient)
                    continue;

                client.Send(packet);
            }
        }

        public int getPosition(int userId)
        {
            return UIdPositionDict[userId];
        }

        public void setWager(int userId,int wager)
        {
            int position = getPosition(userId);
            PlayerList[position - 1].Wager = wager;
        }

        /// <summary>
        /// 发牌 (初始化角色手牌)
        /// </summary>
        public InitCardDto InitPlayerCards()
        {
            InitCardDto initCardDto = new InitCardDto();
            //庄家
            for (int i = 0; i < 2; i++)
            {
                CardDto card = libraryModel.Deal();
                DealerCardList.Add(card);
            }
            initCardDto.InitCardLists[0] = DealerCardList;
            for (int i = 0; i < 4; i++)
            {
                if (PlayerList[i] != null)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        CardDto card = libraryModel.Deal();
                        PlayerList[i].CardList.Add(card);
                    }
                    initCardDto.InitCardLists[i+1] = PlayerList[i].CardList;
                }
            }
            return initCardDto;

        }
        /// <summary>
        /// 1234循环第一个用户
        /// </summary>
        /// <returns></returns>
        public int GetFirstUId()
        {
            int firstuid=-1;
            for (int i = 0; i < 4; i++)
            {
                if (PlayerList[i] != null)
                {
                    firstuid= PlayerList[i].UserId;
                    break;
                }
            }
            return firstuid;
        }

        public int GetNextUId(int lastUid)
        {
            int nextuid = -1;
            int lastPosition = getPosition(lastUid);
            for(int i= lastPosition;i<4;i++)
            {
                //1不为空 2不能离开了
                if (PlayerList[i] != null&& !LeaveUIdList.Contains(PlayerList[i].UserId))
                {
                    nextuid = PlayerList[i].UserId;
                    break;
                }
            }
            
            return nextuid;
        }

        public bool is21ByUserId(int userId)
        {
            int position = getPosition(userId);
            return MultiCardWeight21.GetWeight(PlayerList[position-1].CardList) == 21;
        }

        public bool isCanSplitByUserId(int userId)
        {
            int position = getPosition(userId);
            List<CardDto> tempList = PlayerList[position - 1].CardList;
            return tempList[0].Weight == tempList[1].Weight;

        }

        /// <summary>
        /// 保存一下卡牌数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <param name="isDouble"></param>
        public void SaveListByUserId(int userId,int state,bool isDouble)
        {
            int position = getPosition(userId);
            PlayerCardDto playerCardDto = new PlayerCardDto();
            playerCardDto.CardList.AddRange(PlayerList[position - 1].CardList);
            playerCardDto.CardState = state;
            playerCardDto.Weight = MultiCardWeight21.GetWeight(PlayerList[position - 1].CardList);
            playerCardDto.isDouble = isDouble;
            PlayerList[position - 1].CardListList.Add(playerCardDto);

        }

        /// <summary>
        /// 分牌
        /// </summary>
        /// <param name="userId"></param>
        public void SplitByUserId(int userId)
        {
            int position = getPosition(userId);

            List<CardDto> tempList = new List<CardDto>();
            tempList.Add(PlayerList[position - 1].CardList[1]);
            //删除第一个
            PlayerList[position - 1].CardList.RemoveAt(1);
            //第一组重新发一张牌
            CardDto card = libraryModel.Deal();
            PlayerList[position - 1].CardList.Add(card);
            //第二组发一张牌
            card = libraryModel.Deal();
            tempList.Add(card);
            //第二组加到队列备用
            PlayerList[position - 1].SpliteCardListQueue.Enqueue(tempList);

        }

        /// <summary>
        /// 获取玩家的现有手牌
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<CardDto> GetUserCardsByUserId(int userId)
        {
            int position = getPosition(userId);
            return PlayerList[position-1].CardList;
            
        }

        /// <summary>
        /// 是否分过牌且还有牌
        /// </summary>
        /// <param name="uid"></param>
        public bool isHaveNextHand(int uid)
        {
            int position = getPosition(uid);
            return PlayerList[position - 1].SpliteCardListQueue.Count > 0;
        }
        /// <summary>
        /// 获得下一手牌
        /// </summary>
        /// <param name="uid"></param>
        public void NextHandCardByUId(int uid)
        {
            int position = getPosition(uid);
            PlayerList[position - 1].CardList.Clear();
            PlayerList[position - 1].CardList = PlayerList[position - 1].SpliteCardListQueue.Dequeue();

        }
        /// <summary>
        /// 要牌
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public CardDto HitByUId(int uid)
        {
            int position = getPosition(uid);
            CardDto card = libraryModel.Deal();
            PlayerList[position - 1].CardList.Add(card);
            return card;
        }

        /// <summary>
        /// 获取牌组的总权值
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetWeightByUId(int userId)
        {
            int position = getPosition(userId);

            return MultiCardWeight21.GetWeight(PlayerList[position-1].CardList);
        }
        /// <summary>
        /// 设置正在hsd的userId
        /// </summary>
        /// <param name="userId"></param>
        public void SetPlayingUId(int userId)
        {
            playingUserId = userId;
        }
        /// <summary>
        /// 获取庄家的权值
        /// </summary>
        /// <returns></returns>
        public int GetDealerWeight()
        {
            return MultiCardWeight21.GetWeight(DealerCardList);
        }
        /// <summary>
        /// 庄家要牌
        /// </summary>
        /// <returns></returns>
        public CardDto DealerHit()
        { 
            CardDto card = libraryModel.Deal();
            DealerCardList.Add(card);
            return card;
        }

        



    }
}
