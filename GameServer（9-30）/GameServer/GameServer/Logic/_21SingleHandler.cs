using AhpilyServer;
using GameServer.Cache;
using GameServer.Cache._21single;
using GameServer.Model;
using Protocol.Code;
using Protocol.Dto.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol.Dto;
using System.Threading;

namespace GameServer.Logic
{
    class _21SingleHandler : IHandler
    {
        public _21SingleCache singleCache = Caches.Single;
        public UserCache userCache = Caches.User;
        public BasicStrategy basicStrategy = new BasicStrategy();
        public void OnDisconnect(ClientPeer client)
        {
            leave(client);
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case _21SingleCode.START_CREQ:

                    startFight(client);
                    break;
                case _21SingleCode.GET_CREQ:
                    getCard(client);
                    break;
                case _21SingleCode.NGET_CREQ:
                    ngetCard(client);
                    break;
                case _21SingleCode.NEXT_CREQ:
                    nextGame(client);
                    break;
                case _21SingleCode.LEAVE_CREQ:
                    leave(client);
                    break;
                case _21SingleCode.SET_WAGER_CREQ:
                    setWager(client,(int)value);
                    break;
                case _21SingleCode.DOUBLE_CREQ:
                    doubleCard(client);
                    break;
                case _21SingleCode.MATCH_CREQ:
                    matchCard(client);
                    break;
                case _21SingleCode.SPLIT_CREQ:
                    splitCard(client);
                    break;
                case _21SingleCode.SPLIT_NEXT_CREQ:
                    splitNextCard(client);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 用户离开
        /// </summary>
        /// <param name="client"></param>
        private void leave(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
                () =>
                {
                    
                    if (userCache.IsOnline(client) == false)
                        return;
                     
                    //必须确保在线
                    int userId = userCache.GetId(client);
                    if (singleCache.IsFighting(userId) == false)
                    {
                        return;
                    }

                    SingleRoom room = singleCache.GetRoomByUId(userId);
                    //销毁缓存层的房间数据
                    singleCache.Destroy(room);

                });
        }

