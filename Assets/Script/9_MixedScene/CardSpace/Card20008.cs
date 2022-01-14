using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card20008 : Card
    {
        public override void Init()
        {

            this[CardField.Vitality] = 2;

            //初始化通用卡牌效果
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
                    AgainstInfo.SelectUnits = AgainstInfo.cardSet[Orientation.My][GameRegion.Deck].CardList.Where(card => card.CardID == 20006 || card.CardID == 20007).ToList();
                    await GameSystem.TransSystem.SummonCard(new TriggerInfoModel(this).SetTargetCard(AgainstInfo.SelectUnits));
                }, Condition.Default)
                .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.FieldChange)
                 .AbilityAdd(async (triggerInfo) =>
                 {
                     EffectCommand.Bullet_Gain(triggerInfo);
                     EffectCommand.AudioEffectPlay(1);
                     await Task.Delay(1000);
                     this[CardField.Vitality] = triggerInfo.point;
                 }, Condition.Default)
             .AbilityAppend();
        }

    }
}