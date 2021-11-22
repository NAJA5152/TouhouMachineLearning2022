using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    public static class RowCommand
    {
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
                AgainstInfo.cardSet[GameRegion.Battle].singleRowInfos.ForEach(row => row.SetRegionSelectable(false));
            }
            else
            {
                AgainstInfo.cardSet[region][(Orientation)territory].singleRowInfos.ForEach(row => row.SetRegionSelectable(true));
            }
        }
    }
}


