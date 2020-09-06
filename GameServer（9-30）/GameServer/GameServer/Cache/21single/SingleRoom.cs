using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol.Dto.Single;
using Protocol.Constant;
using AhpilyServer;
namespace GameServer.Cache._21single
{
    public class SingleRoom
    {
        /// <summary>
        /// 房间唯一标识码
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// 在房间内用户id的列表 和 连接对象的 映射关系
        /// </summary>
        public Dictionary<int, ClientPeer> UIdClientDict { get; private set; }

        /// <summary>
        /// 已经准备的玩家id列表
        /// </summary>
        public List<int> ReadyUIdList { get; private set; }

        /// <summary>
        /// 存储所有玩家
        /// </summary>
        public SinglePlayerDto player { get; set; }

        /// <summary>
        /// 牌库
        /// </summary>
        public SingleLibraryModel libraryModel { get; set; }

        /// <summary>
        /// 倍数
        /// </summary>
        public int Multiple { get; set; }

        public int Wager { get; set; }

        /// <summary>
        /// 庄家的牌
        /// </summary>
        public List<CardDto> DealerCardList { get; set; }

        /// <summary>
        /// 构造方法 做初始化
        /// </summary>
        /// <param name="id"></param>
        public SingleRoom(int id, int uid)
        {
            this.Id = id;
            this.player = new SinglePlayerDto(uid);
            this.libraryModel = new SingleLibraryModel();
            this.DealerCardList = new List<CardDto>();
            this.Multiple = 1;
            this.Wager = 10;
        }

        public void Init(int uid)
        {
            this.player = new SinglePlayerDto(uid);
        }

        /// <summary>
        /// 发牌 (初始化角色手牌)
        /// </summary>
        public void InitPlayerCards()
        {
            DealerCardList = new List<CardDto>();
            //庄家和闲家各发两张牌
            //闲家
            for (int i = 0; i < 2; i++)
            {
                CardDto card = libraryModel.Deal();
                player.Add(card);
            }
            //庄家
            for (int i = 0; i < 2; i++)
            {
                CardDto card = libraryModel.Deal();
                DealerCardList.Add(card);
            }

        }

        public void TestInitPlayerCards()
        {
            DealerCardList = new List<CardDto>();
            //庄家和闲家各发两张牌
            //闲家
            for (int i = 0; i < 2; i++)
            {
                string cardName = CardColor.GetString(1) + CardWeight21.GetString(1);
                CardDto card = new CardDto(cardName,1,1);
                player.Add(card);
                
            }
            //庄家
            for (int i = 0; i < 2; i++)
            {
                CardDto card = libraryModel.Deal();
                DealerCardList.Add(card);
            }
        }

        public CardDto GetOnePlayerCard()
        {
            CardDto card = libraryModel.Deal();
            player.Add(card);
            return card;
        }
        public CardDto GetOneDealerCard()
        {
            CardDto card = libraryModel.Deal();
            DealerCardList.Add(card);
            return card;
        }
        /// <summary>
        /// 分牌操作
        /// </summary>
        public void SpliteCard()
        {
            List<CardDto> tempList = new List<CardDto>();
            tempList.Add(player.CardList[1]);
            player.Remove(1);

            CardDto card = libraryModel.Deal();
            player.Add(card);


            card = libraryModel.Deal();
            tempList.Add(card);

            player.SpliteCardListQueue.Enqueue(tempList);
            player.SplitNum++;

        }

        public void NextHandCard()
        {
            player.CardList.Clear();
            player.CardList = player.SpliteCardListQueue.Dequeue();
        }
        /// <summary>
        /// 获取玩家的现有手牌
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<CardDto> GetUserCards(int userId)
        {
            return player.CardList;
            throw new Exception("不存在这个玩家！");
        }

        public bool isPlayer21()
        {
            return CardWeight21.GetWeight(player.CardList) == 21;
        }

