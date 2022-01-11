using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card20012 : Card
    {
        public override void Init()
        {
            this[CardField.Vitality] = 1;

            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this,region,territory);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this).SetTargetCard(this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (triggerInfo) =>
             {
                 await GameSystem.SelectSystem.SelectUnite(this, AgainstInfo.cardSet[Orientation.My][GameRegion.Battle][CardRank.Copper].CardList, 1);
                 await GameSystem.PointSystem.Cure(new TriggerInfoModel(this).SetTargetCard(AgainstInfo.SelectUnits));
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}