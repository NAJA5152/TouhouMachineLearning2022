using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��������
    /// ��������:�ҷ��غϽ������Ƴ����൥λ����ֵ�������������������
    /// </summary>
    public class Card2102005 : Card
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
                  int energyPoint = TwoSideCard.Sum(card => card[CardField.Energy]);
                  await GameSystem.FieldSystem.ChangeField(new TriggerInfoModel(this, this).SetTargetField(CardField.Energy, energyPoint));
                  await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, TwoSideCard).SetTargetField(CardField.Energy, 0));
                  await GameSystem.PointSystem.Gain(new TriggerInfoModel(this,this).SetPoint(energyPoint));
              }, Condition.Default, Condition.OnMyTurn)
              .AbilityAppend();
        }
    }
}