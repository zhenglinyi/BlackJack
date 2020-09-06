using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol.Code;
using Protocol.Dto.Single;

public class SingleFightHandler : HandlerBase
{

    PromptMsg promptMsg = new PromptMsg();
    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case _21SingleCode.GET_PCARD_SRES:
                getPlayerCards(value as List<CardDto>);
                break;
            case _21SingleCode.GET_DCARD_SRES:
                getDealerCards(value as List<CardDto>);
                break;
            case _21SingleCode.OVER_SRES:
                overGame(value as SingleOverDto);
                break;
            case _21SingleCode.GET_SRES:
                getCard(value as CardDto);
                break;
            case _21SingleCode.ADD_DCARD_SRES:
                addDealerCard(value as CardDto);
                break;
            case _21SingleCode.RESHUFF_SRES:
                reshuff();
                break;
            case _21SingleCode.NGET_SRES:
                changeToDealer();
                break;
            case _21SingleCode.CAN_SPLIT_SRES:
                canSplit();
                break;
            case _21SingleCode.SPLIT_CAN_NEXT_SRES:
                splitCanNext();
                break;
            case _21SingleCode.SPLIT_PROMST_SRES:
                splitPromst((int)value);
                break;
            case _21SingleCode.BASIC_STRATEGY_SRES:
                basicStrategy(value as BasicStrategyDto);
                break;
            case _21SingleCode.COUNT_STRATEGY_SRES:
                countStrategy(value as CountStrategyDto);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 初始化闲家手牌
    /// </summary>
    /// <param name="cardList"></param>
    private void getPlayerCards(List<CardDto> cardList)
    {
        //牌的显示
        Dispatch(AreaCode.CHARACTER, CharacterEvent.INIT_PLAYER_CARD, cardList);
        
        
        Dispatch(AreaCode.UI, UIEvent.SINGLE_SHOW_GET_BUTTON, true);
        Dispatch(AreaCode.UI, UIEvent.SINGLE_SHOW_START_BUTTON, false);
        Dispatch(AreaCode.UI, UIEvent.SINGLE_SHOW_NEXT_BUTTON, false);
        

    }

    /// <summary>
    /// 初始化庄家手牌
    /// </summary>
    /// <param name="cardList"></param>
    private void getDealerCards(List<CardDto> cardList)
    {

        //牌的显示
        Dispatch(AreaCode.CHARACTER, CharacterEvent.INIT_DEALER_CARD, cardList);


    }

    /// <summary>
    /// 要了一张牌
    /// </summary>
    /// <param name="dto"></param>
    private void getCard(CardDto dto)
    {
        //增加
        Dispatch(AreaCode.CHARACTER, CharacterEvent.GET_PLAYER_CARD, dto);

        if (dto==null)
        {
            promptMsg.Change("已经21点，不能要牌", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
        }
    }
    /// <summary>
    /// 给庄家一张张发牌
    /// </summary>
    /// <param name="dto"></param>
    private void addDealerCard(CardDto dto)
    {

        //增加
        Dispatch(AreaCode.CHARACTER, CharacterEvent.GET_DEALER_CARD, dto);

    }
    /// <summary>
    /// 重新洗牌给个提示
    /// </summary>
    private void reshuff()
    {
        promptMsg.Change("重新洗牌了", Color.red);
        Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);

    }
    /// <summary>
    /// 游戏结束
    /// </summary>
    /// <param name="value"></param>
    private void overGame(SingleOverDto overDto)
    {

        //switch (overDto.whichwin)
        //{
        //    case 1:
        //        promptMsg.Change("21点，闲家获胜", Color.red);
        //        break;
        //    case 2:
        //        promptMsg.Change("闲家爆牌，庄家获胜", Color.red);
        //        break;
        //    case 3:
        //        promptMsg.Change("庄家爆牌，闲家获胜", Color.red);
        //        break;
        //    case 4:
        //        promptMsg.Change("庄家获胜", Color.red);
        //        break;
        //    case 5:
        //        promptMsg.Change("闲家获胜", Color.red);
        //        break;
        //    case 6:
        //        promptMsg.Change("平局", Color.red);
        //        break;
        //    case 7:
        //        promptMsg.Change("....分牌了我该说啥....", Color.red);
        //        break;
        //    default:
        //        break;
        //}
        //Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);
        //todo牌面清空 做一些豆子处理

        Dispatch(AreaCode.UI, UIEvent.SINGLE_SHOW_OVER_PANEL, overDto);
        Models.GameModel.UserDto = overDto.userDto;
        //更新豆子
        Dispatch(AreaCode.UI, UIEvent.SINGLE_CHANGE_BEEN, Models.GameModel.UserDto.Been);
        //倍数还原
        //Dispatch(AreaCode.UI, UIEvent.CHANGE_MUTIPLE, 1);
        //显示下一把按钮
        Dispatch(AreaCode.UI, UIEvent.SINGLE_SHOW_GET_BUTTON,false);
        Dispatch(AreaCode.UI, UIEvent.SINGLE_SHOW_START_BUTTON, false);
        Dispatch(AreaCode.UI, UIEvent.SINGLE_SHOW_NEXT_BUTTON, true);

        
    }

    /// <summary>
    /// 不要牌响应，即转换给庄家
    /// </summary>
    private void changeToDealer()
    {
        //庄家第一张牌翻转
        Dispatch(AreaCode.CHARACTER, CharacterEvent.CONVERT_FIRST_CARD, null);
    }

    /// <summary>
    /// 能够分牌
    /// </summary>
    private void canSplit()
    {
        Dispatch(AreaCode.UI, UIEvent.SINGLE_SHOW_SPLIT_BUTTON, true);
    }

    private void splitCanNext()
    {
        Dispatch(AreaCode.UI, UIEvent.SINGLE_SHOW_NEXTSPLIT_BUTTON, true);
        Dispatch(AreaCode.UI, UIEvent.SINGLE_SHOW_GET_BUTTON, false);
    }

    private void splitPromst(int value)
    {
        switch(value)
        {
            case 0:
                promptMsg.Change("停牌", Color.red);
                break;
            case 1:
                promptMsg.Change("21点", Color.red);
                break;
            case 2:
                promptMsg.Change("爆牌", Color.red);
                break;
            default:
                break;
        }

        Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMsg);

    }

    public void basicStrategy(BasicStrategyDto bsdto)
    {
        Dispatch(AreaCode.UI, UIEvent.REFRESH_BASIC_STRATEGY, bsdto);
    }

    public void countStrategy(CountStrategyDto csdto)
    {
        Dispatch(AreaCode.UI, UIEvent.REFRESH_COUNT_STRATEGY, csdto);
    }



}
