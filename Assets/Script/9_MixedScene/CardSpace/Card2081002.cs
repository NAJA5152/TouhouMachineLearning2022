using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:�ɵ���Թ��
    /// ��������:�ҷ��غϽ���ʱ:������λ�����ƣ����ƶ����Է����� ���ҷ�passʱ �ٻ������� �����ٻ�:�����ҷ����������Ҳ�Ŀ���
    /// </summary>
    public class Card2081002 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new TriggerInfoModel(this,this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.TransferSystem.MoveToOpHand(this);
               },Condition.OnHand)
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.TransferSystem.DisCard(new TriggerInfoModel(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Hand].CardList.LastOrDefault()));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}