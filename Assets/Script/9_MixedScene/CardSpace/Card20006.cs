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
                   await GameSystem.SelectSystem.SelectLocation(this, region, territory);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this).SetTargetCard(this));
               })
               .AbilityAppend();
            //部署效果
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (triggerInfo) =>
             {
                 AgainstInfo.SelectUnits =GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].CardList.Where(card => card.CardID == 20007 || card.CardID == 20008).ToList();
                 await GameSystem.TransSystem.SummonCard(new TriggerInfoModel(this).SetTargetCard(AgainstInfo.SelectUnits));
             }, Condition.Default)
             .AbilityAppend();

            cardAbility[TriggerTime.When][TriggerType.FieldChange] = new List<Func<TriggerInfoModel, Task>>()
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