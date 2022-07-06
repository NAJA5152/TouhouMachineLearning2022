using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:����������
    /// ��������:�����Ƴ���൥λ���������ѡ���ҷ�����һ���ǽ�λ�������������ֵ�Ļ���
    /// </summary>
    public class Card2103003 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new Event(this,this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
              .AbilityAdd(async (e) =>
              {
                  int num = LeftCard[CardField.Energy];
                  await GameSystem.FieldSystem.SetField(new Event(this, LeftCard).SetTargetField(CardField.Energy, 0));

                  await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Silver,CardRank.Copper].CardList, 1);
                  await GameSystem.FieldSystem.ChangeField(new Event(this, GameSystem.InfoSystem.SelectUnits).SetTargetField(CardField.Shield, num));
              }, Condition.Default)
              .AbilityAppend();
        }
    }
}