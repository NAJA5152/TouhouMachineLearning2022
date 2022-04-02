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
    /// <summary>
    /// 卡牌名称:三月精*月
    /// </summary>
    public class Card2002002 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new TriggerInfoModel(this, this));
               })
               .AbilityAppend();
            //部署效果
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (triggerInfo) =>
             {
                 await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, this).SetTargetField(CardField.Inspire, 2));
                 await GameSystem.TransferSystem.SummonCard(
                     new TriggerInfoModel(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].CardList
                     .Where(card => card.CardID == 2002001 || card.CardID == 2002003)
                     .ToList()
                     ));
             }, Condition.Default)
             .AbilityAppend();
            //被召唤时效果
            AbalityRegister(TriggerTime.When, TriggerType.Summon)
                .AbilityAdd(async (triggerInfo) =>
                {
                    await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, this).SetTargetField(CardField.Inspire, 2));
                }, Condition.Default)
                .AbilityAppend();
        }
    }
}