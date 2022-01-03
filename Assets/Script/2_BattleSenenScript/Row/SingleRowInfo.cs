using Sirenix.OdinInspector;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    //修改为Manager
    public class SingleRowInfo : MonoBehaviour
    {
        public Color color;
        public Card TempCard;
        public Orientation orientation;
        public GameRegion region;
        public bool CanBeSelected;

        public float Range;
        public bool IsMyHandRegion;
        public bool IsSingle;
        public bool HasTempCard;
        [ShowInInspector]
        //计算在全局卡组中对应的顺序
        //根据玩家扮演角色（1或者2）分配上方区域和下方区域
        public int Rank => (int)region + (AgainstInfo.IsPlayer1 ^ (orientation == Orientation.Down) ? 9 : 0);
        private void Awake() => AgainstInfo.cardSet.singleRowInfos.Add(this);
        public int Location => this.JudgeRank(AgainstInfo.FocusPoint);
        public int RowRank => CardSet.globalCardList.IndexOf(ThisRowCards);
        public Material CardMaterial => transform.GetComponent<Renderer>().material;
        [System.Obsolete("废弃，调整结构")]
        public List<Card> ThisRowCards
        {
            get => AgainstInfo.cardSet[Rank];
            set => AgainstInfo.cardSet[Rank] = value;
        }

        void Update()
        {
            TempCardControl();
            SetCardsPosition(ThisRowCards);
            if (IsMyHandRegion)
            {
                Command.RowCommand.RefreshHandCard(ThisRowCards);
            }
            GetComponent<Renderer>().material.SetFloat("_Strength", Mathf.PingPong(Time.time * 10, 10) + 10);
        }
        public void TempCardControl()
        {
            if (AgainstInfo.IsMyTurn)
            {
                if (TempCard == null && CanBeSelected && AgainstInfo.PlayerFocusRegion == this && !HasTempCard)
                {
                    HasTempCard = true;
                    _ = Command.RowCommand.CreatTempCard(this);
                }
                if (TempCard != null && Location != ThisRowCards.IndexOf(TempCard))
                {
                    Command.RowCommand.ChangeTempCard(this);
                }
                if (TempCard != null && !(CanBeSelected && AgainstInfo.PlayerFocusRegion == this))
                {
                    Command.RowCommand.DestoryTempCard(this);
                    HasTempCard = false;
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
    }
}