using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchPanel : UIBase
{

    private void Awake()
    {
        Bind(UIEvent.MULTI_SHOW_ENTER_ROOM_BUTTON);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.MULTI_SHOW_ENTER_ROOM_BUTTON:
                btnEnter.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    private Button btnMatch;//单人模式
    private Button btnMatch2;//多人模式
    private Button btnTrain1;//流水数训练
    private Button btnTrain2;//基本策略训练
    //private Button btnTrain3;//实战训练

    private Image imgBg;
    private Text txtDes;
    private Button btnCancel;
    private Button btnEnter;

    private SocketMsg socketMsg;

    // Use this for initialization
    void Start()
    {
        btnMatch = transform.Find("btnMatch").GetComponent<Button>();
        btnMatch2 = transform.Find("btnMatch2").GetComponent<Button>();
        btnTrain1=transform.Find("btnTrain1").GetComponent<Button>();
        btnTrain2 = transform.Find("btnTrain2").GetComponent<Button>();
        //btnTrain3 = transform.Find("btnTrain3").GetComponent<Button>();
        imgBg = transform.Find("imgBg").GetComponent<Image>();
        txtDes = transform.Find("txtDes").GetComponent<Text>();
        btnCancel = transform.Find("btnCancel").GetComponent<Button>();
        btnEnter = transform.Find("btnEnter").GetComponent<Button>();

        btnMatch.onClick.AddListener(matchClick);
        btnCancel.onClick.AddListener(cancelClick);
        btnEnter.onClick.AddListener(enterClick);
        btnMatch2.onClick.AddListener(multiMatchClick);
        btnTrain1.onClick.AddListener(runCountClick);
        btnTrain2.onClick.AddListener(basicStrategyClick);

        socketMsg = new SocketMsg();

        //默认状态
        setObjectsActive(false);
        btnEnter.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (txtDes.gameObject.activeInHierarchy == false)
            return;

        timer += Time.deltaTime;
        if (timer >= intervalTime)
        {
            doAnimation();
            timer = 0f;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        btnMatch.onClick.RemoveListener(matchClick);
        btnCancel.onClick.RemoveListener(cancelClick);
        btnEnter.onClick.RemoveListener(enterClick);
    }

    private void enterClick()
    {
        //fix bug
        //Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, 2);
        socketMsg.Change(OpCode._21Multi, _21MultiCode.ENTER_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);
        LoadSceneMsg msg = new LoadSceneMsg(4,
                 delegate ()
                 {
                     Debug.Log("4加载完成！");
                     //Dispatch(AreaCode.UI, UIEvent.MULTI_PLAYER_CHANGE_IDENTITY, Models.GameModel.UserDto.Id);
                 });
        Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, msg);
        

    }

    private void matchClick()
    {
        //向服务器发起开始匹配的请求
        //  可以怎么做，假如客户端保存了自身的账号id的话，发消息的时候，都把自身的id发送给服务器
        LoadSceneMsg msg = new LoadSceneMsg(3,
                 delegate ()
                 {
                    
                     Debug.Log("3加载完成！");
                 });
        Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, msg);
        socketMsg.Change(OpCode._21Single, _21SingleCode.MATCH_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);
        /*
        socketMsg.Change(OpCode.MATCH, MatchCode.ENTER_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);

        setObjectsActive(true);

        //按钮隐藏掉
        this.btnMatch.interactable = (false);*/
    }

    private void multiMatchClick()
    {
        socketMsg.Change(OpCode._21Multi, _21MultiCode.MATCH_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);

        setObjectsActive(true);

        //按钮隐藏掉
        this.btnMatch.interactable = false;
        this.btnMatch2.interactable = false;
        this.btnTrain1.interactable = false;
        this.btnTrain2.interactable = false;
        //this.btnTrain3.interactable = false;
    }

    private void cancelClick()
    {
        //向服务器发起离开匹配的请求
        socketMsg.Change(OpCode._21Multi, _21MultiCode.LEAVE_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);

        setObjectsActive(false);

        //按钮隐显示
        this.btnMatch.interactable = true;
        this.btnMatch2.interactable = true;
        this.btnTrain1.interactable = true;
        this.btnTrain2.interactable = true;
        //this.btnTrain3.interactable = true;
    }

    /// <summary>
    /// 流水训练
    /// </summary>
    public void runCountClick()
    {
        LoadSceneMsg msg = new LoadSceneMsg(5,
                 delegate ()
                 {

                     Debug.Log("5加载完成！");
                 });
        Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, msg);

    }
    public void basicStrategyClick()
    {
        LoadSceneMsg msg = new LoadSceneMsg(6,
                 delegate ()
                 {

                     Debug.Log("6加载完成！");
                 });
        Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, msg);
    }

    /// <summary>
    /// 控制点击匹配按钮之后的显示的物体
    /// </summary>
    private void setObjectsActive(bool active)
    {
        imgBg.gameObject.SetActive(active);
        txtDes.gameObject.SetActive(active);
        btnCancel.gameObject.SetActive(active);
        btnEnter.gameObject.SetActive(active);
    }

    private string defaultText = "正在寻找房间";
    //点的数量
    private int dotCount = 0;
    //动画的间隔时间
    private float intervalTime = 1f;
    //计时器
    private float timer = 0f;

    /// <summary>
    /// 做动画
    /// </summary>
    private void doAnimation()
    {
        txtDes.text = defaultText;

        //点增加
        dotCount++;
        if (dotCount > 5)
            dotCount = 1;

        for (int i = 0; i < dotCount; i++)
        {
            txtDes.text += ".";
        }
    }

}
