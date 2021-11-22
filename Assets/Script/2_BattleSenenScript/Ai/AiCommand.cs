using System;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.Command
{
    public class AiCommand
    {
        static Random rand = new Random("gezi".GetHashCode());
        public static void Init() => rand = new Random("gezi".GetHashCode());
        public static int GetRandom(int Min, int Max) => rand.Next(Min, Max);
        public static void RoundStartExchange(bool isControlPlayer)
        {
            if (isControlPlayer)//超时自动操纵玩家时
            {
                if (AgainstInfo.isPlayer1)
                {
                    AgainstInfo.isPlayer1RoundStartExchangeOver = true;
                }
                else
                {
                    AgainstInfo.isPlayer2RoundStartExchangeOver = true;
                }
                AgainstInfo.IsSelectCardOver = true;
            }
            else//操纵对面Ai
            {
                if (AgainstInfo.isPVE)
                {
                    //UnityEngine.Debug.Log("ai选择了交换卡牌"+ AgainstInfo.isPlayer1);
                    if (AgainstInfo.isPlayer1)
                    {
                        AgainstInfo.isPlayer2RoundStartExchangeOver = true;
                    }
                    else
                    {
                        AgainstInfo.isPlayer1RoundStartExchangeOver = true;
                    }
                }
            }
        }
        public static async Task TempPlayOperation()
        {
            if ((Info.AgainstInfo.isDownPass && Info.AgainstInfo.TotalDownPoint < Info.AgainstInfo.TotalUpPoint) ||
                Info.AgainstInfo.cardSet[Orientation.My][GameRegion.Hand].CardList.Count == 0)
            {
                //设置pass标记位
                AgainstInfo.isPlayerPass = true;
            }
            else
            {

                Card targetCard = Info.AgainstInfo.cardSet[Orientation.My][GameRegion.Hand].CardList[0];
                Info.AgainstInfo.playerPlayCard = targetCard;
            }
        }
    }
}