        /// <summary>
        /// 匹配，创建一个房间
        /// </summary>
        /// <param name="client"></param>
        public void matchCard(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    //必须确保在线
                    int userId = userCache.GetId(client);
                    //创建战斗房间
                    SingleRoom room = singleCache.Create(userId);
                    //CountStrategyDto countStrategyDto = room.libraryModel.GetCountStrategyDto();
                    //client.Send(OpCode._21Single, _21SingleCode.COUNT_STRATEGY_SRES, countStrategyDto);



                });

        }
        /// <summary>
        /// 开始战斗
        /// </summary>
        public void startFight(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    //必须确保在线
                    int userId = userCache.GetId(client);
                    //创建战斗房间
                    SingleRoom room = singleCache.GetRoomByUId(userId);
                    //if (room.libraryModel.CardQueue.Count() < 12)
                    //{
                    //    room.libraryModel.Init();
                    //    client.Send(OpCode._21Single, _21SingleCode.RESHUFF_SRES, null);//重新洗牌，给客户端，让客户端给提示
                    //}

                    room.InitPlayerCards();
                    //发送给客户端 他自身有什么牌 庄家有什么牌
                    List<CardDto> cardList = room.GetUserCards(userId);
                    client.Send(OpCode._21Single, _21SingleCode.GET_PCARD_SRES, cardList);
                    client.Send(OpCode._21Single, _21SingleCode.GET_DCARD_SRES, room.DealerCardList);

                    Thread.Sleep(100);
                    //摸到21点，游戏结束
                    if (room.isPlayer21())//21点 这局结束
                    {
                        //client.Send(OpCode._21Single, _21SingleCode.OVER_SRES, 1);
                        client.Send(OpCode._21Single, _21SingleCode.NGET_SRES, null);
                        gameOver(userId,room, 1);
                    }
                    //能够分牌了
                    else if(cardList[0].Weight==cardList[1].Weight)
                    {
                        
                        BasicStrategyDto basicStrategyDto = new BasicStrategyDto();
                        basicStrategyDto.dealerCardType = room.getDealerCardType();
                        basicStrategyDto.playerCardType = room.getPlayerCardType();
                        basicStrategyDto.realAns = basicStrategy.trueStrategy(basicStrategyDto.dealerCardType, basicStrategyDto.playerCardType);
                        client.Send(OpCode._21Single, _21SingleCode.BASIC_STRATEGY_SRES, basicStrategyDto);
                        client.Send(OpCode._21Single, _21SingleCode.CAN_SPLIT_SRES, null);
                    }
                    else
                    {
                        BasicStrategyDto basicStrategyDto = new BasicStrategyDto();
                        basicStrategyDto.dealerCardType = room.getDealerCardType();
                        basicStrategyDto.playerCardType = room.getPlayerCardType();
                        basicStrategyDto.realAns = basicStrategy.trueStrategy(basicStrategyDto.dealerCardType, basicStrategyDto.playerCardType);
                        client.Send(OpCode._21Single, _21SingleCode.BASIC_STRATEGY_SRES, basicStrategyDto);
                    }
                 
                });
        }

        /// <summary>
        /// 要牌
        /// </summary>
        /// <param name="client"></param>
        public void getCard(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    //必须确保在线
                    int userId = userCache.GetId(client);

                    SingleRoom room = singleCache.GetRoomByUId(userId);
                    if(room.isPlayer21())
                    {
                        client.Send(OpCode._21Single, _21SingleCode.GET_SRES, null);//已经21点不能要牌，最好客户端设置一下
                        return;
                    }
                    CardDto dto = room.GetOnePlayerCard();
                    client.Send(OpCode._21Single, _21SingleCode.GET_SRES, dto);
                    if(room.GetPlayerWeight()>21)//闲家爆牌 这局结束
                    {
                        if (room.player.SplitNum > 1)//是分牌的情况
                        {
                            client.Send(OpCode._21Single, _21SingleCode.SPLIT_PROMST_SRES, 2);//闲家爆牌
                            //room.player.winOrLose.Add(-1);
                            //room.player.SpliteCardListList.Add(room.player.CardList);
                            room.SaveList(1, false);
                            if (room.player.SpliteCardListQueue.Count() > 0)
                            {
                                
                                
                                client.Send(OpCode._21Single, _21SingleCode.SPLIT_CAN_NEXT_SRES, null);

                            }
                            else
                            {
                                gameOver(userId, room, 7);
                            }

                        }
                        else
                        {
                            client.Send(OpCode._21Single, _21SingleCode.NGET_SRES, null);
                            gameOver(userId, room, 2);
                        }
                        
                    }
                    else if(room.GetPlayerWeight()!=21)
                    {
                        BasicStrategyDto basicStrategyDto = new BasicStrategyDto();
                        basicStrategyDto.dealerCardType = room.getDealerCardType();
                        basicStrategyDto.playerCardType = room.getPlayerCardType();
                        basicStrategyDto.realAns = basicStrategy.trueStrategy(basicStrategyDto.dealerCardType, basicStrategyDto.playerCardType);
                        client.Send(OpCode._21Single, _21SingleCode.BASIC_STRATEGY_SRES, basicStrategyDto);
                    }
                    else
                    {
                        client.Send(OpCode._21Single, _21SingleCode.BASIC_STRATEGY_SRES, null);
                    }


                });
        }
        /// <summary>
        /// 不要牌
        /// </summary>
        /// <param name="client"></param>
        public void ngetCard(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    //必须确保在线
                    int userId = userCache.GetId(client);

                    SingleRoom room = singleCache.GetRoomByUId(userId);
                    
                    //client.Send(OpCode._21Single, _21SingleCode.OVER_SRES, whichwin);
                    //说明是分牌的
                    if(room.player.SplitNum>1)
                    {
                        //room.addWeightAndMulti(1);
                        room.SaveList(3, false);
                        //client.Send(OpCode._21Single, _21SingleCode.SPLIT_PROMST_SRES, whichwin);
                        //先不能判断输赢
                        if (room.player.SpliteCardListQueue.Count() > 0)
                        {
                            client.Send(OpCode._21Single, _21SingleCode.SPLIT_PROMST_SRES, 0);//随便提示一下
                            
                            client.Send(OpCode._21Single, _21SingleCode.SPLIT_CAN_NEXT_SRES, null);                          

                        }
                        else
                        {
                            gameOver(userId, room, 7);
                        }

                    }
                    else
                    {
                        client.Send(OpCode._21Single, _21SingleCode.NGET_SRES, null);
                        while (room.GetDealerWeight() < 17)
                        {
                            CardDto dto = room.GetOneDealerCard();
                            client.Send(OpCode._21Single, _21SingleCode.ADD_DCARD_SRES, dto);
                        }

                        int whichwin = room.whichWin();
                        gameOver(userId, room, whichwin);
                    }
                    

                });

        }
        /// <summary>
        /// 下一把
        /// </summary>
        /// <param name="client"></param>
        public void nextGame(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    //必须确保在线
                    int userId = userCache.GetId(client);

                    SingleRoom room = singleCache.GetRoomByUId(userId);
                    room.Multiple = 1;
                    if (room.libraryModel.CardQueue.Count() < 12)
                    {
                        room.libraryModel.Init();
                        client.Send(OpCode._21Single, _21SingleCode.RESHUFF_SRES, null);//重新洗牌，给客户端，让客户端给提示
                    }
                    //测试，只发俩A
                    //room.TestInitPlayerCards();
                    room.InitPlayerCards();
                    //发送给客户端 他自身有什么牌 庄家有什么牌
                    List<CardDto> cardList = room.GetUserCards(userId);
                    client.Send(OpCode._21Single, _21SingleCode.GET_PCARD_SRES, cardList);
                    client.Send(OpCode._21Single, _21SingleCode.GET_DCARD_SRES, room.DealerCardList);
                    Thread.Sleep(100);
                    //摸到21点，牛逼，游戏结束
                    if (room.isPlayer21())//21点 这局结束
                    {

                        //client.Send(OpCode._21Single, _21SingleCode.OVER_SRES, 1);
                        client.Send(OpCode._21Single, _21SingleCode.NGET_SRES, null);
                        gameOver(userId,room, 1);
                    }
                    //能够分牌了
                    else if (cardList.Count>1&&cardList[0].Weight == cardList[1].Weight)
                    {
                        BasicStrategyDto basicStrategyDto = new BasicStrategyDto();
                        basicStrategyDto.dealerCardType = room.getDealerCardType();
                        basicStrategyDto.playerCardType = room.getPlayerCardType();
                        basicStrategyDto.realAns = basicStrategy.trueStrategy(basicStrategyDto.dealerCardType, basicStrategyDto.playerCardType);
                        client.Send(OpCode._21Single, _21SingleCode.BASIC_STRATEGY_SRES, basicStrategyDto);
                        client.Send(OpCode._21Single, _21SingleCode.CAN_SPLIT_SRES, null);
                    }
                    else
                    {
                        BasicStrategyDto basicStrategyDto = new BasicStrategyDto();
                        basicStrategyDto.dealerCardType = room.getDealerCardType();
                        basicStrategyDto.playerCardType = room.getPlayerCardType();
                        basicStrategyDto.realAns = basicStrategy.trueStrategy(basicStrategyDto.dealerCardType, basicStrategyDto.playerCardType);
                        client.Send(OpCode._21Single, _21SingleCode.BASIC_STRATEGY_SRES, basicStrategyDto);
                    }
                });

        }

        /// <summary>
        /// 设置赌注
        /// </summary>
        /// <param name="client"></param>
        public void setWager(ClientPeer client,int wager)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    //必须确保在线
                    int userId = userCache.GetId(client);
                    //创建战斗房间
                    SingleRoom room = singleCache.GetRoomByUId(userId);
                    room.Wager = wager;

                });
        }
        
        public void gameOver(int userId,SingleRoom room,int whichwin)
        {

            UserModel um = userCache.GetModelById(userId);
            ClientPeer client = userCache.GetClientPeer(userId);
            int wager = room.Wager;
            SingleOverDto odto = new SingleOverDto();
            
            


            switch (whichwin)
            {
                case 1:
                    {
                        int dealerWeight = room.GetDealerWeight();
                        odto.dealerWeight = dealerWeight;
                        //21点，和庄家比 获胜或平局
                        if (dealerWeight != 21)//获胜
                        {
                            um.Been += (int)(1.5 * wager);
                            um.Exp += 150;
                            odto.dealerState = 3;
                            odto.playerStateList.Add(1);
                            odto.playerWeightList.Add(room.GetPlayerWeight());
                            odto.playerWinBeenList.Add((int)(1.5 * wager));
                        }
                        else
                        {
                            um.Exp += 75;
                            odto.dealerState = 1;
                            odto.playerStateList.Add(0);
                            odto.playerWeightList.Add(room.GetPlayerWeight());
                            odto.playerWinBeenList.Add((int)(0));

                        }
                        break;
                    }
                    
                    
                    
                case 2://闲家爆牌
                case 4://庄家获胜
                    {
                        int dealerWeight = room.GetDealerWeight();
                        odto.dealerWeight = dealerWeight;
                        odto.dealerState = 3;
                        um.Been -= wager * room.Multiple;
                        um.Exp += 50;
                        odto.playerWeightList.Add(room.GetPlayerWeight());
                        odto.playerWinBeenList.Add(-(wager * room.Multiple));

                        if(room.Multiple==2)
                        {
                            if(room.GetPlayerWeight()>21)//加倍爆牌
                            {
                                odto.playerStateList.Add(3);
                            }
                            else
                            {
                                odto.playerStateList.Add(5);//加倍输
                            }
                        }
                        else
                        {
                            if (room.GetPlayerWeight() > 21)//加倍爆牌
                            {
                                odto.playerStateList.Add(2);
                            }
                            else
                            {
                                odto.playerStateList.Add(7);//加倍输
                            }
                        }
                        
                        break;
                    }
                    
                case 3://庄家爆牌，闲家获胜
                case 5:
                    {
                        int dealerWeight = room.GetDealerWeight();
                        odto.dealerWeight = dealerWeight;
                        if(room.GetDealerWeight()>21)//爆牌
                        {
                            odto.dealerState = 2;
                        }
                        else
                        {
                            odto.dealerState = 3;
                        }
                        

                        um.Been += wager * room.Multiple;
                        odto.playerWinBeenList.Add(wager * room.Multiple);
                        odto.playerWeightList.Add(room.GetPlayerWeight());
                        if (room.Multiple==2)//加倍赢
                        {
                            odto.playerStateList.Add(4);
                        }
                        else
                        {
                            odto.playerStateList.Add(6);//赢
                        }
                        um.Exp += 100;
                        break;
                    }
                    
                case 6:
                    {
                        int dealerWeight = room.GetDealerWeight();
                        odto.dealerWeight = dealerWeight;
                        odto.dealerState = 3;
                        odto.playerWinBeenList.Add(0);
                        odto.playerWeightList.Add(room.GetPlayerWeight());
                        odto.playerStateList.Add(8);//平
                        um.Exp += 75;
                        break;
                    }
                    
                case 7:
                    {
                        //看看是不是全爆牌了
                        bool isallboom = room.isAllBoom();

                        //这个是让客户端翻面
                        client.Send(OpCode._21Single, _21SingleCode.NGET_SRES, null);
                        if(!isallboom)
                        {
                            while (room.GetDealerWeight() < 17)
                            {
                                CardDto carddto = room.GetOneDealerCard();
                                client.Send(OpCode._21Single, _21SingleCode.ADD_DCARD_SRES, carddto);
                            }
                        }
                        int dealerWeight = room.GetDealerWeight();
                        odto.dealerWeight = dealerWeight;

                        if (room.DealerCardList.Count == 2 && room.GetDealerWeight() == 21)//庄家是21点
                        {
                            odto.dealerState = 1;
                        }
                        else if(room.GetDealerWeight()>21)
                        {
                            odto.dealerState = 2;
                        }
                        else
                        {
                            odto.dealerState = 3;
                        }

                        for (int j = 0; j < room.player.SpliteCardListList.Count; j++)
                        {
                            int multi = 1;

                            PlayerCardDto tempPlayerCardDto = room.player.SpliteCardListList[j];
                            int tempWeight = tempPlayerCardDto.Weight;
                            odto.playerWeightList.Add(tempWeight);

                            int tempState = tempPlayerCardDto.CardState;
                            bool tempIsDouble = tempPlayerCardDto.isDouble;
                            if (tempIsDouble)
                            {
                                multi = 2;

                            }


                            //21点
                            if (tempState == 1)
                            {
                                if(dealerWeight==1)//平局
                                {
                                    odto.playerWinBeenList.Add(0);
                                    odto.playerStateList.Add(0);
                                    um.Exp += 75;
                                }
                                else
                                {
                                    um.Been += (int)(wager * 1.5);
                                    odto.playerWinBeenList.Add((int)(wager * 1.5));
                                    odto.playerStateList.Add(1);
                                    
                                    um.Exp += 150;
                                }
                                
                                
                            }
                            //闲家爆牌
                            else if (tempState == 2)
                            {
                                um.Been -= wager * multi;
                                odto.playerWinBeenList.Add(-wager * multi);
                                if (multi == 1)
                                    odto.playerStateList.Add(2);
                                else
                                    odto.playerStateList.Add(3);
                                um.Exp += 50;

                            }
                            //不要 权值小于
                            else if (tempState == 3)
                            {
                                //庄家爆牌
                                if (dealerWeight > 21)
                                {
                                    um.Been += wager * multi;
                                    odto.playerWinBeenList.Add(wager * multi);
                                    if (multi == 1)
                                        odto.playerStateList.Add(6);
                                    else
                                        odto.playerStateList.Add(4);

                                    um.Exp += 100;
                                }
                                //闲家点大
                                else if (dealerWeight < tempWeight)
                                {
                                    um.Been += wager * multi;
                                    odto.playerWinBeenList.Add(wager * multi);
                                    if (multi == 1)
                                        odto.playerStateList.Add(6);
                                    else
                                        odto.playerStateList.Add(4);
                                    um.Exp += 100;
                                }
                                //庄家点大
                                else if (dealerWeight > tempWeight)
                                {
                                    um.Been -= wager * multi;
                                    odto.playerWinBeenList.Add(-wager * multi);
                                    if (multi == 1)
                                        odto.playerStateList.Add(7);
                                    else
                                        odto.playerStateList.Add(5);
                                    um.Exp += 50;
                                }
                                //平局
                                else
                                {
                                    odto.playerWinBeenList.Add(0);
                                    odto.playerStateList.Add(8);

                                    um.Exp += 75;
                                }

                            }

                        }

                        //double sum = room.computeWinOrLoss();
                        //um.Been += (int)(sum * wager);


                        //um.Exp += 50 * room.player.SplitNum;
                        //清理工作
                        room.player.SplitNum = 1;
                        room.splitClear();

                        break;
                    }
                    

                default:
                    break;
            }
            int maxExp = um.Lv * 100;
            while (maxExp <= um.Exp)
            {
                um.Lv++;
                um.Exp -= maxExp;
                maxExp = um.Lv * 100;
            }
            userCache.Update(um);
            UserDto dto = new UserDto(um.Id, um.Name, um.Been, um.WinCount, um.LoseCount, um.RunCount, um.Lv, um.Exp);
            //SingleOverDto odto = new SingleOverDto(dto, whichwin);
            odto.userDto = dto;
            client.Send(OpCode._21Single, _21SingleCode.OVER_SRES, odto);
            room.player.CardList.Clear();
            room.DealerCardList.Clear();
            Thread.Sleep(100);
            CountStrategyDto countStrategyDto = room.libraryModel.GetCountStrategyDto();
            client.Send(OpCode._21Single, _21SingleCode.COUNT_STRATEGY_SRES, countStrategyDto);
        }

        /// <summary>
        /// 加倍
        /// </summary>
        /// <param name="client"></param>
        public void doubleCard(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    //必须确保在线
                    int userId = userCache.GetId(client);

                    SingleRoom room = singleCache.GetRoomByUId(userId);
                    
                    CardDto dto = room.GetOnePlayerCard();
                    client.Send(OpCode._21Single, _21SingleCode.GET_SRES, dto);
                    //说明是分牌的
                    if (room.player.SplitNum > 1)
                    {
                        if (room.GetPlayerWeight() > 21)//闲家爆牌 这局结束
                        {
                            room.SaveList(2, true);
                            client.Send(OpCode._21Single, _21SingleCode.SPLIT_PROMST_SRES, 2);//闲家爆牌
                            //room.player.SpliteCardListList.Add(room.player.CardList);
                            if (room.player.SpliteCardListQueue.Count() > 0)
                            {

                                //room.player.winOrLose.Add(-2);//加倍的时候爆牌了，所以是-2
                                client.Send(OpCode._21Single, _21SingleCode.SPLIT_CAN_NEXT_SRES, null);

                            }
                            else
                            {
                                gameOver(userId, room, 7);
                            }

                        }
                        else
                        {
                            //room.addWeightAndMulti(2);
                            room.SaveList(3, true);
                            //client.Send(OpCode._21Single, _21SingleCode.SPLIT_PROMST_SRES, whichwin);
                            //先不能判断输赢
                            if (room.player.SpliteCardListQueue.Count() > 0)
                            {
                                client.Send(OpCode._21Single, _21SingleCode.SPLIT_PROMST_SRES, 0);//随便提示一下

                                client.Send(OpCode._21Single, _21SingleCode.SPLIT_CAN_NEXT_SRES, null);

                            }
                            else
                            {
                                gameOver(userId, room, 7);
                            }
                        }

                    }
                    else
                    {
                        //设置倍数
                        room.Multiple = 2;
                        if (room.GetPlayerWeight() > 21)//闲家爆牌 这局结束
                        {

                            //client.Send(OpCode._21Single, _21SingleCode.OVER_SRES, 2);
                            client.Send(OpCode._21Single, _21SingleCode.NGET_SRES, null);
                            gameOver(userId, room, 2);
                        }
                        else
                        {
                            client.Send(OpCode._21Single, _21SingleCode.NGET_SRES, null);
                            while (room.GetDealerWeight() < 17)
                            {
                                CardDto dto2 = room.GetOneDealerCard();
                                client.Send(OpCode._21Single, _21SingleCode.ADD_DCARD_SRES, dto2);
                            }

                            int whichwin = room.whichWin();
                            //client.Send(OpCode._21Single, _21SingleCode.OVER_SRES, whichwin);
                            gameOver(userId, room, whichwin);
                        }

                    }

                   

                    


                });
        }

        /// <summary>
        /// 分牌
        /// </summary>
        /// <param name="client"></param>
        public void splitCard(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    //必须确保在线
                    int userId = userCache.GetId(client);

                    SingleRoom room = singleCache.GetRoomByUId(userId);
                    room.SpliteCard();
                    List<CardDto> cardList = room.GetUserCards(userId);
                    client.Send(OpCode._21Single, _21SingleCode.GET_PCARD_SRES, cardList);
                    if (room.isPlayer21())//21点 这局结束
                    {
                        //room.player.SpliteCardListList.Add(cardList);
                        //client.Send(OpCode._21Single, _21SingleCode.OVER_SRES, 1);
                        room.SaveList(1, false);
                        if (room.player.SpliteCardListQueue.Count()>0)
                        {
                            client.Send(OpCode._21Single, _21SingleCode.SPLIT_PROMST_SRES, 1);//21点牛逼
                            //room.player.winOrLose.Add(1.5);                            
                            client.Send(OpCode._21Single, _21SingleCode.SPLIT_CAN_NEXT_SRES, null);
                            
                        }
                        else
                        {
                            gameOver(userId, room, 7);
                        }

                    }
                    else if (cardList[0].Weight == cardList[1].Weight)
                    {
                        BasicStrategyDto basicStrategyDto = new BasicStrategyDto();
                        basicStrategyDto.dealerCardType = room.getDealerCardType();
                        basicStrategyDto.playerCardType = room.getPlayerCardType();
                        basicStrategyDto.realAns = basicStrategy.trueStrategy(basicStrategyDto.dealerCardType, basicStrategyDto.playerCardType);
                        client.Send(OpCode._21Single, _21SingleCode.BASIC_STRATEGY_SRES, basicStrategyDto);
                        client.Send(OpCode._21Single, _21SingleCode.CAN_SPLIT_SRES, null);
                    }
                    else
                    {
                        BasicStrategyDto basicStrategyDto = new BasicStrategyDto();
                        basicStrategyDto.dealerCardType = room.getDealerCardType();
                        basicStrategyDto.playerCardType = room.getPlayerCardType();
                        basicStrategyDto.realAns = basicStrategy.trueStrategy(basicStrategyDto.dealerCardType, basicStrategyDto.playerCardType);
                        client.Send(OpCode._21Single, _21SingleCode.BASIC_STRATEGY_SRES, basicStrategyDto);
                    }

                   

                });
        }

        public void splitNextCard(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    //必须确保在线
                    int userId = userCache.GetId(client);

                    SingleRoom room = singleCache.GetRoomByUId(userId);
                    room.NextHandCard();
                    List<CardDto> cardList = room.GetUserCards(userId);
                    client.Send(OpCode._21Single, _21SingleCode.GET_PCARD_SRES, cardList);
                    
                    if (room.isPlayer21())//21点 这局结束
                    {
                        room.SaveList(1, false);
                        //room.player.SpliteCardListList.Add(cardList);
                        if (room.player.SpliteCardListQueue.Count() > 0)
                        {
                            client.Send(OpCode._21Single, _21SingleCode.SPLIT_PROMST_SRES, 1);//21点牛逼
                            //room.player.winOrLose.Add(1.5);
                            client.Send(OpCode._21Single, _21SingleCode.SPLIT_CAN_NEXT_SRES, null);
                            
                        }
                        else
                        {
                            gameOver(userId, room, 7);
                        }

                    }
                    else if (cardList.Count>1&&cardList[0].Weight == cardList[1].Weight)
                    {
                        BasicStrategyDto basicStrategyDto = new BasicStrategyDto();
                        basicStrategyDto.dealerCardType = room.getDealerCardType();
                        basicStrategyDto.playerCardType = room.getPlayerCardType();
                        basicStrategyDto.realAns = basicStrategy.trueStrategy(basicStrategyDto.dealerCardType, basicStrategyDto.playerCardType);
                        client.Send(OpCode._21Single, _21SingleCode.BASIC_STRATEGY_SRES, basicStrategyDto);
                        client.Send(OpCode._21Single, _21SingleCode.CAN_SPLIT_SRES, null);
                    }
                    else
                    {
                        BasicStrategyDto basicStrategyDto = new BasicStrategyDto();
                        basicStrategyDto.dealerCardType = room.getDealerCardType();
                        basicStrategyDto.playerCardType = room.getPlayerCardType();
                        basicStrategyDto.realAns = basicStrategy.trueStrategy(basicStrategyDto.dealerCardType, basicStrategyDto.playerCardType);
                        client.Send(OpCode._21Single, _21SingleCode.BASIC_STRATEGY_SRES, basicStrategyDto);
                    }



                });

        }
    }
}
