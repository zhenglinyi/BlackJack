using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Cache._21mutiFight;
using GameServer.Cache;
using Protocol.Code;
using AhpilyServer;
using System.Text.RegularExpressions;
using GameServer.Model;
using Protocol.Dto;
using Protocol.Dto.Multi;
using System.Threading;
using Protocol.Constant;

namespace GameServer.Logic
{

    public class _21MultiHandler
    {
        public _21MutiFightCache fightCache = Caches.Multi;
        public UserCache userCache = Caches.User;
        System.Timers.Timer timer;

        public _21MultiHandler()
        {
            timer = new System.Timers.Timer(100);
            timer.Enabled = true;
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            SingleExecute.Instance.Execute(
                delegate ()
                {
                    foreach (int roomId in fightCache.GetAllWaitingRoomId())
                    {
                        _21MutiFightRoom room = fightCache.GetRoomByRoomId(roomId);
                        if (room.IsAllReady())
                        {
                            if(room.isStart)
                            {
                                if(room.countDownTime==0)
                                {
                                    room.Brocast(OpCode._21Multi, _21MultiCode.START_BRO, null);
                                    StartFight(roomId);
                                }
                                room.countDownTime -= 100;
                            }
                            else
                            {
                                room.isStart = true;
                                room.countDownTime = 4000;
                                brocast(room, OpCode._21Multi, _21MultiCode.COUNT_DOWN_BRO, null, null);
                            }
                            
                        }
                        else
                        {
                            if (room.isStart)
                            {
                                brocast(room, OpCode._21Multi, _21MultiCode.STOP_COUNT_DOWN_BRO, null, null);
                            }
                            
                            room.isStart = false;
                            room.countDownTime = 0;
                        }
                    }
                });
            
            
        }

        public void OnDisconnect(ClientPeer client)
        {
            leave(client);
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case _21MultiCode.MATCH_CREQ:
                    match(client);
                    break;

                case _21MultiCode.ENTER_CREQ:
                    enter(client);
                    break;
                case _21MultiCode.READY_CREQ:
                    ready(client);
                    break;
                case _21MultiCode.LEAVE_CREQ:
                    leave(client);
                    break;
                case _21MultiCode.SET_WAGER_CREQ:
                    setWager(client, (int)value);
                    break;
                case _21MultiCode.HIT_CREQ:
                    hit(client);
                    break;
                case _21MultiCode.SPLIT_CREQ:
                    split(client);
                    break;
                case _21MultiCode.SPLIT_NEXT_CREQ:
                    splitNext(client);
                    break;
                case _21MultiCode.STAND_CREQ:
                    stand(client);
                    break;
                case _21MultiCode.DOUBLE_CREQ:
                    doubleCard(client);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="client"></param>
        public void match(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    if (!userCache.IsOnline(client))
                        return;
                    int userId = userCache.GetId(client);

                    //if (userId != id)
                    //    return;

                    //如果用户已经在匹配房间等待了 那就无视
                    if (fightCache.IsInRoom(userId))
                    {
                        client.Send(OpCode._21Multi, _21MultiCode.MATCH_SRES, -1);//重复匹配
                        return;
                    }
                    //正常匹配
                    _21MutiFightRoom room = fightCache.Match(userId, client);
                    
                    //广播给房间内除了当前客户端的其他用户，有新玩家计入了  参数：新进入的玩家的用户id
                    #region 构造一个UserDto  Dto就是针对UI定义的 UI需要什么我们就加什么字段
                    UserModel model = userCache.GetModelById(userId);
                    UserDto userDto = new UserDto(model.Id, model.Name, model.Been, model.WinCount, model.LoseCount, model.RunCount, model.Lv, model.Exp);
                    int position = room.getPosition(userId);

                    MultiEnterDto edto = new MultiEnterDto(userDto, position);
                    #endregion
                    room.Brocast(OpCode._21Multi, _21MultiCode.ENTER_BRO, edto, client);


                    MutiRoomDto dto = makeRoomDto(room);
                    client.Send(OpCode._21Multi, _21MultiCode.MATCH_SRES, dto);
                    Console.WriteLine("有玩家进行了匹配");
                }
                );
        }

