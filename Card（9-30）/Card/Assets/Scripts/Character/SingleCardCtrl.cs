using Protocol.Dto.Single;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCardCtrl : MonoBehaviour 
{

    //数据
    public CardDto CardDto { get; private set; }

    private SpriteRenderer spriteRenderer;

    private bool isDealerFirst;

    /// <summary>
    /// 初始化卡牌数据
    /// </summary>
    /// <param name="card"></param>
    /// <param name="index"></param>
    /// <param name="isMine"></param>
    public void Init(CardDto card, int index, bool isDealerFirst)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.CardDto = card;
        this.isDealerFirst = isDealerFirst;
        string resPath = string.Empty;
        if (!isDealerFirst)
        {
            resPath = "Poker/" + card.Name;
        }
        else
        {
            resPath = "Poker/CardBack";
        }
        Sprite sp = Resources.Load<Sprite>(resPath);
        spriteRenderer.sprite = sp;
        spriteRenderer.sortingOrder = index;
    }
}
