using UnityEngine;

namespace TouhouMachineLearningSummary.Control
{
    public class SelectCardManager : MonoBehaviour
    {
        public int Rank;
        public GameObject selectRect => transform.GetChild(1).gameObject;

        public void OnMouseClick()
        {
            if (Info.AgainstInfo.cardBoardMode == GameEnum.CardBoardMode.Select || Info.AgainstInfo.cardBoardMode == GameEnum.CardBoardMode.ExchangeCard)
            {
                if (Info.AgainstInfo.SelectBoardCardRanks.Contains(Rank))//如果已选，则移除
                {
                    Info.AgainstInfo.SelectBoardCardRanks.Remove(Rank);
                    selectRect.SetActive(false);
                    Debug.Log("取消选择" + Rank);
                    //如果是小局开局抽卡，则不同步选择卡牌数据消息，只同步换牌数据
                    //若是卡牌效果换牌，则同步换牌数据
                    if (!Info.AgainstInfo.isRoundStartExchange)
                    {
                        Command.NetCommand.AsyncInfo(GameEnum.NetAcyncType.SelectBoardCard);
                    }
                }
                else//否则加入选择列表
                {
                    Info.AgainstInfo.SelectBoardCardRanks.Add(Rank);
                    selectRect.SetActive(true);
                    Debug.Log("选择" + Rank);
                    if (!Info.AgainstInfo.isRoundStartExchange)
                    {
                        Command.NetCommand.AsyncInfo(GameEnum.NetAcyncType.SelectBoardCard);
                    }

                }
            }
        }
    }
}