using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card10001 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async triggerInfo =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this,region,territory);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this).SetTargetCard(this));
               }, Condition.Default)
                .AbilityAdd(async triggerInfo =>
                {
                    await GameSystem.SelectSystem.SelectLocation(this, region, territory);
                    await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this).SetTargetCard(this));
                }, Condition.Default)
               .AbilityAppend();
        }
    }
}