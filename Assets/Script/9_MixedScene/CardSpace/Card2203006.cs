using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:�ڽ̴���
    /// </summary>
    public class Card2203006 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new TriggerInfoModel(this, this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle][CardRank.NoGold].CardList, 1);
                   if (GameSystem.InfoSystem.SelectUnit != null)
                   {
                       await GameSystem.TransferSystem
                       .MoveCard(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnit)
                            .SetLocation(GameSystem.InfoSystem.SelectUnit.CurrentOrientation, CurrentRegion, -1));
                   }
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}