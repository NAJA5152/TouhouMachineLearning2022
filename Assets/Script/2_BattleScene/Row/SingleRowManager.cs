using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
namespace TouhouMachineLearningSummary.Manager
{
    //负责管理每个区域的卡牌位置，区域显示状态等
    public class SingleRowManager : MonoBehaviour
    {
        public Color color;
        public Card TempCard;
        public Orientation orientation;
        public GameRegion region;
        public bool CanBeSelected;
        public float Range;
        public bool IsMyHandRegion;
        //当前行管理其所管理的的卡牌列表
        public List<Card> CardList => AgainstInfo.cardSet[RowRank];
        public Material CardMaterial => transform.GetComponent<Renderer>().material;
        //只有一个卡牌位
        bool IsSingle => region == GameRegion.Grave || region == GameRegion.Deck || region == GameRegion.Used;
        [ShowInInspector]
        //计算在全局卡组中对应的顺序
        //根据玩家扮演角色（1或者2）分配上方区域和下方区域
        public int RowRank => (int)region + (AgainstInfo.IsPlayer1 ^ (orientation == Orientation.Down) ? 9 : 0);
        //判断玩家的焦点区域位置次序
        public int Rank
        {
            get
            {
                int Rank = 0;
                float posx = -(AgainstInfo.FocusPoint.x - this.transform.position.x);
                int UnitsNum = this.CardList.Where(card => !card.IsGray).Count();
                for (int i = 0; i < UnitsNum; i++)
                {
                    if (posx > i * 1.6 - (UnitsNum - 1) * 0.8)
                    {
                        Rank = i + 1;
                    }
                }
                return Rank;
            }
        }
        private void Awake() => AgainstInfo.cardSet.RowManagers.Add(this);
        void Update()
        {
            TempCardControl();
            SetCardsPosition(CardList.Where(card => card != null).ToList());
            if (IsMyHandRegion)
            {
                CardList.ForEach(card => card.isPrepareToPlay = (AgainstInfo.playerFocusCard != null && card == AgainstInfo.playerFocusCard && card.IsFree));
            }
            GetComponent<Renderer>().material.SetFloat("_Strength", Mathf.PingPong(Time.time * 10, 10) + 10);

            void TempCardControl()
            {
                if (AgainstInfo.IsMyTurn)
                {
                    //创建临时卡牌
                    if (TempCard == null && CanBeSelected && AgainstInfo.PlayerFocusRegion == this && TempCard == null)
                    {
                        Card modelCard = AgainstInfo.cardSet[Orientation.My][GameRegion.Used].CardList.LastOrDefault();
                        TempCard = Command.CardCommand.GenerateCard(modelCard.CardID);
                        TempCard.IsGray = true;
                        TempCard.IsCanSee = true;
                        CardList.Insert(Rank, TempCard);
                        //TempCard.Init();
                    }
                    //改变临时卡牌的位置
                    if (TempCard != null && Rank != CardList.IndexOf(TempCard))
                    {
                        CardList.Remove(TempCard);
                        CardList.Insert(Rank, TempCard);
                    }
                    //销毁临时卡牌
                    if (TempCard != null && !(CanBeSelected && AgainstInfo.PlayerFocusRegion == this))
                    {
                        CardList.Remove(TempCard);
                        Destroy(TempCard.gameObject);
                        TempCard = null;
                    }
                }
            }
            void SetCardsPosition(List<Card> ThisCardList)
            {
                int Num = ThisCardList.Count;
                for (int i = 0; i < ThisCardList.Count; i++)
                {

                    float Actual_Interval = Mathf.Min(Range / Num, 1.6f);
                    float Actual_Bias = IsSingle ? 0 : (Mathf.Min(ThisCardList.Count, 6) - 1) * 0.8f;
                    Vector3 Actual_Offset_Up = transform.up * (0.2f + i * 0.01f) * (ThisCardList[i].isPrepareToPlay ? 1.1f : 1);
                    Vector3 MoveStepOver_Offset = ThisCardList[i].isMoveStepOver ? Vector3.zero : Vector3.up;
                    Vector3 Actual_Offset_Forward = ThisCardList[i].isPrepareToPlay ? -transform.forward * 0.5f : Vector3.zero;
                    if (ThisCardList[i].IsAutoMove)
                    {
                        ThisCardList[i].SetCardTransform(transform.position + Vector3.left * (Actual_Interval * i - Actual_Bias) + Actual_Offset_Up + Actual_Offset_Forward + MoveStepOver_Offset, transform.eulerAngles);
                    }
                    else
                    {
                        ThisCardList[i].SetCardTransform(AgainstInfo.dragToPoint, Vector3.zero);
                    }
                    ThisCardList[i].RefreshState();
                }
            }

        }
    }
}