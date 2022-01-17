using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card1000001 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async triggerInfo =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this).SetTargetCard(this));
               }, Condition.Default)
                .AbilityAdd(async triggerInfo =>
                {
                    await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                    await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this).SetTargetCard(this));
                }, Condition.Default)
               .AbilityAppend();
        }
    }
}