using Protocol.Code;
using Protocol.Constant;
using Protocol.Constant.Train;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class RunStatePanel : UIBase
{
	private void Awake()
	{
		Bind(UIEvent.RUN_SHOW_NEXT_BUTTON);
	}

	public override void Execute(int eventCode, object message)
	{
		switch (eventCode)
		{
			case UIEvent.RUN_SHOW_NEXT_BUTTON:
				{
					btnExit.gameObject.SetActive(true);
					btnNext.gameObject.SetActive(true);
					btnOK.gameObject.SetActive(false);
					inputRC.gameObject.SetActive(false);
					break;
				}
				
			default:
				break;
		}
	}

	private InputField inputRC;
	private Text txtRC;
	private Text txtResult;
	private Button btnOK;
	private Button btnNext;
	private Button btnStart;
	private Button btnExit;
	private Button btnTip;
	TrainLibraryModel libraryModel;
	List<Card> cardList;
	int count;

	void Start()
	{
		inputRC = transform.Find("inputRC").GetComponent<InputField>();
		txtRC = transform.Find("txtRC").GetComponent<Text>();
		txtResult = transform.Find("txtResult").GetComponent<Text>();

		btnOK = transform.Find("btnOK").GetComponent<Button>();
		btnNext = transform.Find("btnNext").GetComponent<Button>();
		btnStart = transform.Find("btnStart").GetComponent<Button>();
		btnExit = transform.Find("btnExit").GetComponent<Button>();
		btnTip = transform.Find("btnTip").GetComponent<Button>();

		inputRC.gameObject.SetActive(false);
		txtRC.text = "RC = 0";
		txtResult.gameObject.SetActive(false);
		btnOK.gameObject.SetActive(false);
		btnNext.gameObject.SetActive(false);
		

		btnOK.onClick.AddListener(okClick);
		btnNext.onClick.AddListener(nextClick);
		btnStart.onClick.AddListener(startClick);
		btnExit.onClick.AddListener(exitClick);
		btnTip.onClick.AddListener(tipClick);

		libraryModel = new TrainLibraryModel();
		count = 0;
		




	}

	public override void OnDestroy()
	{
		base.OnDestroy();

		btnOK.onClick.RemoveAllListeners();
		btnNext.onClick.RemoveAllListeners();
		btnStart.onClick.RemoveAllListeners();
		btnExit.onClick.RemoveAllListeners();
		btnTip.onClick.RemoveAllListeners();
	}

	/// <summary>
	/// 离开
	/// </summary>
	public void exitClick()
	{
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
	/// <summary>
	/// 开始
	/// </summary>
	public void startClick()
	{
		inputRC.gameObject.SetActive(true);
		btnOK.gameObject.SetActive(true);
		btnStart.gameObject.SetActive(false);
		btnExit.gameObject.SetActive(false);
		//发牌
		cardList = new List<Card>();
		System.Random r = new System.Random();
		int num = r.Next(4, 11);

		cardList = libraryModel.MultiDeal(num);
		Dispatch(AreaCode.CHARACTER, CharacterEvent.RUNCOUNT_INIT_CARD, cardList);

	}

	public int runningCount()
	{
		return TrainCardWeight21.getRunningCount(cardList);
	}
	/// <summary>
	/// 确定
	/// </summary>
	public void okClick()
	{
		if(count+ runningCount() == Convert.ToInt32(inputRC.text))
		{
			txtResult.gameObject.SetActive(true);
			txtResult.text = "正 确";
			txtRC.text = "RC = " + (count + runningCount());
			count += runningCount();
			btnOK.gameObject.SetActive(false);
			inputRC.gameObject.SetActive(false);
			btnNext.gameObject.SetActive(true);
			btnExit.gameObject.SetActive(true);
		}
		else
		{
			txtResult.gameObject.SetActive(true);
			txtResult.text = "错 误";
			txtRC.text = "RC = " + (count + runningCount());

			//错误面板弹出
			RunError re = new RunError();
			re.lastCount = count;
			re.nowCount = count + runningCount();
			re.cardList = cardList;
			count += runningCount();

			Dispatch(AreaCode.UI, UIEvent.RUN_SHOW_ERROR_PANEL, re);

		}

	}


	public void nextClick()
	{
		inputRC.gameObject.SetActive(true);
		btnOK.gameObject.SetActive(true);
		btnNext.gameObject.SetActive(false);
		btnExit.gameObject.SetActive(false);
		Dispatch(AreaCode.CHARACTER, CharacterEvent.RUNCOUNT_CLEAR_CARD, null);
		if(libraryModel.RemainCardNum()<30)
		{
			libraryModel.Init();
		}
		//发牌
		cardList = new List<Card>();
		System.Random r = new System.Random();
		int num = r.Next(4, 11);

		cardList = libraryModel.MultiDeal(num);
		Dispatch(AreaCode.CHARACTER, CharacterEvent.RUNCOUNT_INIT_CARD, cardList);

	}
	/// <summary>
	/// 提示
	/// </summary>
	public void tipClick()
	{
		
		Dispatch(AreaCode.UI, UIEvent.RUN_SHOW_TIP_PANEL, null);
	}

}
