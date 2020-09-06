using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol.Dto;
using Protocol.Code;
using UnityEngine.UI;
using Protocol.Dto.Single;

public class StrategyPanel : UIBase
{
    protected void Awake()
    {
        Bind(UIEvent.REFRESH_BASIC_STRATEGY,
           UIEvent.REFRESH_COUNT_STRATEGY);

    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.REFRESH_BASIC_STRATEGY:
                {
                    refreshBasic(message as BasicStrategyDto);
                    break;
                }
            case UIEvent.REFRESH_COUNT_STRATEGY:
                {
                    refreshCount(message as CountStrategyDto);
                    break;
                }
                
            


            default:
                break;
        }

    }

    /// <summary>
    /// 角色的数据
    /// </summary>
    private Text txtBasic;
    private Text txtCount;



    protected virtual void Start()
    {
        txtBasic = transform.Find("txtBasic").GetComponent<Text>();
        txtCount = transform.Find("txtCount").GetComponent<Text>();

        //txtBasic.gameObject.SetActive(false);
        //txtCount.gameObject.SetActive(true);


    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    public void refreshBasic(BasicStrategyDto basicStrategyDto)
    {
        //txtBasic.gameObject.SetActive(true);
        if (basicStrategyDto==null)
        {
            txtBasic.text = "已经21点";
        }
        else
        {
            txtBasic.text = "庄家牌型 " + basicStrategyDto.dealerCardType + " 闲家牌型 " + basicStrategyDto.playerCardType + " 策略 " + basicStrategyDto.realAns;

        }
        
    }

    public void refreshCount(CountStrategyDto countStrategyDto)
    {
        txtCount.text = "流水数：" + countStrategyDto.runCount + "  剩余牌数：" + countStrategyDto.containCardCount + "/208  " + "真数：" + countStrategyDto.trueCount.ToString("0.00");
        //txtCount.gameObject.SetActive(true);
    }

}
