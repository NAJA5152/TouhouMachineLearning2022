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
        //只有一个卡牌位
        bool IsSingle => region == GameRegion.Grave || region == GameRegion.Deck || region == GameRegion.Used;
        [ShowInInspector]
        //计算在全局卡组中对应的顺序
        //根据玩家扮演角色（1或者2）分配上方区域和下方区域
        public int RowRank => (int)region + (AgainstInfo.IsPlayer1 ^ (orientation == Orientation.Down) ? 9 : 0);
        private void Awake() => AgainstInfo.cardSet.RowManagers.Add(this);
        public int Location => JudgeRank(this, AgainstInfo.FocusPoint);
        public Material CardMaterial => transform.GetComponent<Renderer>().material;
        //当前行管理其所管理的的卡牌列表
        public List<Card> CardList => AgainstInfo.cardSet[RowRank];

        void Update()
        {
            TempCardControl();
            SetCardsPosition(CardList.Where(card=>card!=null).ToList());
            if (IsMyHandRegion)
            {
                CardList.ForEach(card => card.isPrepareToPlay = (AgainstInfo.playerFocusCard != null && card == AgainstInfo.playerFocusCard && card.IsFree));
            }
            GetComponent<Renderer>().material.SetFloat("_Strength", Mathf.PingPong(Time.time * 10, 10) + 10);
        }
        public void TempCardControl()
        {
            if (AgainstInfo.IsMyTurn)
            {
                //创建临时卡牌
                if (TempCard == null && CanBeSelected && AgainstInfo.PlayerFocusRegion == this && TempCard==null)
                {
                    Card modelCard = AgainstInfo.cardSet[Orientation.My][GameRegion.Used].CardList.LastOrDefault();
                    TempCard = Command.CardCommand.GenerateCard(modelCard.CardID);
                    TempCard.IsGray = true;
                    TempCard.SetCardSeeAble(true);
                    CardList.Insert(Location, TempCard);
                    //TempCard.Init();
                }
                //改变临时卡牌的位置
                if (TempCard != null && Location != CardList.IndexOf(TempCard))
                {
                    CardList.Remove(TempCard);
                    CardList.Insert(Location, TempCard);
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
                    ThisCardList[i].SetMoveTarget(transform.position + Vector3.left * (Actual_Interval * i - Actual_Bias) + Actual_Offset_Up + Actual_Offset_Forward + MoveStepOver_Offset, transform.eulerAngles);
                }
                else
                {
                    ThisCardList[i].SetMoveTarget(AgainstInfo.dragToPoint, Vector3.zero);
                }
                ThisCardList[i].RefreshState();
            }
        }
        public static int JudgeRank(SingleRowManager singleRowInfo, Vector3 point)
        {
            int Rank = 0;
            float posx = -(point.x - singleRowInfo.transform.position.x);
            int UniteNum = singleRowInfo.CardList.Where(card => !card.IsGray).Count();
            for (int i = 0; i < UniteNum; i++)
            {
                if (posx > i * 1.6 - (UniteNum - 1) * 0.8)
                {
                    Rank = i + 1;
                }
            }
            return Rank;
        }
    }
}