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
    public class Card20006 : Card
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
                   await GameSystem.TransSystem.DeployCard(new TriggerInfo(this).SetTargetCard(this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (triggerInfo) =>
             {
                 AgainstInfo.selectUnits = AgainstInfo.cardSet[Orientation.My][GameRegion.Deck].CardList.Where(card => card.cardID == 20007 || card.cardID == 20008).ToList();
                 await GameSystem.TransSystem.SummonCard(new TriggerInfo(this).SetTargetCard(AgainstInfo.selectUnits));
             }, Condition.Default)
             .AbilityAppend();

            cardAbility[TriggerTime.When][TriggerType.FieldChange] = new List<Func<TriggerInfo, Task>>()
            {
                async (triggerInfo) =>
                {
                    EffectCommand.Bullet_Gain(triggerInfo);
                    await AudioCommand.PlayAsync(GameAudioType.Biu);
                    this[CardField.Vitality]=triggerInfo.point;
                }
            };
        }
    }
}