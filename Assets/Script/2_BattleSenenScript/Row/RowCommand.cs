using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    public static class RowCommand
    {
        ///////////////////////////////////////////////////////////////////////////////////原本的INFO/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static List<Card> GetCardList(Card targetCard) => CardSet.globalCardList.First(list => list.Contains(targetCard));
        public static Location GetLocation(Card TargetCard)
        {
            int RankX = -1;
            int RankY = -1;
            for (int i = 0; i < CardSet.globalCardList.Count; i++)
            {
                if (CardSet.globalCardList[i].Contains(TargetCard))
                {
                    RankX = i;
                    RankY = CardSet.globalCardList[i].IndexOf(TargetCard);
                }
            }
            return new Location(RankX, RankY);
        }
        public static Card GetCard(int x, int y) => x == -1 ? null : CardSet.globalCardList[x][y];
        public static Card GetCard(Location Locat) => Locat.X == -1 ? null : CardSet.globalCardList[Locat.X][Locat.Y];
        public static SingleRowInfo GetSingleRowInfoById(int Id) => AgainstInfo.cardSet.singleRowInfos.First(infos => infos.ThisRowCards == CardSet.globalCardList[Id]);
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static async Task CreatTempCard(SingleRowInfo SingleInfo)
        {
            Card modelCard = AgainstInfo.cardSet[Orientation.My][GameRegion.Uesd].CardList[0];
            SingleInfo.TempCard = CardCommand.CreateCard(modelCard.cardID);
            SingleInfo.TempCard.isGray = true;
            SingleInfo.TempCard.SetCardSeeAble(true);
            SingleInfo.ThisRowCards.Insert(SingleInfo.Location, SingleInfo.TempCard);
            SingleInfo.TempCard.Init();
        }
        public static void DestoryTempCard(SingleRowInfo SingleInfo)
        {
            SingleInfo.ThisRowCards.Remove(SingleInfo.TempCard);
            GameObject.Destroy(SingleInfo.TempCard.gameObject);
            SingleInfo.TempCard = null;
        }
        public static void ChangeTempCard(SingleRowInfo SingleInfo)
        {
            SingleInfo.ThisRowCards.Remove(SingleInfo.TempCard);
            SingleInfo.ThisRowCards.Insert(SingleInfo.Location, SingleInfo.TempCard);
        }
        public static void RefreshHandCard(List<Card> cardList)
        {
            cardList.ForEach(card => card.isPrepareToPlay = (AgainstInfo.playerFocusCard != null && card == AgainstInfo.playerFocusCard && card.isFree));
        }
        public static void SetPlayCardMoveFree(bool isFree)
        {
            AgainstInfo.cardSet[Orientation.Down][GameRegion.Leader, GameRegion.Hand].CardList.ForEach(card => card.isFree = isFree);
        }
        public static void SetRegionSelectable(GameRegion region, Territory territory = Territory.All)
        {
            if (region == GameRegion.None)
            {
                AgainstInfo.cardSet[GameRegion.Battle].singleRowInfos.ForEach(row =>
                {
                    row.CanBeSelected = false;
                    row.CardMaterial.SetColor("_GlossColor", Color.black);
                });
            }
            else
            {
                AgainstInfo.cardSet[region][(Orientation)territory].singleRowInfos.ForEach(row =>
                {
                    row.CanBeSelected = true;
                    row.CardMaterial.SetColor("_GlossColor", row.color);
                });
            }
        }
    }
}