        public int GetPlayerWeight()
        {
            return CardWeight21.GetWeight(player.CardList);
        }
        public int GetDealerWeight()
        {
            return CardWeight21.GetWeight(DealerCardList);
        }
        public int whichWin()
        {
            int dw = GetDealerWeight();
            int pw = GetPlayerWeight();
            if (dw > 21)//庄家爆牌
                return 3;
            else if (dw > pw)//庄家胜利
                return 4;
            else if (dw < pw)//闲家胜利
                return 5;
            else//平局
                return 6;

        }

        public void addWeightAndMulti(int multi)
        {
            player.multiList.Add(multi);
            player.weightList.Add(GetPlayerWeight());
        }

        public double computeWinOrLoss()
        {
            int dw = GetDealerWeight();
            int pw = 0;
            int pm = 0;
            for (int i=0;i<player.weightList.Count;i++)
            {
                pw = player.weightList[i];
                pm = player.multiList[i];
                if (dw > 21 || dw < pw)//庄家爆牌 闲家胜利
                    player.winOrLose.Add(pm);
                else if (dw > pw)//庄家胜利
                    player.winOrLose.Add(-1 * pm);
                else//平局
                    player.winOrLose.Add(0);
            }

            return player.winOrLose.Sum();

        }

        public void splitClear()
        {
            player.SplitNum = 1;
            player.SpliteCardListQueue.Clear();
            player.SpliteCardListList.Clear();
            player.weightList.Clear();
            player.multiList.Clear();
            player.winOrLose.Clear();
        }

        /// <summary>
        /// 保存一下卡牌数据
        /// </summary>
        ///1 21dian 2爆牌 3不要 
        /// <param name="state"></param>
        /// <param name="isDouble"></param>
        public void SaveList(int state, bool isDouble)
        {
            PlayerCardDto playerCardDto = new PlayerCardDto();
            playerCardDto.CardList.AddRange(player.CardList);
            playerCardDto.CardState = state;
            playerCardDto.Weight = CardWeight21.GetWeight(player.CardList);
            playerCardDto.isDouble = isDouble;
            player.SpliteCardListList.Add(playerCardDto);

        }

        public bool isAllBoom()
        {
            foreach(PlayerCardDto p in player.SpliteCardListList)
            {
                if (p.CardState != 2)
                    return false;
            }
            return true;
        }

        public string getDealerCardType()
        {
            string dealerCardType = "";
            int w = DealerCardList[1].Weight;
            if (w >= 10)
            {
                dealerCardType += "10";
            }
            else
            {
                dealerCardType += w.ToString();
            }
            return dealerCardType;



        }

        public bool isContainA(List<CardDto> cardList)
        {
            foreach(CardDto c in cardList)
            {
                if (c.Weight == 1)
                    return true;
            }
            return false;
        }
        public string getPlayerCardType()
        {
            string playerCardType = "";
            if(player.CardList.Count==2)
            {

            }
            if (player.CardList.Count == 2&&player.CardList[0].Weight == player.CardList[1].Weight)//对子
            {
                if (player.CardList[0].Weight == 1)
                {
                    playerCardType += "AA";

                }
                else if (player.CardList[0].Weight >= 10)
                {
                    playerCardType += "TT";
                }
                else
                {
                    playerCardType += player.CardList[0].Weight.ToString();
                    playerCardType += player.CardList[0].Weight.ToString();
                }

            }
            else if (isContainA(player.CardList))
            {
                int otherWeight = 0;
                foreach(CardDto c in player.CardList)
                {
                    otherWeight += c.Weight;
                }
                otherWeight -= 1;
                if(otherWeight<=9)
                {
                    playerCardType += "A";
                    playerCardType += otherWeight.ToString();
                }
                else
                {
                    playerCardType += "H";
                    playerCardType += CardWeight21.GetWeight(player.CardList).ToString();
                }
                
                
            }
            else
            {
                playerCardType += "H";
                playerCardType += CardWeight21.GetWeight(player.CardList).ToString();

            }
            return playerCardType;
        }







    }
}
