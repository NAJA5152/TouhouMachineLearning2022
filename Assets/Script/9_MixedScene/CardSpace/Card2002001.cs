using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:���¾�*��
    /// ��������:����:�����������Ϊ2, ���ٻ����¾�*�£����¾�*�� ���ٻ�ʱ:�����������Ϊ2
    /// </summary>
    public class Card2002001 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new TriggerInfoModel(this, this));
               })
               .AbilityAppend();
            //����Ч��
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (triggerInfo) =>
             {
                 await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, this).SetTargetField(CardField.Inspire, 2));
                 await GameSystem.TransferSystem.SummonCard(
                     new TriggerInfoModel(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].CardList
                     .Where(card => card.CardID == 2002002 || card.CardID == 2002003)
                     .ToList())
                     );
             }, Condition.Default)
             .AbilityAppend();
            //���ٻ�ʱЧ��
            AbalityRegister(TriggerTime.When, TriggerType.Summon)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, this).SetTargetField(CardField.Inspire, 2));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}