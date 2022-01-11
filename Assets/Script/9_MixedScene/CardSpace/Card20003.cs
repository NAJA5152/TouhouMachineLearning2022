using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card20003 : Card
    {
        public override void Init()
        {
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
                  AgainstInfo.SelectUnits = AgainstInfo.cardSet[GameRegion.Battle][CardRank.Copper, CardRank.Silver][CardFeature.Largest].CardList;
                  await GameSystem.PointSystem.Destory(new TriggerInfoModel(this).SetTargetCard(AgainstInfo.SelectUnits));               
              }, Condition.Default)
              .AbilityAppend();
        }
    }
}