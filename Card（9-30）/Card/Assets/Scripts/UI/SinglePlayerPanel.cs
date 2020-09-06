using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol.Dto;
using Protocol.Code;
using UnityEngine.UI;

public class SinglePlayerPanel : UIBase
{
    protected void Awake()
    {
        Bind(UIEvent.SINGLE_SHOW_GET_BUTTON,
            UIEvent.SINGLE_SHOW_NEXT_BUTTON,
            UIEvent.SINGLE_SHOW_START_BUTTON,
            UIEvent.SINGLE_SHOW_SPLIT_BUTTON,
            UIEvent.SINGLE_SHOW_NEXTSPLIT_BUTTON);

    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.SINGLE_SHOW_GET_BUTTON:
                {
                    bool active = (bool)message;
                    btnGet.gameObject.SetActive(active);
                    btnNGet.gameObject.SetActive(active);
                    btnDouble.gameObject.SetActive(active);
                    break;
                }
                
            case UIEvent.SINGLE_SHOW_NEXT_BUTTON:
                {
                    bool active = (bool)message;
                    btnNext.gameObject.SetActive(active);
                    btnLeave.gameObject.SetActive(active);
                    btnReWager.gameObject.SetActive(active);
                    break;
                }
            case UIEvent.SINGLE_SHOW_START_BUTTON:
                {
                    bool active = (bool)message;
                    btnStart.gameObject.SetActive(active);
                    btnDoWager.gameObject.SetActive(active);
                    break;
                }
            case UIEvent.SINGLE_SHOW_SPLIT_BUTTON:
                {
                    bool active = (bool)message;
                    btnSplit.gameObject.SetActive(active);
                    break;
                }
            case UIEvent.SINGLE_SHOW_NEXTSPLIT_BUTTON:
                {
                    bool active = (bool)message;
                    btnNextSplit.gameObject.SetActive(active);
                    break;
                }


            default:
                break;
        }

    }

    /// <summary>
    /// 角色的数据
    /// </summary>
    protected UserDto userDto;

    private Button btnStart;
    private Button btnNext;
    private Button btnLeave;
    private Button btnGet;
    private Button btnNGet;

    private Button btnDoWager;
    private Button btnReWager;
    private Button btnDouble;
    private Button btnSplit;
    private Button btnNextSplit;
    private Button btn10;
    private Button btn100;
    private Button btn1000;

    private Button btnCount;
    private Button btnStrategy;

    private int thisWager;



    private SocketMsg socketMsg;

    protected virtual void Start()
    {
        

        thisWager = 10;

        btnStart = transform.Find("btnStart").GetComponent<Button>();
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

        btnCount = transform.Find("btnCount").GetComponent<Button>();
        btnStrategy = transform.Find("btnStrategy").GetComponent<Button>();


        btnStart.onClick.AddListener(startClick);
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

        btnCount.onClick.AddListener(btnCountClick);
        btnStrategy.onClick.AddListener(btnStrategyClick);

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
        //fixbug923 
        //UserDto myUserDto = Models.GameModel.MatchRoomDto.UIdUserDict[Models.GameModel.UserDto.Id];
        //this.userDto = myUserDto;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        
        btnStart.onClick.RemoveAllListeners();
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

        btnCount.onClick.RemoveAllListeners();
        btnStrategy.onClick.RemoveAllListeners();
    }

    private void startClick()
    {
        /*
        btnStart.gameObject.SetActive(false);
        btnGet.gameObject.SetActive(true);
        btnNGet.gameObject.SetActive(true);
        btnDouble.gameObject.SetActive(true);
        */
        socketMsg.Change(OpCode._21Single, _21SingleCode.START_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }

    private void getClick()
    {
        //要了牌不能加倍
        btnDouble.gameObject.SetActive(false);
        btnNextSplit.gameObject.SetActive(false);
        btnSplit.gameObject.SetActive(false);
        socketMsg.Change(OpCode._21Single, _21SingleCode.GET_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }

    private void leaveClick()
    {
        socketMsg.Change(OpCode._21Single, _21SingleCode.LEAVE_CREQ, null);
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
        Dispatch(AreaCode.CHARACTER, CharacterEvent.CLEAR_PLAYER_CARD, null);
        Dispatch(AreaCode.CHARACTER, CharacterEvent.CLEAR_DEALER_CARD, null);
        
        socketMsg.Change(OpCode._21Single, _21SingleCode.NEXT_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }

    private void ngetClick()
    {
        btnNextSplit.gameObject.SetActive(false);
        btnSplit.gameObject.SetActive(false);
        socketMsg.Change(OpCode._21Single, _21SingleCode.NGET_CREQ, null);
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
        thisWager = wager;
        Dispatch(AreaCode.UI, UIEvent.SINGLE_CHANGE_WAGER, wager);
        //发到服务器
        socketMsg.Change(OpCode._21Single, _21SingleCode.SET_WAGER_CREQ, wager);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }

    /// <summary>
    /// 加倍点击
    /// </summary>
    private void btnDoubleClick()
    {
        btnNextSplit.gameObject.SetActive(false);
        btnSplit.gameObject.SetActive(false);
        //Dispatch(AreaCode.UI, UIEvent.CHANGE_MUTIPLE, 2);
        socketMsg.Change(OpCode._21Single, _21SingleCode.DOUBLE_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);
        
    }

    /// <summary>
    /// 分牌点击
    /// </summary>
    private void btnSplitClick()
    {
        btnSplit.gameObject.SetActive(false);
        Dispatch(AreaCode.CHARACTER, CharacterEvent.CLEAR_PLAYER_CARD, null);
        socketMsg.Change(OpCode._21Single, _21SingleCode.SPLIT_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);
        

    }

    /// <summary>
    /// 分牌后下一手牌
    /// </summary>
    private void btnNextSplitClick()
    {
        btnNextSplit.gameObject.SetActive(false);
        btnGet.gameObject.SetActive(true);
        btnNGet.gameObject.SetActive(true);
        btnDouble.gameObject.SetActive(true);
        Dispatch(AreaCode.CHARACTER, CharacterEvent.CLEAR_PLAYER_CARD, null);
        socketMsg.Change(OpCode._21Single, _21SingleCode.SPLIT_NEXT_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }

    /// <summary>
    /// 
    /// </summary>
    private void btnCountClick()
    {
        Dispatch(AreaCode.UI, UIEvent.COUNT_SHOW_TIP_PANEL, null);
    }

    private void btnStrategyClick()
    {
        Dispatch(AreaCode.UI, UIEvent.STRATEGY_SHOW_TIP_PANEL, null);
    }
}
