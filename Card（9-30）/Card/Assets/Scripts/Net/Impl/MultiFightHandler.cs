using Protocol.Code;
using Protocol.Dto.Multi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiFightHandler : HandlerBase
{

    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case _21MultiCode.MATCH_SRES:
                matchResponse(value as MutiRoomDto);
                break;
            case _21MultiCode.ENTER_SRES:
                enterResponse(value as MutiRoomDto);
                break;
            case _21MultiCode.ENTER_BRO:
                enterBro(value as MultiEnterDto);
                break;
            case _21MultiCode.LEAVE_BRO:
                leaveBro((int)value);
                break;
            case _21MultiCode.READY_BRO:
                readyBro((int)value);
                break;
            case _21MultiCode.START_BRO:
                startBro();
                break;
            case _21MultiCode.INIT_CARD_BRO:
                initCardBro(value as InitCardDto);
                break;
            case _21MultiCode.TURN_HS_BRO:
                turnHSBro(value as TurnHandDto);
                break;
            case _21MultiCode.OVER_HAND_BRO:
                overHandBro(value as OverHandDto);
                break;
            case _21MultiCode.SPLIT_BRO:
                splitBro(value as SplitBroDto);
                break;
            case _21MultiCode.SPLIT_CAN_NEXT_SRES:
                splitCanNextSres();
                break;
            case _21MultiCode.HIT_BRO:
                hitBro(value as HitBroDto);
                break;
            case _21MultiCode.TURN_DEALER_BRO:
                turnDealerBro();
                break;
            case _21MultiCode.DEALER_HIT_BRO:
                dealerHitBro(value as CardDto);
                break;
            case _21MultiCode.OVER_BRO:
                over(value as GameOverDto);
                break;
            case _21MultiCode.COUNT_DOWN_BRO:
                countDown();
                break;
            case _21MultiCode.STOP_COUNT_DOWN_BRO:
                stopCountDown();
                break;

            default:
                break;
        }
    }


    PromptMsg promptMsg = new PromptMsg();

    bool isEnter = false;
    bool isFight = false;

    private void matchResponse(MutiRoomDto multiRoomDto)
    {
        Models.GameModel.MutiRoomDto = multiRoomDto;
        //显示进入房间的按钮
        Dispatch(AreaCode.UI, UIEvent.MULTI_SHOW_ENTER_ROOM_BUTTON, null);
        
    }
    private void enterResponse(MutiRoomDto multiRoomDto)
    {
        isEnter = true;
        //Models.GameModel.MutiRoomDto = multiRoomDto;
        //todo 显示房间里的人 显示豆子
    }

    private void enterBro(MultiEnterDto multiEnterDto)
    {
        if(isEnter)
        {
            //更新房间数据
            MutiRoomDto room = Models.GameModel.MutiRoomDto;
            room.Add(multiEnterDto.userDto, multiEnterDto.position);

            //给UI绑定数据
            switch(multiEnterDto.position)
            {
                case 1:
                    Dispatch(AreaCode.UI, UIEvent.SET_1_PLAYER_DATA, multiEnterDto.userDto);
                    break;
                case 2:
                    Dispatch(AreaCode.UI, UIEvent.SET_2_PLAYER_DATA, multiEnterDto.userDto);
                    break;
                case 3:
                    Dispatch(AreaCode.UI, UIEvent.SET_3_PLAYER_DATA, multiEnterDto.userDto);
                    break;
                case 4:
                    Dispatch(AreaCode.UI, UIEvent.SET_4_PLAYER_DATA, multiEnterDto.userDto);
                    break;
                default:
                    break;
            }
            //发消息 显示玩家的状态面板所有游戏物体
            Dispatch(AreaCode.UI, UIEvent.MULTI_PLAYER_ENTER, multiEnterDto.userDto.Id);

            //给用户一个提示
            promptMsg.Change("有新玩家 ( " + multiEnterDto.userDto.Name + " )进入", UnityEngine.Color.blue);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
        }
        //进来一个显示一个
    }

    private void leaveBro(int leaveUserId)
    {
        //游戏开始和没开始分开

        //根据user
        //发消息 隐藏玩家的状态面板所有游戏物体
        if(!isFight)//游戏没有开始
        {
            Dispatch(AreaCode.UI, UIEvent.MULTI_PLAYER_LEAVE, leaveUserId);
            //
            Models.GameModel.MutiRoomDto.Leave(leaveUserId);

        }
        //战斗开始后离开的
        else
        {
            Dispatch(AreaCode.UI, UIEvent.MULTI_PLAYER_LEAVE, leaveUserId);
            //todo 把牌也得隐藏了
            int position = Models.GameModel.MutiRoomDto.UIdPositionDict[leaveUserId];
            switch(position)
            {
                case 1:
                    Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_CLEAR_PLAYER1_CARD, null);
                    break;
                case 2:
                    Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_CLEAR_PLAYER2_CARD, null);
                    break;
                case 3:
                    Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_CLEAR_PLAYER3_CARD, null);
                    break;
                case 4:
                    Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_CLEAR_PLAYER4_CARD, null);
                    break;

            }


            Models.GameModel.MutiRoomDto.LeaveUIdList.Add(leaveUserId);
        }
        
    }

    private void readyBro(int readyUserId)
    {
        //保存数据
        Models.GameModel.MutiRoomDto.Ready(readyUserId);
        //显示为玩家状态面板的准备文字
        Dispatch(AreaCode.UI, UIEvent.MULTI_PLAYER_READY, readyUserId);

        //fixbug923 判断是否是自身
        if (readyUserId == Models.GameModel.UserDto.Id)
        {
            //发送消息 隐藏准备按钮 防止多次点击 和服务器交互
            Dispatch(AreaCode.UI, UIEvent.MULTI_PLAYER_HIDE_READY_BUTTON, null);
        }
    }
    /// <summary>
    /// 游戏开始
    /// </summary>
    private void startBro()
    {
        isFight = true;
        Dispatch(AreaCode.UI, UIEvent.MULTI_PLAYER_HIDE_STATE, null);


    }

    private void initCardBro(InitCardDto initCardDto)
    {
        Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_INIT_DEALER_CARD, initCardDto.InitCardLists[0]);
        for(int i=1;i<5;i++)
        {
            if(initCardDto.InitCardLists[i]!=null)
            {
                initPlayerCardByPosition(i, initCardDto.InitCardLists[i]);
            }
        }
        
    }

    private void initPlayerCardByPosition(int position,List<CardDto> cardList)
    {
        switch(position)
        {
            case 1:
                Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_INIT_PLAYER1_CARD, cardList);
                break;
            case 2:
                Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_INIT_PLAYER2_CARD, cardList);
                break;
            case 3:
                Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_INIT_PLAYER3_CARD, cardList);
                break;
            case 4:
                Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_INIT_PLAYER4_CARD, cardList);
                break;
            default:
                break;
        }
    }

    private void turnHSBro(TurnHandDto turnHandDto)
    {
        if(Models.GameModel.UserDto.Id== turnHandDto.userId)
        {
            Dispatch(AreaCode.UI, UIEvent.MULTI_SHOW_HSD_BUTTON, true);
            promptMsg.Change("轮到你了", UnityEngine.Color.green);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
            if(turnHandDto.canSplit)
            {
                Dispatch(AreaCode.UI, UIEvent.MULTI_SHOW_SPLIT_BUTTON, true);
            }
        }
        else
        {
            Dispatch(AreaCode.UI, UIEvent.MULTI_SHOW_HSD_BUTTON, false);
        }
    }

    /// <summary>
    /// 一把牌结束给出提示
    /// </summary>
    /// <param name="overHandDto"></param>
    private void overHandBro(OverHandDto overHandDto)
    {
        string msg= overHandDto.position.ToString()+"号玩家";
        switch(overHandDto.type)
        {
            case 1:
                msg += "21点";
                break;
            case 2:
                msg += "爆牌";
                break;
            case 3:
                msg += "停牌";
                break;
            case 4:
                msg += "加倍后爆牌";
                break;
            case 5:
                msg += "加倍";
                break;
            default:
                break;


        }

        promptMsg.Change(msg, UnityEngine.Color.red);
        Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
    }


    /// <summary>
    /// 分牌响应
    /// </summary>
    /// <param name="splitBroDto"></param>
    private void splitBro(SplitBroDto splitBroDto)
    {
        string msg = splitBroDto.position.ToString() + "号玩家分牌";
        promptMsg.Change(msg, UnityEngine.Color.red);
        Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
        switch (splitBroDto.position)
        {
            case 1:
                Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_CLEAR_PLAYER1_CARD, null);
                Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_INIT_PLAYER1_CARD, splitBroDto.cardList);
                break;
            case 2:
                Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_CLEAR_PLAYER2_CARD, null);
                Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_INIT_PLAYER2_CARD, splitBroDto.cardList);
                break;
            case 3:
                Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_CLEAR_PLAYER3_CARD, null);
                Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_INIT_PLAYER3_CARD, splitBroDto.cardList);
                break;
            case 4:
                Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_CLEAR_PLAYER4_CARD, null);
                Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_INIT_PLAYER4_CARD, splitBroDto.cardList);
                break;

        }
        

        

    }

    /// <summary>
    /// 显示分牌后可下一手的按钮
    /// </summary>
    private void splitCanNextSres()
    {
        Dispatch(AreaCode.UI, UIEvent.MULTI_SHOW_SPLIT_NEXT_BUTTON, true);
        Dispatch(AreaCode.UI, UIEvent.MULTI_SHOW_HSD_BUTTON, false);
    }

    /// <summary>
    /// 要牌
    /// </summary>
    /// <param name="hitBroDto"></param>
    private void hitBro(HitBroDto hitBroDto)
    {
        if(hitBroDto.card==null&&hitBroDto.userId==Models.GameModel.UserDto.Id)
        {
            promptMsg.Change("已经21点了不能要牌", UnityEngine.Color.red);
        }
        else
        {
            switch (hitBroDto.position)
            {
                case 1:
                    Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_HIT_PLAYER1_CARD, hitBroDto.card);
                    break;
                case 2:
                    Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_HIT_PLAYER2_CARD, hitBroDto.card);
                    break;
                case 3:
                    Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_HIT_PLAYER3_CARD, hitBroDto.card);
                    break;
                case 4:
                    Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_HIT_PLAYER4_CARD, hitBroDto.card);
                    break;

            }
        }
        
    }

    /// <summary>
    /// 把第一张牌翻转一下
    /// </summary>
    private void turnDealerBro()
    {
        Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_CONVERT_FIRST_CARD, null);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="card"></param>
    private void dealerHitBro(CardDto card)
    {
        Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_HIT_DEALER_CARD, card);
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    /// <param name="gameOverDto"></param>
    private void over(GameOverDto gameOverDto)
    {
        //先把房间处理一下
        foreach(int uid in Models.GameModel.MutiRoomDto.LeaveUIdList)
        {
            Models.GameModel.MutiRoomDto.Leave(uid);
            Models.GameModel.MutiRoomDto.ReadyUIdList.Clear();
        }
        //结算面板
        Dispatch(AreaCode.UI, UIEvent.MULTI_SHOW_OVER_PANEL, gameOverDto);
        //更新一下用户信息
        for(int i=0;i<gameOverDto.userDtoList.Count;i++)
        {
            if(Models.GameModel.UserDto.Id== gameOverDto.userDtoList[i].Id)
            {
                Models.GameModel.UserDto = gameOverDto.userDtoList[i];
                break;
            }
        }
        //豆子面板刷新一下
        Dispatch(AreaCode.UI, UIEvent.MULTI_CHANGE_BEEN, Models.GameModel.UserDto.Been);
    }

    /// <summary>
    /// 倒计时
    /// </summary>
    private void countDown()
    {
        Dispatch(AreaCode.UI, UIEvent.SHOW_COUNT_DOWN_PANEL, null);
    }

    private void stopCountDown()
    {
        Dispatch(AreaCode.UI, UIEvent.STOP_COUNT_DOWN_PANEL, null);
    }
}
