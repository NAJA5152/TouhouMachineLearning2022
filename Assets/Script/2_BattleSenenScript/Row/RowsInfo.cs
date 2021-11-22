using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.Info
{
    public class RowsInfo : SerializedMonoBehaviour
    {
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
    }
}