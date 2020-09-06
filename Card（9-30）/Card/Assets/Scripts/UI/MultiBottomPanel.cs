using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiBottomPanel : UIBase
{

    private void Awake()
    {
        Bind(UIEvent.MULTI_CHANGE_BEEN,
            UIEvent.MULTI_CHANGE_WAGER
            );
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.MULTI_CHANGE_BEEN:
                changeBeen((int)message);
                break;
            case UIEvent.MULTI_CHANGE_WAGER:
                changeWager((int)message);
                break;
            
            default:
                break;
        }
    }

    private Text txtBeen;
    private Text txtWager;
    //private Text txtMutiple;

    private SocketMsg socketMsg;

    private void Start()
    {
        txtBeen = transform.Find("txtBeen").GetComponent<Text>();
        txtWager = transform.Find("txtWager").GetComponent<Text>();
        //txtMutiple = transform.Find("txtMutiple").GetComponent<Text>();
        socketMsg = new SocketMsg();

        refreshPanel(Models.GameModel.UserDto.Been);
    }
    /// <summary>
    /// 刷新自身面板的信息
    /// </summary>
    private void refreshPanel(int beenCount)
    {
        this.txtBeen.text = "× " + beenCount;
    }

    /// <summary>
    /// 改变下注
    /// </summary>
    /// <param name="muti"></param>
    private void changeWager(int wager)
    {
        txtWager.text = "下注 × " + wager;
    }
    /// <summary>
    /// 改变豆子
    /// </summary>
    /// <param name="muti"></param>
    private void changeBeen(int beenCount)
    {
        txtBeen.text = "× " + beenCount;
    }

   
}
