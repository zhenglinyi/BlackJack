using Protocol.Dto.Single;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleOverPanel : UIBase
{

	private void Awake()
	{
		Bind(UIEvent.SINGLE_SHOW_OVER_PANEL);
	}

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.SINGLE_SHOW_OVER_PANEL:
                refreshPanel(message as SingleOverDto);
                break;
            default:
                break;
        }
    }

    
    private Text txtDealerSettle;
    private Text txtPlayerSettle;

    private Button btnBack;

    void Start()
    {
        txtDealerSettle = transform.Find("txtDealerSettle").GetComponent<Text>();
        txtPlayerSettle = transform.Find("txtPlayerSettle").GetComponent<Text>();
        btnBack = transform.Find("btnBack").GetComponent<Button>();
        btnBack.onClick.AddListener(backClick);

        setPanelActive(false);
    }

    /// <summary>
    /// 刷新显示
    /// </summary>
    private void refreshPanel(SingleOverDto singleOverDto)
    {
        //显示面板
        setPanelActive(true);

        txtDealerSettle.text = "庄家 ：点数 ";
        txtDealerSettle.text += singleOverDto.dealerWeight;
        txtDealerSettle.text += "  ";
        if(singleOverDto.dealerState==1)
        {
            txtDealerSettle.text += "21点";
        }
        else if (singleOverDto.dealerState==2)
        {
            txtDealerSettle.text += "爆牌";
        }

        txtPlayerSettle.text = "";
        for (int j = 0; j < singleOverDto.playerWeightList.Count; j++)
        {
            txtPlayerSettle.text += "点数 ";
            //1 21点，2 爆牌，3 加倍爆牌 ，4 加倍 赢 ，5 加倍 输，6赢 ，7 输,8 平
            switch (singleOverDto.playerStateList[j])
            {
                case 0:
                    {
                        txtPlayerSettle.text += singleOverDto.playerWeightList[j];
                        txtPlayerSettle.text += " 黑杰克(平)  + ";
                        txtPlayerSettle.text += singleOverDto.playerWinBeenList[j];
                        break;
                    }


                case 1:
                    {
                        txtPlayerSettle.text += singleOverDto.playerWeightList[j];
                        txtPlayerSettle.text += " 黑杰克(赢)  + ";
                        txtPlayerSettle.text += singleOverDto.playerWinBeenList[j];
                        break;
                    }
                case 2:
                    {
                        txtPlayerSettle.text += singleOverDto.playerWeightList[j];
                        txtPlayerSettle.text += "爆牌  ";
                        txtPlayerSettle.text += singleOverDto.playerWinBeenList[j];
                        break;
                    }
                case 3:
                    {
                        txtPlayerSettle.text += singleOverDto.playerWeightList[j];
                        txtPlayerSettle.text += "加倍爆牌  ";
                        txtPlayerSettle.text += singleOverDto.playerWinBeenList[j];
                        break;
                    }
                case 4:
                    {
                        txtPlayerSettle.text += singleOverDto.playerWeightList[j];
                        txtPlayerSettle.text += "加倍 赢  + ";
                        txtPlayerSettle.text += singleOverDto.playerWinBeenList[j];
                        break;
                    }
                case 5:
                    {
                        txtPlayerSettle.text += singleOverDto.playerWeightList[j];
                        txtPlayerSettle.text += "加倍 输  ";
                        txtPlayerSettle.text += singleOverDto.playerWinBeenList[j];
                        break;
                    }
                case 6:
                    {
                        txtPlayerSettle.text += singleOverDto.playerWeightList[j];
                        txtPlayerSettle.text += "赢  +";
                        txtPlayerSettle.text += singleOverDto.playerWinBeenList[j];
                        break;
                    }
                case 7:
                    {
                        txtPlayerSettle.text += singleOverDto.playerWeightList[j];
                        txtPlayerSettle.text += "输  ";
                        txtPlayerSettle.text += singleOverDto.playerWinBeenList[j];
                        break;
                    }
                case 8:
                    {
                        txtPlayerSettle.text += singleOverDto.playerWeightList[j];
                        txtPlayerSettle.text += "平  + ";
                        txtPlayerSettle.text += singleOverDto.playerWinBeenList[j];
                        break;
                    }



            }

            txtPlayerSettle.text += "\n";
        }

    }
    private void backClick()
    {
        setPanelActive(false);
        //Dispatch(AreaCode.UI, UIEvent.MULTI_SHOW_NEXT_BUTTON, null);

    }
}
