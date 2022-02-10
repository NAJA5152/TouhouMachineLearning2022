using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:���׵���а��
    /// </summary>
    public class Card2011004 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this, this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
              .AbilityAdd(async (triggerInfo) =>
              {
                  List<Card> cardList = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][CardTag.Tool].CardList;
                  await GameSystem.SelectSystem.SelectBoardCard(this, cardList);
                  if (GameSystem.InfoSystem.SelectBoardCardRanks.Count > 0)
                  {
                      await GameSystem.TransSystem.PlayCard(new TriggerInfoModel(this, cardList[GameSystem.InfoSystem.SelectBoardCardRanks[0]]));
                  }
              }, Condition.Default)
              .AbilityAppend();
        }
    }
}