using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol.Constant;
using Protocol.Constant.Train;
using Protocol.Code;

public class BasicStatePanel : UIBase
{

	private void Awake()
	{
		Bind(UIEvent.BASIC_SHOW_NEXT_BUTTON);
	}

	public override void Execute(int eventCode, object message)
	{
		switch (eventCode)
		{
			case UIEvent.BASIC_SHOW_NEXT_BUTTON:
				{
					btnExit.gameObject.SetActive(true);
					btnNext.gameObject.SetActive(true);
					setHSDPButton(false);
					break;
				}

			default:
				break;
		}
	}

	private Text txtDealer;
	private Text txtPlayer;
	private Text txtResult;

	private Button btnHit;
	private Button btnStand;
	private Button btnDouble;
	private Button btnSplit;

	private Button btnNext;
	private Button btnStart;
	private Button btnExit;
	private Button btnTip;

	TrainLibraryModel libraryModel;
	List<Card> playerCardList;
	List<Card> dealerCardList;

	private Hashtable hashtableX = null;
	private Hashtable hashtableY = null;
	private string[,] Matrix = null;

	private string dealerCardType;
	private string playerCardType;
	void Start()
	{
		txtDealer = transform.Find("txtDealer").GetComponent<Text>();
		txtPlayer = transform.Find("txtPlayer").GetComponent<Text>();
		txtResult = transform.Find("txtResult").GetComponent<Text>();

		btnHit = transform.Find("btnHit").GetComponent<Button>();
		btnStand = transform.Find("btnStand").GetComponent<Button>();
		btnDouble = transform.Find("btnDouble").GetComponent<Button>();
		btnSplit = transform.Find("btnSplit").GetComponent<Button>();

		btnNext = transform.Find("btnNext").GetComponent<Button>();
		btnStart = transform.Find("btnStart").GetComponent<Button>();
		btnExit = transform.Find("btnExit").GetComponent<Button>();
		btnTip = transform.Find("btnTip").GetComponent<Button>();

		
		
		txtResult.gameObject.SetActive(false);
		btnHit.gameObject.SetActive(false);
		btnStand.gameObject.SetActive(false);
		btnDouble.gameObject.SetActive(false);
		btnSplit.gameObject.SetActive(false);

		btnNext.gameObject.SetActive(false);


		btnHit.onClick.AddListener(hitClick);
		btnStand.onClick.AddListener(standClick);
		btnDouble.onClick.AddListener(doubleClick);
		btnSplit.onClick.AddListener(splitClick);

		btnNext.onClick.AddListener(nextClick);
		btnStart.onClick.AddListener(startClick);
		btnExit.onClick.AddListener(exitClick);
		btnTip.onClick.AddListener(tipClick);

		libraryModel = new TrainLibraryModel();

		hashtableX = GetHtX();
		hashtableY = GetHtY();
		Matrix = GetMatrix();

		dealerCardType = "";
		playerCardType = "";






	}

