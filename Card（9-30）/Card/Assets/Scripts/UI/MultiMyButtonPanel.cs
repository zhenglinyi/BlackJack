using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiMyButtonPanel : UIBase
{
    protected void Awake()
    {
        Bind(UIEvent.MULTI_PLAYER_HIDE_READY_BUTTON,
            UIEvent.MULTI_SHOW_HSD_BUTTON,
            UIEvent.MULTI_SHOW_SPLIT_BUTTON,
            UIEvent.MULTI_SHOW_SPLIT_NEXT_BUTTON,
            UIEvent.MULTI_SHOW_NEXT_BUTTON
            );
    }

    public override void Execute(int eventCode, object message)
    {
        switch(eventCode)
        {
            case UIEvent.MULTI_PLAYER_HIDE_READY_BUTTON:
                {
                    btnReady.gameObject.SetActive(false);
                    btnDoWager.gameObject.SetActive(false);
                    break;
                }
                
            case UIEvent.MULTI_SHOW_HSD_BUTTON:
                {
                    bool active = (bool)message;
                    btnGet.gameObject.SetActive(active);
                    btnNGet.gameObject.SetActive(active);
                    btnDouble.gameObject.SetActive(active);
                    break;
                }
                
            case UIEvent.MULTI_SHOW_SPLIT_BUTTON:
                {
                    bool active = (bool)message;
                    btnSplit.gameObject.SetActive(active);
                    break;
                }
            case UIEvent.MULTI_SHOW_SPLIT_NEXT_BUTTON:
                {
                    bool active = (bool)message;
                    btnNextSplit.gameObject.SetActive(active);
                    break;
                }
            case UIEvent.MULTI_SHOW_NEXT_BUTTON:
                {
                    //bool active = (bool)message;
                    btnLeave.gameObject.SetActive(true);
                    btnNext.gameObject.SetActive(true);

                    
                    btnGet.gameObject.SetActive(false);
                    btnNGet.gameObject.SetActive(false);

                    btnDouble.gameObject.SetActive(false);
                    btnSplit.gameObject.SetActive(false);
                    btnNextSplit.gameObject.SetActive(false);
                   


                    break;
                }



            default:
                break;
        }
    }

    private Button btnReady;
    private Button btnDoWager;

    private Button btnNext;
    private Button btnLeave;
    private Button btnGet;
    private Button btnNGet;

    
    private Button btnReWager;
    private Button btnDouble;
    private Button btnSplit;
    private Button btnNextSplit;

    private Button btn10;
    private Button btn100;
    private Button btn1000;

    private SocketMsg socketMsg;

    protected virtual void Start()
    {
        btnReady = transform.Find("btnReady").GetComponent<Button>();
        btnNext = transform.Find("btnNext").GetComponent<Button>();
        btnLeave = transform.Find("btnLeave").GetComponent<Button>();
        btnGet = transform.Find("btnGet").GetComponent<Button>();
        btnNGet = transform.Find("btnNGet").GetComponent<Button>();

        btnDoWager = transform.Find("btnDoWager").GetComponent<Button>();
        btnReWager = transform.Find("btnReWager").GetComponent<Button>();
        btnDouble = transform.Find("btnDouble").GetComponent<Button>();
        btnSplit = transform.Find("btnSplit").GetComponent<Button>();
        btnNextSplit = transform.Find("btnNextSplit").GetComponent<Button>();

        btn10 = transform.Find("btn10").GetComponent<Button>();
        btn100 = transform.Find("btn100").GetComponent<Button>();
        btn1000 = transform.Find("btn1000").GetComponent<Button>();

        btnReady.onClick.AddListener(readyClick);
        btnNext.onClick.AddListener(nextClick);
        btnGet.onClick.AddListener(getClick);
        btnLeave.onClick.AddListener(leaveClick);
        btnNGet.onClick.AddListener(ngetClick);

        btnDoWager.onClick.AddListener(changeWagerClick);
        btnReWager.onClick.AddListener(changeWagerClick);

        btn10.onClick.AddListener(btn10Click);
        btn100.onClick.AddListener(btn100Click);
        btn1000.onClick.AddListener(btn1000Click);

        btnDouble.onClick.AddListener(btnDoubleClick);
        btnSplit.onClick.AddListener(btnSplitClick);
        btnNextSplit.onClick.AddListener(btnNextSplitClick);

        socketMsg = new SocketMsg();


        //默认状态
        btnNext.gameObject.SetActive(false);
        btnLeave.gameObject.SetActive(false);
        btnGet.gameObject.SetActive(false);
        btnNGet.gameObject.SetActive(false);

        btnReWager.gameObject.SetActive(false);
        btnDouble.gameObject.SetActive(false);
        btnSplit.gameObject.SetActive(false);
        btnNextSplit.gameObject.SetActive(false);
        btn10.gameObject.SetActive(false);
        btn100.gameObject.SetActive(false);
        btn1000.gameObject.SetActive(false);
        
    }

    public override void OnDestroy()
    {
        base.OnDestroy();


        btnReady.onClick.RemoveAllListeners();
        btnNext.onClick.RemoveAllListeners();
        btnLeave.onClick.RemoveAllListeners();
        btnGet.onClick.RemoveAllListeners();
        btnNGet.onClick.RemoveAllListeners();

        btnDoWager.onClick.RemoveAllListeners();
        btnReWager.onClick.RemoveAllListeners();
        btn10.onClick.RemoveAllListeners();
        btn100.onClick.RemoveAllListeners();
        btn1000.onClick.RemoveAllListeners();
        btnDouble.onClick.RemoveAllListeners();
        btnSplit.onClick.RemoveAllListeners();
        btnNextSplit.onClick.RemoveAllListeners();
    }

    private void readyClick()
    {
        
        socketMsg.Change(OpCode._21Multi, _21MultiCode.READY_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);
        //321动画
        //Dispatch(AreaCode.UI, UIEvent.SHOW_COUNT_DOWN_PANEL, null);
    }

    private void getClick()
    {
        Dispatch(AreaCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "card_deal");
        btnDouble.gameObject.SetActive(false);
        btnSplit.gameObject.SetActive(false);
        btnNextSplit.gameObject.SetActive(false);
        socketMsg.Change(OpCode._21Multi, _21MultiCode.HIT_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }

    private void leaveClick()
    {
        socketMsg.Change(OpCode._21Multi, _21MultiCode.LEAVE_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);
        LoadSceneMsg msg = new LoadSceneMsg(1,
                 delegate ()
                 {
                     //向服务器获取信息
                     SocketMsg socketMsg = new SocketMsg(OpCode.USER, UserCode.GET_INFO_CREQ, null);
                     Dispatch(AreaCode.NET, 0, socketMsg);
                     //Debug.Log("加载完成！");
                 });
        Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, msg);
    }

    private void nextClick()
    {
        btnDoWager.gameObject.SetActive(true);
        btnReady.gameObject.SetActive(true);
        btnNext.gameObject.SetActive(false);
        btnLeave.gameObject.SetActive(false);
        Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_CLEAR_DEALER_CARD, null);
        Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_CLEAR_PLAYER1_CARD, null);
        Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_CLEAR_PLAYER2_CARD, null);
        Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_CLEAR_PLAYER3_CARD, null);
        Dispatch(AreaCode.CHARACTER, CharacterEvent.MULTI_CLEAR_PLAYER4_CARD, null);

        
    }

    private void ngetClick()
    {
        btnSplit.gameObject.SetActive(false);
        socketMsg.Change(OpCode._21Multi, _21MultiCode.STAND_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);
        

    }

    /// <summary>
    /// 点击下注按钮
    /// </summary>
    private void changeWagerClick()
    {
        showWager3Button(true);
    }
    /// <summary>
    /// 是否显示三个下注按钮
    /// </summary>
    /// <param name="active"></param>
    private void showWager3Button(bool active)
    {
        btn10.gameObject.SetActive(active);
        btn100.gameObject.SetActive(active);
        btn1000.gameObject.SetActive(active);
    }
    /// <summary>
    /// 下注10点击
    /// </summary>
    private void btn10Click()
    {
        showWager3Button(false);
        changeWager(10);
    }
    private void btn100Click()
    {
        showWager3Button(false);
        changeWager(100);
    }
    private void btn1000Click()
    {
        showWager3Button(false);
        changeWager(1000);
    }
    /// <summary>
    /// 改变赌注
    /// </summary>
    /// <param name="wager"></param>
    private void changeWager(int wager)
    {
        //thisWager = wager;
        Dispatch(AreaCode.UI, UIEvent.MULTI_CHANGE_WAGER, wager);
        //发到服务器
        socketMsg.Change(OpCode._21Multi, _21MultiCode.SET_WAGER_CREQ, wager);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }

    /// <summary>
    /// 加倍点击
    /// </summary>
    private void btnDoubleClick()
    {
        Dispatch(AreaCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "card_deal");
        btnDouble.gameObject.SetActive(false);
        btnSplit.gameObject.SetActive(false);
        btnNextSplit.gameObject.SetActive(false);
        socketMsg.Change(OpCode._21Multi, _21MultiCode.DOUBLE_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);


    }

    /// <summary>
    /// 分牌点击
    /// </summary>
    private void btnSplitClick()
    {
        Dispatch(AreaCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "card_deal");
        btnSplit.gameObject.SetActive(false);
        socketMsg.Change(OpCode._21Multi, _21MultiCode.SPLIT_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);


    }

    /// <summary>
    /// 分牌后下一手牌
    /// </summary>
    private void btnNextSplitClick()
    {
        Dispatch(AreaCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "card_deal");
        btnNextSplit.gameObject.SetActive(false);
        socketMsg.Change(OpCode._21Multi, _21MultiCode.SPLIT_NEXT_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }
}
