using Protocol.Dto.Multi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiOverPanel : UIBase
{

	private void Awake()
	{
		Bind(UIEvent.MULTI_SHOW_OVER_PANEL);
	}

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.MULTI_SHOW_OVER_PANEL:
                refreshPanel(message as GameOverDto);
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
    private void refreshPanel(GameOverDto gameOverDto)
    {
        //显示面板
        setPanelActive(true);

        txtDealerSettle.text = "庄家 ：点数 ";
        txtDealerSettle.text += gameOverDto.dealerWeight;
        if(gameOverDto.dealerWeight>21)
        {
            txtDealerSettle.text += "爆牌";
        }

        txtPlayerSettle.text = "";
        for(int i=0;i<4;i++)
        {
            if(gameOverDto.playerWeightListList[i]!=null)
            {
                txtPlayerSettle.text += "玩家 ";
                txtPlayerSettle.text += i+1;
                txtPlayerSettle.text += "\n";
                if (gameOverDto.isLeaveList[i] == true)
                {
                    txtPlayerSettle.text += "中途离开  ";
                    txtPlayerSettle.text += gameOverDto.playerWinBeenListList[i][0];
                    txtPlayerSettle.text += "\n";
                }
                else
                {
                    for(int j=0;j< gameOverDto.playerWeightListList[i].Count;j++)
                    {
                        txtPlayerSettle.text += "点数 ";
                        //1 21点，2 爆牌，3 加倍爆牌 ，4 加倍 赢 ，5 加倍 输，6赢 ，7 输,8 平
                        switch (gameOverDto.playerStateListList[i][j])
                        {
                            
                            case 1:
                                {
                                    txtPlayerSettle.text += gameOverDto.playerWeightListList[i][j];
                                    txtPlayerSettle.text += "黑杰克  + ";
                                    txtPlayerSettle.text += gameOverDto.playerWinBeenListList[i][j];
                                    break;
                                }
                            case 2:
                                {
                                    txtPlayerSettle.text += gameOverDto.playerWeightListList[i][j];
                                    txtPlayerSettle.text += "爆牌  ";
                                    txtPlayerSettle.text += gameOverDto.playerWinBeenListList[i][j];
                                    break;
                                }
                            case 3:
                                {
                                    txtPlayerSettle.text += gameOverDto.playerWeightListList[i][j];
                                    txtPlayerSettle.text += "加倍爆牌  ";
                                    txtPlayerSettle.text += gameOverDto.playerWinBeenListList[i][j];
                                    break;
                                }
                            case 4:
                                {
                                    txtPlayerSettle.text += gameOverDto.playerWeightListList[i][j];
                                    txtPlayerSettle.text += "加倍 赢  + ";
                                    txtPlayerSettle.text += gameOverDto.playerWinBeenListList[i][j];
                                    break;
                                }
                            case 5:
                                {
                                    txtPlayerSettle.text += gameOverDto.playerWeightListList[i][j];
                                    txtPlayerSettle.text += "加倍 输  ";
                                    txtPlayerSettle.text += gameOverDto.playerWinBeenListList[i][j];
                                    break;
                                }
                            case 6:
                                {
                                    txtPlayerSettle.text += gameOverDto.playerWeightListList[i][j];
                                    txtPlayerSettle.text += "赢  +";
                                    txtPlayerSettle.text += gameOverDto.playerWinBeenListList[i][j];
                                    break;
                                }
                            case 7:
                                {
                                    txtPlayerSettle.text += gameOverDto.playerWeightListList[i][j];
                                    txtPlayerSettle.text += "输  ";
                                    txtPlayerSettle.text += gameOverDto.playerWinBeenListList[i][j];
                                    break;
                                }
                            case 8:
                                {
                                    txtPlayerSettle.text += gameOverDto.playerWeightListList[i][j];
                                    txtPlayerSettle.text += "平  + ";
                                    txtPlayerSettle.text += gameOverDto.playerWinBeenListList[i][j];
                                    break;
                                }



                        }

                        txtPlayerSettle.text += "\n";
                    }
                }
            }
        }
        
    }
    private void backClick()
    {
        setPanelActive(false);
        Dispatch(AreaCode.UI, UIEvent.MULTI_SHOW_NEXT_BUTTON, null);

    }
}