        public void enter(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    if (!userCache.IsOnline(client))
                        return;
                    int userId = userCache.GetId(client);
                    //正常匹配
                    _21MutiFightRoom room = fightCache.GetRoom(userId);
                    
                    /*//广播给房间内除了当前客户端的其他用户，有新玩家计入了  参数：新进入的玩家的用户id
                    #region 构造一个UserDto  Dto就是针对UI定义的 UI需要什么我们就加什么字段
                    UserModel model = userCache.GetModelById(userId);
                    UserDto userDto = new UserDto(model.Id, model.Name, model.Been, model.WinCount, model.LoseCount, model.RunCount, model.Lv, model.Exp);
                    #endregion
                    room.Brocast(OpCode._21Multi, _21MultiCode.ENTER_BRO, userDto, client);
                    */
                    //返回给当前客户端 给他房间的数据模型
                    MutiRoomDto dto = makeRoomDto(room);
                    client.Send(OpCode._21Multi, _21MultiCode.ENTER_SRES, dto);
                    
                    
                    Console.WriteLine("有玩家进入了房间");
                }
                );
        }

        /// <summary>
        /// 准备
        /// </summary>
        /// <param name="client"></param>
        private void ready(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
                () =>
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    int userId = userCache.GetId(client);
                    if (fightCache.IsInRoom(userId) == false)
                        return;
                    //一定要注意安全的验证
                    _21MutiFightRoom room = fightCache.GetRoom(userId);
                    room.Ready(userId);
                    //之前忘记了 &&&……%……&￥&￥&%#%#&￥&……%&
                    room.Brocast(OpCode._21Multi, _21MultiCode.READY_BRO, userId);

                    //检测：是否所有玩家都准备了
                    //if (room.IsAllReady())
                    //{
                        
                    //    //通知房间内的玩家  要进行战斗了 给客户端群发消息
                    //    room.Brocast(OpCode._21Multi, _21MultiCode.START_BRO, null);
                    //    Console.WriteLine("要开始战斗了");
                    //    Thread.Sleep(1000);
                    //    //开始战斗 
                    //    StartFight(userId, client, room);

                    //}
                }
                );
        }
        private MutiRoomDto makeRoomDto(_21MutiFightRoom room)
        {
            MutiRoomDto dto = new MutiRoomDto();
            //给 UIdClientDict 赋值
            foreach (var uid in room.UIdClientDict.Keys)
            {
                UserModel model = userCache.GetModelById(uid);
                UserDto userDto = new UserDto(model.Id, model.Name, model.Been, model.WinCount, model.LoseCount, model.RunCount, model.Lv, model.Exp);
                dto.UIdUserDict.Add(uid, userDto);

            }

            dto.UIdPositionDict = room.UIdPositionDict;
            dto.ReadyUIdList = room.ReadyUIdList;
            return dto;
        }

        /// <summary>
        /// 离开    
        /// </summary>
        /// <param name="client"></param>
        private void leave(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    if (!userCache.IsOnline(client))
                        return;
                    int userId = userCache.GetId(client);
                    //用户没有匹配 不能退出 非法操作
                    if (fightCache.IsInRoom(userId) == false)
                    {
                        //client.Send(OpCode.MATCH, MatchCode.LEAVE_SRES, -1);
                        return;
                    }
                    //在战斗的时候离开
                    if (fightCache.IsInPlaying(userId))
                    {
                        _21MutiFightRoom room = fightCache.GetRoom(userId);
                        //加入到离开链表里
                        room.LeaveUIdList.Add(userId);
                        //广播一下离开的用户
                        room.Brocast(OpCode._21Multi, _21MultiCode.LEAVE_BRO, userId);
                        //如果在处理的时候离开了  换下一个
                        if (userId==room.playingUserId)
                        {
                            int nextUid = room.GetNextUId(userId);
                            room.SetPlayingUId(nextUid);
                            //没有下一个玩家了
                            if (nextUid == -1)
                            {
                                //room.SetPlayingUId(-1);
                                //todo 庄家的操作
                                gameOver(room);
                            }
                            else
                            {

                                //和刚开始差不多
                                if (room.is21ByUserId(nextUid))//21点 直接赢了
                                {
                                    //记录一下这组牌
                                    room.SaveListByUserId(nextUid, 1, false);
                                    int position = room.getPosition(nextUid);
                                    OverHandDto ohdto = new OverHandDto(nextUid, position, 1);
                                    brocast(room, OpCode._21Multi, _21MultiCode.OVER_HAND_BRO, ohdto, null);
                                    //该下一个玩家了
                                    turnPlayer(nextUid, position, room);
                                }
                                if (room.isCanSplitByUserId(nextUid))//能够分牌
                                {
                                    TurnHandDto thdto = new TurnHandDto(true, nextUid);
                                    //client.Send(OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto);
                                    brocast(room, OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto, null);
                                }
                                else
                                {
                                    TurnHandDto thdto = new TurnHandDto(false, nextUid);
                                    //client.Send(OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto);
                                    brocast(room, OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto, null);

                                }
                            }
                        }
                        

                    }
                    //未战斗的时候离开
                    else
                    {
                        //正常离开
                        _21MutiFightRoom room = fightCache.Leave(userId);
                        //广播给房间内所有人 有人离开了 参数：离开的用户id
                        room.Brocast(OpCode._21Multi, _21MultiCode.LEAVE_BRO, userId);

                    }
                    

                    Console.WriteLine("有玩家离开房间");
                });
        }

        /// <summary>
        /// 设置赌金
        /// </summary>
        /// <param name="client"></param>
        /// <param name="wager"></param>
        private void setWager(ClientPeer client, int wager)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    //必须确保在线
                    int userId = userCache.GetId(client);
                    //获取战斗房间
                    _21MutiFightRoom room = fightCache.GetRoomByUId(userId);
                    room.setWager(userId,wager);

                });
        }

        /// <summary>
        /// 开始战斗
        /// </summary>
        private void StartFight(int userId, ClientPeer client, _21MutiFightRoom room)
        {
            //房间设置为开始
            fightCache.SetRoomPlay(userId);
            //初始发牌
            InitCardDto initCardDto=room.InitPlayerCards();
            brocast(room, OpCode._21Multi, _21MultiCode.INIT_CARD_BRO, initCardDto, null);

            //发送开始HIT或STAND的响应
            int firstUserId = room.GetFirstUId();
            room.SetPlayingUId(firstUserId);
            if (room.is21ByUserId(firstUserId))//21点 直接赢了
            {
                //记录一下这组牌
                room.SaveListByUserId(firstUserId, 1,false);
                int position = room.getPosition(firstUserId);
                OverHandDto ohdto = new OverHandDto(firstUserId, position, 1);
                brocast(room, OpCode._21Multi, _21MultiCode.OVER_HAND_BRO, ohdto, null);
                //该下一个玩家了
                turnPlayer(firstUserId, position,room);
            }
            else
            {
                if (room.isCanSplitByUserId(firstUserId))//能够分牌
                {
                    TurnHandDto thdto = new TurnHandDto(true, firstUserId);
                    //client.Send(OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto);
                    brocast(room, OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto, null);
                }
                else
                {
                    TurnHandDto thdto = new TurnHandDto(false, firstUserId);
                    //client.Send(OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto);
                    brocast(room, OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto, null);

                }
            }
            
            

        }

        private void StartFight(int roomId)
        {
            //房间设置为开始
            fightCache.SetRoomPlayByRoomId(roomId);
            _21MutiFightRoom room = fightCache.GetRoomByRoomId(roomId);
            //初始发牌
            InitCardDto initCardDto = room.InitPlayerCards();
            brocast(room, OpCode._21Multi, _21MultiCode.INIT_CARD_BRO, initCardDto, null);

            //发送开始HIT或STAND的响应
            int firstUserId = room.GetFirstUId();
            room.SetPlayingUId(firstUserId);
            if (room.is21ByUserId(firstUserId))//21点 直接赢了
            {
                //记录一下这组牌
                room.SaveListByUserId(firstUserId, 1, false);
                int position = room.getPosition(firstUserId);
                OverHandDto ohdto = new OverHandDto(firstUserId, position, 1);
                brocast(room, OpCode._21Multi, _21MultiCode.OVER_HAND_BRO, ohdto, null);
                //该下一个玩家了
                turnPlayer(firstUserId, position, room);
            }
            else
            {
                if (room.isCanSplitByUserId(firstUserId))//能够分牌
                {
                    TurnHandDto thdto = new TurnHandDto(true, firstUserId);
                    //client.Send(OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto);
                    brocast(room, OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto, null);
                }
                else
                {
                    TurnHandDto thdto = new TurnHandDto(false, firstUserId);
                    //client.Send(OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto);
                    brocast(room, OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto, null);

                }
            }



        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastUserId"></param>
        /// <param name="lastPosition"></param>
        /// <param name="room"></param>
        private void turnPlayer(int lastUserId,int lastPosition,_21MutiFightRoom room)
        {
            ClientPeer client = userCache.GetClientPeer(lastUserId);
            //是不是分过牌 并且还有牌 有牌还是给他发牌
            if (room.isHaveNextHand(lastUserId))
            {
                
                client.Send(OpCode._21Multi, _21MultiCode.SPLIT_CAN_NEXT_SRES, null);
            }
            //没有了的话 就下一个玩家开始操作
            else
            {
                int nextUid = room.GetNextUId(lastUserId);
                room.SetPlayingUId(nextUid);
                //没有下一个玩家了
                if (nextUid == -1)
                {
                    //room.SetPlayingUId(-1);
                    //todo 庄家的操作
                    gameOver(room);
                }
                else
                {
                    
                    //和刚开始差不多
                    if (room.is21ByUserId(nextUid))//21点 直接赢了
                    {
                        //记录一下这组牌
                        room.SaveListByUserId(nextUid, 1, false);
                        int position = room.getPosition(nextUid);
                        OverHandDto ohdto = new OverHandDto(nextUid, position, 1);
                        brocast(room, OpCode._21Multi, _21MultiCode.OVER_HAND_BRO, ohdto, null);
                        //该下一个玩家了
                        turnPlayer(nextUid, position, room);
                    }
                    else if (room.isCanSplitByUserId(nextUid))//能够分牌
                    {
                        TurnHandDto thdto = new TurnHandDto(true, nextUid);
                        //client.Send(OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto);
                        brocast(room, OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto, null);
                    }
                    else
                    {
                        TurnHandDto thdto = new TurnHandDto(false, nextUid);
                        //client.Send(OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto);
                        brocast(room, OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto, null);

                    }
                }

            }
            




        }

        private void gameOver(_21MutiFightRoom room)
        {
            //给庄家发牌
            brocast(room, OpCode._21Multi, _21MultiCode.TURN_DEALER_BRO, null, null);
            //把所有的state遍历一遍，都爆牌了就不用要了
            while (room.GetDealerWeight() < 17)
            {
                CardDto card = room.DealerHit();
                brocast(room, OpCode._21Multi, _21MultiCode.DEALER_HIT_BRO, card, null);

            }

            //结算
            //结算对象
            GameOverDto gameOverDto = new GameOverDto();
            //todo 写一个总结算的对象，把玩家信息和这轮游戏的信息都发过去 状态 1 2 3 4 5
            //庄家的权值
            int dealerWeight = room.GetDealerWeight();
            gameOverDto.dealerWeight = dealerWeight;
            for (int i = 0; i < 4; i++)
            {
                //不为空
                if (room.PlayerList[i] != null)
                {
                    List<int> StateList = new List<int>();
                    List<int> WeightList = new List<int>();
                    List<int> WinBeenList = new List<int>();

                    UserModel um = userCache.GetModelById(room.PlayerList[i].UserId);
                    //ClientPeer client = userCache.GetClientPeer(room.PlayerList[i].UserId);
                    int wager = room.PlayerList[i].Wager;
                    
                    //一名掉线玩家
                    if (room.LeaveUIdList.Contains(room.PlayerList[i].UserId))
                    {
                        //扣双倍豆
                        um.Been -= wager * 2;
                        gameOverDto.isLeaveList[i] = true;
                        WinBeenList.Add(-wager * 2);
                        gameOverDto.playerWinBeenListList[i] = WinBeenList;

                    }
                    //正常玩家
                    else
                    {
                        //每一把牌经验豆子结算
                        for(int j=0;j<room.PlayerList[i].CardListList.Count;j++)
                        {
                            int multi = 1;

                            PlayerCardDto tempPlayerCardDto = room.PlayerList[i].CardListList[j];
                            int tempWeight = tempPlayerCardDto.Weight;
                            WeightList.Add(tempWeight);
                            int tempState = tempPlayerCardDto.CardState;
                            bool tempIsDouble = tempPlayerCardDto.isDouble;
                            if (tempIsDouble)
                            {
                                multi = 2;

                            }
                                

                            //21点
                            if(tempState==1)
                            {
                                um.Been += (int)(wager * 1.5);
                                WinBeenList.Add((int)(wager * 1.5));
                                StateList.Add(1);
                                um.Exp += 150;
                            }
                            //闲家爆牌
                            else if(tempState==2)
                            {
                                um.Been -= wager*multi;
                                WinBeenList.Add(-wager * multi);
                                if (multi == 1)
                                    StateList.Add(2);
                                else
                                    StateList.Add(3);
                                um.Exp += 50;

                            }
                            //不要 权值小于
                            else if(tempState==3)
                            {
                                //庄家爆牌
                                if(dealerWeight>21)
                                {
                                    um.Been += wager * multi;
                                    WinBeenList.Add(wager * multi);
                                    if (multi == 1)
                                        StateList.Add(6);
                                    else
                                        StateList.Add(4);

                                    um.Exp += 100;
                                }
                                //闲家点大
                                else if(dealerWeight < tempWeight)
                                {
                                    um.Been += wager * multi;
                                    WinBeenList.Add(wager * multi);
                                    if (multi == 1)
                                        StateList.Add(6);
                                    else
                                        StateList.Add(4);
                                    um.Exp += 100;
                                }
                                //庄家点大
                                else if(dealerWeight > tempWeight)
                                {
                                    um.Been -= wager * multi;
                                    WinBeenList.Add(-wager * multi);
                                    if (multi == 1)
                                        StateList.Add(7);
                                    else
                                        StateList.Add(5);
                                    um.Exp += 50;
                                }
                                //平局
                                else
                                {
                                    WinBeenList.Add(0);
                                    StateList.Add(8);
                                    
                                    um.Exp += 75;
                                }

                            }

                        }

                    }
                    gameOverDto.playerWeightListList[i] = WeightList;
                    gameOverDto.playerStateListList[i] = StateList;
                    gameOverDto.playerWinBeenListList[i] = WinBeenList;

                    int maxExp = um.Lv * 100;
                    while (maxExp <= um.Exp)
                    {
                        um.Lv++;
                        um.Exp -= maxExp;
                        maxExp = um.Lv * 100;
                    }
                    userCache.Update(um);
                    UserDto dto = new UserDto(um.Id, um.Name, um.Been, um.WinCount, um.LoseCount, um.RunCount, um.Lv, um.Exp);
                    gameOverDto.userDtoList[i] = dto;
                }

                
            }

            //掉线的 不用发了，改一下这个函数 主要用在overpanel
            brocast(room, OpCode._21Multi, _21MultiCode.OVER_BRO, gameOverDto, null);
            
            //房间设置为等待
            fightCache.SetRoomWait(room);
            //删除离开列表里的用户
            foreach(int uid in room.LeaveUIdList)
            {
                fightCache.Leave(uid);
            }
            //重置房间
            room.resetRoom();



        }

         

        /// <summary>
        /// 要牌
        /// </summary>
        /// <param name="client"></param>
        private void hit(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
               () =>
               {
                   if (userCache.IsOnline(client) == false)
                       return;
                   int userId = userCache.GetId(client);
                   if (fightCache.IsInRoom(userId) == false)
                       return;
                    //一定要注意安全的验证
                   _21MutiFightRoom room = fightCache.GetRoom(userId);
                   int position = room.getPosition(userId);
                   HitBroDto hbdto;

                   //如果牌已经21点不能要牌，最好客户端设置一下
                   if (room.is21ByUserId(userId))
                   {
                       //client.Send(OpCode._21Multi, _21MultiCode.HIT_BRO, null);
                       //发送个空的carddto
                       hbdto = new HitBroDto(userId, position,null);
                       brocast(room, OpCode._21Multi, _21MultiCode.HIT_BRO, hbdto, null);
                       return;
                   }
                   //把要的牌广播一下
                   CardDto carddto = room.HitByUId(userId);

                   hbdto = new HitBroDto(userId, position, carddto);
                   brocast(room, OpCode._21Multi, _21MultiCode.HIT_BRO, hbdto, null);
                   //爆牌了
                   if(room.GetWeightByUId(userId)>21)
                   {
                       //记录一下这组牌
                       room.SaveListByUserId(userId, 2, false);
                       //int position = room.getPosition(userId);
                       OverHandDto ohdto = new OverHandDto(userId, position, 2);
                       brocast(room, OpCode._21Multi, _21MultiCode.OVER_HAND_BRO, ohdto, null);
                       //该下一个玩家了
                       turnPlayer(userId, position, room);
                   }
               }
               );

        }

        /// <summary>
        /// 停牌
        /// </summary>
        /// <param name="client"></param>
        private void stand(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
              () =>
              {
                  if (userCache.IsOnline(client) == false)
                      return;
                  int userId = userCache.GetId(client);
                  if (fightCache.IsInRoom(userId) == false)
                      return;
                   //一定要注意安全的验证
                  _21MutiFightRoom room = fightCache.GetRoom(userId);
                  int position = room.getPosition(userId);
                  room.SaveListByUserId(userId, 3, false);
                  OverHandDto ohdto = new OverHandDto(userId, position, 3);
                  brocast(room, OpCode._21Multi, _21MultiCode.OVER_HAND_BRO, ohdto, null);
                  //该下一个玩家了
                  turnPlayer(userId, position, room);
              }
              );
        }
        /// <summary>
        /// 加倍
        /// </summary>
        /// <param name="client"></param>
        private void doubleCard(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
               () =>
               {
                   if (userCache.IsOnline(client) == false)
                       return;
                   int userId = userCache.GetId(client);
                   if (fightCache.IsInRoom(userId) == false)
                       return;
                   //一定要注意安全的验证
                   _21MutiFightRoom room = fightCache.GetRoom(userId);

                   
                   //把要的牌广播一下
                   CardDto carddto = room.HitByUId(userId);
                   int position = room.getPosition(userId);
                   HitBroDto hbdto = new HitBroDto(userId, position, carddto);
                   //可以用hit的code
                   brocast(room, OpCode._21Multi, _21MultiCode.HIT_BRO, hbdto, null);
                   
                   //爆牌了
                   if (room.GetWeightByUId(userId) > 21)
                   {
                       //记录一下这组牌 true代表加倍了
                       room.SaveListByUserId(userId, 2, true);
                       //4代表加倍后爆牌
                       OverHandDto ohdto = new OverHandDto(userId, position, 4);
                       brocast(room, OpCode._21Multi, _21MultiCode.OVER_HAND_BRO, ohdto, null);
                       //该下一个玩家了
                       turnPlayer(userId, position, room);
                   }
                   //相当于加倍后不要牌了
                   else
                   {
                       room.SaveListByUserId(userId, 3, true);
                       //5代表加倍
                       OverHandDto ohdto = new OverHandDto(userId, position, 5);
                       brocast(room, OpCode._21Multi, _21MultiCode.OVER_HAND_BRO, ohdto, null);
                       //该下一个玩家了
                       turnPlayer(userId, position, room);

                   }
               }
               );

        }
        /// <summary>
        /// 分牌
        /// </summary>
        /// <param name="client"></param>
        private void split(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
               () =>
               {
                   if (userCache.IsOnline(client) == false)
                       return;
                   int userId = userCache.GetId(client);
                   if (fightCache.IsInRoom(userId) == false)
                       return;
                   //一定要注意安全的验证
                   _21MutiFightRoom room = fightCache.GetRoom(userId);
                   //分牌
                   room.SplitByUserId(userId);
                   //获取分牌后的第一组手牌
                   List<CardDto> cardList = room.GetUserCardsByUserId(userId);
                   //广播发送一下手牌
                   SplitBroDto sbdto = new SplitBroDto();
                   sbdto.userId = userId;
                   sbdto.position = room.getPosition(userId);
                   sbdto.cardList = cardList;
                   brocast(room, OpCode._21Multi, _21MultiCode.SPLIT_BRO, sbdto, null);
                   //和开始游戏后进行一样的判断
                   //21点 直接赢了
                   if (room.is21ByUserId(userId))
                   {
                       //记录一下这组牌
                       room.SaveListByUserId(userId, 1, false);
                       int position = room.getPosition(userId);
                       OverHandDto ohdto = new OverHandDto(userId, position, 1);
                       brocast(room, OpCode._21Multi, _21MultiCode.OVER_HAND_BRO, ohdto, null);
                       //该下一个玩家了
                       turnPlayer(userId, position, room);
                   }
                   //能够分牌
                   if (room.isCanSplitByUserId(userId))
                   {
                       TurnHandDto thdto = new TurnHandDto(true, userId);
                       client.Send(OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto);
                       //brocast(room, OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto, null);
                   }
                   else
                   {
                       TurnHandDto thdto = new TurnHandDto(false, userId);
                       client.Send(OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto);
                       //brocast(room, OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto, null);

                   }



               }
               );


        }


        /// <summary>
        /// 分牌后要下一组牌
        /// </summary>
        /// <param name="client"></param>
        private void splitNext(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
               () =>
               {
                   if (userCache.IsOnline(client) == false)
                       return;
                   int userId = userCache.GetId(client);
                   if (fightCache.IsInRoom(userId) == false)
                       return;
                   //一定要注意安全的验证
                   _21MutiFightRoom room = fightCache.GetRoom(userId);
                   //发下一组牌
                   room.NextHandCardByUId(userId);
                   //获取分牌后的下一组手牌
                   List<CardDto> cardList = room.GetUserCardsByUserId(userId);
                   //广播发送一下手牌
                   SplitBroDto sbdto = new SplitBroDto();
                   sbdto.userId = userId;
                   sbdto.position = room.getPosition(userId);
                   sbdto.cardList = cardList;
                   //分牌和 下一手用的一个code
                   brocast(room, OpCode._21Multi, _21MultiCode.SPLIT_BRO, sbdto, null);
                   //和开始游戏后进行一样的判断
                   //21点 直接赢了
                   if (room.is21ByUserId(userId))
                   {
                       //记录一下这组牌
                       room.SaveListByUserId(userId, 1, false);
                       int position = room.getPosition(userId);
                       OverHandDto ohdto = new OverHandDto(userId, position, 1);
                       brocast(room, OpCode._21Multi, _21MultiCode.OVER_HAND_BRO, ohdto, null);
                       //该下一个玩家了
                       turnPlayer(userId, position, room);
                   }
                   //能够分牌
                   if (room.isCanSplitByUserId(userId))
                   {
                       TurnHandDto thdto = new TurnHandDto(true, userId);
                       client.Send(OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto);
                       //brocast(room, OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto, null);
                   }
                   else
                   {
                       TurnHandDto thdto = new TurnHandDto(false, userId);
                       client.Send(OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto);
                       //brocast(room, OpCode._21Multi, _21MultiCode.TURN_HS_BRO, thdto, null);

                   }



               }
               );

        }
        /// <summary>
        /// 广播
        /// </summary>
        /// <param name="opCode"></param>
        /// <param name="subCode"></param>
        /// <param name="value"></param>
        /// <param name="exClient"></param>
        private void brocast(_21MutiFightRoom room, int opCode, int subCode, object value, ClientPeer exClient = null)
        {
            SocketMsg msg = new SocketMsg(opCode, subCode, value);
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);

            foreach (var player in room.PlayerList)
            {
                //fixbug923 
                if (player!=null&&userCache.IsOnline(player.UserId)&&!room.LeaveUIdList.Contains(player.UserId))
                {
                    ClientPeer client = userCache.GetClientPeer(player.UserId);
                    if (client == exClient)
                        continue;
                    client.Send(packet);
                }
            }
        }
    }
}
