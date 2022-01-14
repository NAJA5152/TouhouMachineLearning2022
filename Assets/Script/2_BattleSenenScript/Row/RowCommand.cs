using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    public static class RowCommand
    {
        ///////////////////////////////////////////////////////////////////////////////////原本的INFO/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Location GetLocation(Card TargetCard)
        {
            int RankX = -1;
            int RankY = -1;
            for (int i = 0; i < CardSet.GlobalCardList.Count; i++)
            {
                if (CardSet.GlobalCardList[i].Contains(TargetCard))
                {
                    RankX = i;
                    RankY = CardSet.GlobalCardList[i].IndexOf(TargetCard);
                }
            }
            return new Location(RankX, RankY);
        }
        public static Card GetCard(int x, int y) => x == -1 ? null : CardSet.GlobalCardList[x][y];
        public static Card GetCard(Location location) => location.X == -1 ? null : CardSet.GlobalCardList[location.X][location.Y];
        public static List<Card> GetCardList(Card targetCard) => CardSet.GlobalCardList.First(list => list.Contains(targetCard));

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetPlayCardMoveFree(bool isFree) => AgainstInfo.cardSet[Orientation.Down][GameRegion.Leader, GameRegion.Hand].CardList.ForEach(card => card.IsFree = isFree);
        public static void SetRegionSelectable(GameRegion region, Territory territory = Territory.All)
        {
            if (region == GameRegion.None)
            {
                AgainstInfo.cardSet[GameRegion.Battle].SingleRowInfos.ForEach(row =>
                {
                    row.CanBeSelected = false;
                    row.CardMaterial.SetColor("_GlossColor", Color.black);
                });
            }
            else
            {
                AgainstInfo.cardSet[region][(Orientation)territory].SingleRowInfos.ForEach(row =>
                {
                    row.CanBeSelected = true;
                    row.CardMaterial.SetColor("_GlossColor", row.color);
                });
            }
        }
    }
}