	public override void OnDestroy()
	{
		base.OnDestroy();

		btnHit.onClick.RemoveAllListeners();
		btnStand.onClick.RemoveAllListeners();
		btnDouble.onClick.RemoveAllListeners();
		btnSplit.onClick.RemoveAllListeners();
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
	private void startClick()
	{
		btnStart.gameObject.SetActive(false);
		btnExit.gameObject.SetActive(false);
		setHSDPButton(true);
		playerCardList = new List<Card>();
		dealerCardList = new List<Card>();
		do
		{
			playerCardList = libraryModel.MultiDeal(2);
		} while (TrainCardWeight21.GetWeight(playerCardList) == 21);//是21点重新发牌

		dealerCardList = libraryModel.MultiDeal(2);

		setPlayerCardType(playerCardList);
		setDealerCardType(dealerCardList);


		//初始化显示牌
		Dispatch(AreaCode.CHARACTER, CharacterEvent.BASIC_DEALER_INIT_CARD, dealerCardList);
		Dispatch(AreaCode.CHARACTER, CharacterEvent.BASIC_PLAYER_INIT_CARD, playerCardList);




	}

	/// <summary>
	/// 判断闲家牌的类型
	/// </summary>
	/// <param name="cardList"></param>
	private void setPlayerCardType(List<Card> cardList)
	{
		playerCardType = "";
		if(cardList[0].Weight==cardList[1].Weight)//对子
		{
			if (cardList[0].Weight == 1)
			{
				playerCardType += "AA";

			}
			else if (cardList[0].Weight >= 10)
			{
				playerCardType += "TT";
			}
			else
			{
				playerCardType += cardList[0].Weight.ToString();
				playerCardType += cardList[0].Weight.ToString();
			}

		}
		else if(cardList[0].Weight==1|| cardList[1].Weight==1)
		{
			playerCardType += "A";
			List<Card> tempList = new List<Card>();
			if (cardList[0].Weight == 1)
			{
				tempList.Add(cardList[1]);
				playerCardType += TrainCardWeight21.GetWeight(tempList).ToString(); 
				
			}
			else
			{
				tempList.Add(cardList[0]);
				playerCardType += TrainCardWeight21.GetWeight(tempList).ToString();
			}
		}
		else
		{
			playerCardType += "H";
			playerCardType += TrainCardWeight21.GetWeight(cardList).ToString();

		}
		txtPlayer.text = "闲家 " + playerCardType;

	}

	private void setDealerCardType(List<Card> cardList)
	{
		
		dealerCardType = "";
		int w = cardList[1].Weight;
		if(w>=10)
		{
			dealerCardType += "10";
		}
		else
		{
			dealerCardType += w.ToString();
		}
		

		txtDealer.text = "庄家 " + dealerCardType;
	}
	
	/// <summary>
	/// 设置四个按钮
	/// </summary>
	/// <param name="active"></param>
	private void setHSDPButton(bool active)
	{
		btnHit.gameObject.SetActive(active);
		btnStand.gameObject.SetActive(active);
		btnSplit.gameObject.SetActive(active);
		btnDouble.gameObject.SetActive(active);
	}

	public void tipClick()
	{

		Dispatch(AreaCode.UI, UIEvent.BASIC_SHOW_TIP_PANEL, null);
	}

	/// <summary>
	/// 生成哈希对应表  dealer
	/// </summary>
	/// <returns></returns>
	private Hashtable GetHtX()
	{
		//策略数组X轴对应表，X表示列数
		Hashtable htX = new Hashtable();

		//表示策略数组对应的X轴取值
		htX.Add("2", 0);
		htX.Add("3", 1);
		htX.Add("4", 2);
		htX.Add("5", 3);
		htX.Add("6", 4);
		htX.Add("7", 5);
		htX.Add("8", 6);
		htX.Add("9", 7);
		htX.Add("10", 8); //表示 10,J,Q,K
		htX.Add("1", 9);  //表示 A

		return htX;
	}

	/// <summary>
	/// 生成哈希对应表  player
	/// </summary>
	/// <returns></returns>
	private Hashtable GetHtY()
	{
		//策略数组Y轴对应表，Y表示行数
		Hashtable htY = new Hashtable();

		//表示策略数组对应的Y轴取值
		

		htY.Add("H20", 0);
		htY.Add("H19", 1);
		htY.Add("H18", 2);
		htY.Add("H17", 3);
		htY.Add("H16", 4);
		htY.Add("H15", 5);
		htY.Add("H14", 6);
		htY.Add("H13", 7);
		htY.Add("H12", 8);
		htY.Add("H11", 9);
		htY.Add("H10", 10);
		htY.Add("H9", 11);
		htY.Add("H8", 12);
		htY.Add("H7", 13);
		htY.Add("H6", 14);
		htY.Add("H5", 15);

		htY.Add("A9", 16);
		htY.Add("A8", 17);
		htY.Add("A7", 18);
		htY.Add("A6", 19);
		htY.Add("A5", 20);
		htY.Add("A4", 21);
		htY.Add("A3", 22);
		htY.Add("A2", 23);

		htY.Add("AA", 24);
		htY.Add("TT", 25);
		htY.Add("99", 26);
		htY.Add("88", 27);
		htY.Add("77", 28);
		htY.Add("66", 29);
		htY.Add("55", 30);
		htY.Add("44", 31);
		htY.Add("33", 32);
		htY.Add("22", 33);

		return htY;
	}

	/// <summary>
	/// 获得策略数组1, A17Stop 标准策略，no surrender/stop at soft 17
	/// </summary>
	/// <returns></returns>
	private string[,] GetMatrix()
	{
		string[,] multiArrayA = new string[34, 10] {
			{"P","P","P","P","P","P","H","H","H","H"},//表示第0行开始，X=0Y=0至X=9Y=29
            {"P","P","P","P","P","P","H","H","H","H"},
			{"H","H","H","P","P","P","H","H","H","H"},
			{"D","D","D","D","D","D","D","D","H","H"},
			{"P","P","P","P","P","H","H","H","H","H"},
			{"P","P","P","P","P","P","H","H","H","H"},
			{"P","P","P","P","P","P","P","P","P","P"},
			{"P","P","P","P","P","S","P","P","S","S"},
			{"S","S","S","S","S","S","S","S","S","S"},
			{"P","P","P","P","P","P","P","P","P","P"},
			{"H","H","H","D","D","H","H","H","H","H"},
			{"H","H","H","D","D","H","H","H","H","H"},
			{"H","H","D","D","D","H","H","H","H","H"},
			{"H","H","D","D","D","H","H","H","H","H"},
			{"H","D","D","D","D","H","H","H","H","H"},
			{"S","Ds","Ds","Ds","Ds","S","S","H","H","H"},
			{"S","S","S","S","S","S","S","S","S","S"},
			{"S","S","S","S","S","S","S","S","S","S"},
			{"H","H","H","H","H","H","H","H","H","H"},
			{"H","H","H","H","H","H","H","H","H","H"},
			{"H","D","D","D","D","H","H","H","H","H"},
			{"D","D","D","D","D","D","D","D","H","H"},
			{"D","D","D","D","D","D","D","D","D","H"},
			{"H","H","S","S","S","H","H","H","H","H"},
			{"S","S","S","S","S","H","H","H","H","H"},
			{"S","S","S","S","S","H","H","H","H","H"},
			{"S","S","S","S","S","H","H","H","H","H"},
			{"S","S","S","S","S","H","H","H","H","H"},
			{"S","S","S","S","S","S","S","S","S","S"},
			{"S","S","S","S","S","S","S","S","S","S"},
			{"S","S","S","S","S","H","H","H","H","H"},
			{"S","S","S","S","S","H","H","H","H","H"},
			{"S","S","S","S","S","S","S","S","S","S"},
			{"S","S","S","S","S","S","S","S","S","S"}
			};

		string[,] multiArray = new string[34, 10] {
			{"S","S","S","S","S","S","S","S","S","S"},
			{"S","S","S","S","S","S","S","S","S","S"},
			{"S","S","S","S","S","S","S","S","S","S"},
			{"S","S","S","S","S","S","S","S","S","S"},

			{"S","S","S","S","S","H","H","H","H","H"},
			{"S","S","S","S","S","H","H","H","H","H"},
			{"S","S","S","S","S","H","H","H","H","H"},
			{"S","S","S","S","S","H","H","H","H","H"},

			{"H","H","S","S","S","H","H","H","H","H"},
			{"D","D","D","D","D","D","D","D","D","D"},//
			{"D","D","D","D","D","D","D","D","H","H"},
			{"H","D","D","D","D","H","H","H","H","H"},
			{"H","H","H","H","H","H","H","H","H","H"},
			{"H","H","H","H","H","H","H","H","H","H"},
			{"H","H","H","H","H","H","H","H","H","H"},
			{"H","H","H","H","H","H","H","H","H","H"},

			{"S","S","S","S","S","S","S","S","S","S"},
			{"S","S","S","S","D","S","S","S","S","S"},
			{"D","D","D","D","D","S","S","H","H","H"},
			{"H","D","D","D","D","H","H","H","H","H"},
			{"H","H","D","D","D","H","H","H","H","H"},
			{"H","H","D","D","D","H","H","H","H","H"},
			{"H","H","H","D","D","H","H","H","H","H"},
			{"H","H","H","D","D","H","H","H","H","H"},
			{"P","P","P","P","P","P","P","P","P","P"},
			{"S","S","S","S","S","S","S","S","S","S"},
			{"P","P","P","P","P","S","P","P","S","S"},
			{"P","P","P","P","P","P","P","P","P","P"},
			{"P","P","P","P","P","P","H","H","H","H"},
			{"P","P","P","P","P","H","H","H","H","H"},
			{"D","D","D","D","D","D","D","D","H","H"},
			{"H","H","H","P","P","P","H","H","H","H"},
			{"P","P","P","P","P","P","H","H","H","H"},
			{"P","P","P","P","P","P","H","H","H","H"}

			};

		return multiArray;
	}

	private void hitClick()
	{
		string select = "H";
		judge(select);
		setHSDPButton(false);
		btnExit.gameObject.SetActive(true);
		btnNext.gameObject.SetActive(true);

	}
	private void standClick()
	{
		string select = "S";
		judge(select);
		setHSDPButton(false);
		btnExit.gameObject.SetActive(true);
		btnNext.gameObject.SetActive(true);
	}
	private void doubleClick()
	{
		string select = "D";
		judge(select);
		setHSDPButton(false);
		btnExit.gameObject.SetActive(true);
		btnNext.gameObject.SetActive(true);

	}
	private void splitClick()
	{
		string select = "P";
		judge(select);
		setHSDPButton(false);
		btnExit.gameObject.SetActive(true);
		btnNext.gameObject.SetActive(true);

	}

	private void judge(string select)
	{
		int intX = (int)hashtableX[dealerCardType];
		int intY = (int)hashtableY[playerCardType];

		string realAns = Matrix[intY, intX];
		txtResult.gameObject.SetActive(true);
		if(select== realAns)
		{
			txtResult.text = "正 确";
		}
		else
		{
			txtResult.text = "错 误";
		}
	}

	private void nextClick()
	{
		btnNext.gameObject.SetActive(false);
		btnExit.gameObject.SetActive(false);
		txtResult.gameObject.SetActive(false);
		setHSDPButton(true);
		Dispatch(AreaCode.CHARACTER, CharacterEvent.BASIC_DEALER_CLEAR_CARD, null);
		Dispatch(AreaCode.CHARACTER, CharacterEvent.BASIC_PLAYER_CLEAR_CARD, null);
		playerCardList = new List<Card>();
		dealerCardList = new List<Card>();
		do
		{
			playerCardList = libraryModel.MultiDeal(2);
		} while (TrainCardWeight21.GetWeight(playerCardList) == 21);//是21点重新发牌

		dealerCardList = libraryModel.MultiDeal(2);

		setPlayerCardType(playerCardList);
		setDealerCardType(dealerCardList);


		//初始化显示牌
		Dispatch(AreaCode.CHARACTER, CharacterEvent.BASIC_DEALER_INIT_CARD, dealerCardList);
		Dispatch(AreaCode.CHARACTER, CharacterEvent.BASIC_PLAYER_INIT_CARD, playerCardList);
	}

}
