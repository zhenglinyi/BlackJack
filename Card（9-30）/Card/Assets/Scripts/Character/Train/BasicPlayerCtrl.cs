using Protocol.Constant.Train;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPlayerCtrl : CharacterBase
{

    private void Awake()
    {
        Bind(CharacterEvent.BASIC_PLAYER_INIT_CARD,

            CharacterEvent.BASIC_PLAYER_CLEAR_CARD);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case CharacterEvent.BASIC_PLAYER_INIT_CARD:
                StartCoroutine(initPlayerCard(message as List<Card>));
                break;

            case CharacterEvent.BASIC_PLAYER_CLEAR_CARD:
                clearPlayerCard();
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 自身管理的卡牌列表
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
    private IEnumerator initPlayerCard(List<Card> cardList)
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
        cardCtrl.Init(card, index, false);

        //存储本地
        this.cardCtrlList.Add(cardCtrl);
    }
    /// <summary>
    /// 要牌
    /// </summary>
    /// <param name="Card"></param>
    private void getPlayerCard(Card Card)
    {
        GameObject cardPrefab = Resources.Load<GameObject>("Card/MyCard");
        int index = cardCtrlList.Count;
        createGo(Card, index, cardPrefab);
    }

    private void clearPlayerCard()
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
