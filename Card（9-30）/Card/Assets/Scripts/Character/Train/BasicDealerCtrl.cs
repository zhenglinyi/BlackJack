using Protocol.Constant.Train;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDealerCtrl : CharacterBase
{

    private void Awake()
    {
        Bind(CharacterEvent.BASIC_DEALER_INIT_CARD,
            CharacterEvent.BASIC_DEALER_CLEAR_CARD,
            CharacterEvent.BASIC_CONVERT_FIRST_CARD);
    }
    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case CharacterEvent.BASIC_DEALER_INIT_CARD:
                StartCoroutine(initDealerCard(message as List<Card>));
                break;
            
            case CharacterEvent.BASIC_DEALER_CLEAR_CARD:
                clearDealerCard();
                break;
            case CharacterEvent.BASIC_CONVERT_FIRST_CARD:
                convertFirstCard();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 庄家的卡牌列表
    /// </summary>
    private List<TrainCardCtrl> cardCtrlList;

    /// <summary>
    /// 卡牌的父物体
    /// </summary>
    private Transform cardParent;

    private PromptMsg promptMsg;
    private SocketMsg socketMsg;

    // Use this for initialization
    void Start()
    {
        cardParent = transform.Find("CardPoint");
        cardCtrlList = new List<TrainCardCtrl>();

        promptMsg = new PromptMsg();
        socketMsg = new SocketMsg();
    }


    /// <summary>
    /// 初始化显示卡牌
    /// </summary>
    private IEnumerator initDealerCard(List<Card> cardList)
    {
        GameObject cardPrefab = Resources.Load<GameObject>("Card/MyCard");

        for (int i = 0; i < cardList.Count; i++)
        {
            createGo(cardList[i], i, cardPrefab);
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 创建卡牌游戏物体
    /// </summary>
    /// <param name="card"></param>
    /// <param name="index"></param>
    private void createGo(Card card, int index, GameObject cardPrefab)
    {
        GameObject cardGo = Object.Instantiate(cardPrefab, cardParent) as GameObject;
        cardGo.name = card.Name;
        cardGo.transform.localPosition = new Vector2((0.25f * index), 0);
        TrainCardCtrl cardCtrl = cardGo.GetComponent<TrainCardCtrl>();
        bool isDealerfirst;
        if (index == 0)
            isDealerfirst = true;
        else
            isDealerfirst = false;
        cardCtrl.Init(card, index, isDealerfirst);

        //存储本地
        this.cardCtrlList.Add(cardCtrl);
    }
    /// <summary>
    /// 要牌
    /// </summary>
    /// <param name="Card"></param>
    private void getDealerCard(Card Card)
    {
        GameObject cardPrefab = Resources.Load<GameObject>("Card/MyCard");
        int index = cardCtrlList.Count;
        createGo(Card, index, cardPrefab);
    }

    private void convertFirstCard()
    {
        var cardCtrl = cardCtrlList[0];
        Card Card = cardCtrl.Card;
        cardCtrl.gameObject.SetActive(true);
        cardCtrl.Init(Card, 0, false);

    }
    /// <summary>
    /// 游戏结束后清除所有的卡片
    /// </summary>
    private void clearDealerCard()
    {
        List<GameObject> itemList = new List<GameObject>();
        for (int i = 0; i < cardParent.childCount; i++)
        {
            itemList.Add(cardParent.GetChild(i).gameObject);
        }
        for (int i = 0; i < itemList.Count; i++)
        {
            DestroyImmediate(itemList[i]);
        }
        cardCtrlList.Clear();
    }
}
