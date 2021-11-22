using Sirenix.OdinInspector;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    public class SingleRowInfo : MonoBehaviour
    {
        public Color color;
        public Card TempCard;
        public Orientation orientation;
        public GameRegion region;
        public bool CanBeSelected;
        [ShowInInspector]
        //计算在全局卡组中对应的顺序
        public int rank
        {
            get
            {
                int regionId = (int)region;
                if (region > GameRegion.Battle)
                {
                    regionId -= 2;
                }
                //根据玩家扮演角色（1或者2）分配上方区域和下方区域
                return regionId + (AgainstInfo.isPlayer1 ^ (orientation == Orientation.Down) ? 9 : 0);
            }
        }

        private void Awake() => AgainstInfo.cardSet.singleRowInfos.Add(this);
        public int Location => this.JudgeRank(AgainstInfo.FocusPoint);
        public int RowRank => CardSet.globalCardList.IndexOf(ThisRowCards);
        public Material CardMaterial => transform.GetComponent<Renderer>().material;
        public List<Card> ThisRowCards
        {
            get => AgainstInfo.cardSet[rank];
            set => AgainstInfo.cardSet[rank] = value;
        }
        public void SetRegionSelectable(bool CanBeSelected)
        {
            this.CanBeSelected = CanBeSelected;
            CardMaterial.SetColor("_GlossColor", CanBeSelected ? color : Color.black);
        }
    }
}