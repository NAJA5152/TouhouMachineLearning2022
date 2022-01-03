using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    public class CardSet
    {
        /// <summary>
        /// 卡牌数据集的最源头，可直接修改
        /// </summary>
        [ShowInInspector]
        public static List<List<Card>> globalCardList = new List<List<Card>>();
        public List<SingleRowInfo> singleRowInfos = new List<SingleRowInfo>();
        [ShowInInspector]
        private List<Card> cardList = null;
        public int count => CardList.Count;

        public List<Card> CardList { get => cardList ?? globalCardList.SelectMany(x => x).ToList(); set => cardList = value; }

        ///// <summary>
        ///// 得到触发牌之外的卡牌列表，用于广播触发事件的前后相关事件
        ///// </summary>
        ///// <param name="card"></param>
        ///// <returns></returns>
        //public List<Card> BroastCardList(Card card) => Info.AgainstInfo.cardSet[Orientation.All].CardList.Except(new List<Card> { card }).ToList();

        public CardSet()
        {
            globalCardList.Clear();
            Enumerable.Range(0, 18).ToList().ForEach(x => globalCardList.Add(new List<Card>()));
        }
        public CardSet(List<SingleRowInfo> singleRowInfos, List<Card> cardList = null)
        {
            this.singleRowInfos = singleRowInfos;
            this.CardList = cardList;
        }
        public List<Card> this[int rank]
        {
            get => globalCardList[rank];
            set => globalCardList[rank] = value;
        }
        public CardSet this[params GameRegion[] regions]
        {
            get
            {
                List<SingleRowInfo> targetRows = new List<SingleRowInfo>();
                regions.ToList().ForEach(region =>
                {
                    if (region == GameRegion.Battle)
                    {
                        targetRows.AddRange(singleRowInfos.Where(row =>
                        row.region == GameRegion.Water ||
                        row.region == GameRegion.Fire ||
                        row.region == GameRegion.Wind ||
                        row.region == GameRegion.Soil
                        ));
                    }
                    else
                    {
                        targetRows.AddRange(singleRowInfos.Where(row => row.region == region));
                    }
                });
                List<Card> filterCardList = CardList ?? globalCardList.SelectMany(x => x).ToList();
                filterCardList = filterCardList.Intersect(targetRows.SelectMany(x => x.ThisRowCards)).ToList();
                return new CardSet(targetRows, filterCardList);
            }
        }
        public CardSet this[Orientation orientation]
        {
            get
            {
                List<SingleRowInfo> targetRows = new List<SingleRowInfo>();
                switch (orientation)
                {
                    case Orientation.Up:
                        targetRows = singleRowInfos.Where(row => row.orientation == orientation).ToList(); break;
                    case Orientation.Down:
                        targetRows = singleRowInfos.Where(row => row.orientation == orientation).ToList(); break;
                    case Orientation.My:
                        return this[AgainstInfo.IsMyTurn ? Orientation.Down : Orientation.Up];
                    case Orientation.Op:
                        return this[AgainstInfo.IsMyTurn ? Orientation.Up : Orientation.Down];
                    case Orientation.All:
                        targetRows = singleRowInfos; break;
                }
                //List<Card> filterCardList = cardList ?? globalCardList.SelectMany(x => x).ToList();
                List<Card> filterCardList = CardList;
                filterCardList = filterCardList.Intersect(targetRows.SelectMany(x => x.ThisRowCards)).ToList();
                return new CardSet(targetRows, filterCardList);
            }
        }
        //待补充
        public CardSet this[CardState cardState]
        {
            get
            {
                //CardList = CardList ?? globalCardList.SelectMany(x => x).ToList();
                List<Card> filterCardList = CardList.Where(card =>
                   card.cardStates.ContainsKey(cardState) && card.cardStates[cardState])
                    .ToList();
                return new CardSet(singleRowInfos, filterCardList);
            }
        }
        //待补充
        public CardSet this[CardField cardField]
        {
            get
            {
                //CardList = CardList ?? globalCardList.SelectMany(x => x).ToList();
                List<Card> filterCardList = CardList.Where(card =>
                   card.cardFields.ContainsKey(cardField))
                    .ToList();
                return new CardSet(singleRowInfos, filterCardList);
            }
        }
        /// <summary>
        /// 根据Tag检索卡牌
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public CardSet this[params CardTag[] tags]
        {
            get
            {
                CardList = CardList ?? globalCardList.SelectMany(x => x).ToList();
                List<Card> filterCardList = CardList.Where(card =>
                    tags.Any(tag =>
                        card.cardTag.Contains(tag.ToString().Translation())))
                    .ToList();
                return new CardSet(singleRowInfos, filterCardList);
            }
        }
        //待补充
        public CardSet this[CardFeature cardFeature]
        {
            get
            {
                List<Card> filterCardList = CardList;
                if (filterCardList.Any())
                {
                    switch (cardFeature)
                    {
                        case CardFeature.Largest:
                            int largestPoint = CardList.Max(card => card.showPoint);
                            filterCardList = CardList.Where(card => card.showPoint == largestPoint).ToList();
                            break;
                        case CardFeature.Lowest:
                            int lowestPoint = CardList.Min(card => card.showPoint);
                            filterCardList = CardList.Where(card => card.showPoint == lowestPoint).ToList();
                            break;
                        default:
                            break;
                    }
                }
                return new CardSet(singleRowInfos, filterCardList);
            }
        }
        //按卡牌阶级筛选
        public CardSet this[params CardRank[] ranks]
        {
            get
            {
                List<Card> filterCardList = CardList
                    .Where(card => ranks.Any(rank => card.cardRank == rank))
                    .ToList();
                return new CardSet(singleRowInfos, filterCardList);
            }
        }
        //按卡牌类型筛选
        public CardSet this[CardType type]
        {
            get
            {
                List<Card> filterCardList = CardList
                    .Where(card => card.cardType == type)
                    .ToList();
                return new CardSet(singleRowInfos, filterCardList);
            }
        }
        public void Add(Card card, int rank = -1)
        {
            if (singleRowInfos.Count != 1)
            {
                Debug.LogWarning("选择区域异常，数量为" + singleRowInfos.Count);
            }
            if (rank == -1)
            {
                rank = singleRowInfos[0].ThisRowCards.Count;
            }
            singleRowInfos[0].ThisRowCards.Insert(rank, card);
        }
        public void Remove(Card card)
        {
            if (singleRowInfos.Count != 1)
            {
                Debug.LogWarning("选择区域异常，数量为" + singleRowInfos.Count);
            }
            singleRowInfos[0].ThisRowCards.Remove(card);
        }
        //任意区域排序
        public void Order() => singleRowInfos.ForEach(x => x.ThisRowCards = x.ThisRowCards.OrderByDescending(card => card.cardRank).ThenBy(card => card.basePoint).ThenBy(card => card.cardID).ToList());
    }
}