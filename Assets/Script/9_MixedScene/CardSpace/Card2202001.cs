using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:���ô���
    /// ��������:ÿ��һ����λ����λ�ƣ��ҷ��漣���ƻ��һ����ֵ
    /// </summary>
    public class Card2202001 : Card
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
            AbalityRegister(TriggerTime.After, TriggerType.Move)
             .AbilityAdd(async (triggerInfo) =>
             {
                 await GameSystem.FieldSystem.ChangeField(new TriggerInfoModel(this, Info.AgainstInfo.cardSet[Orientation.My][CardTag.Miracle].CardList).SetTargetField(CardField.Pary, 1));
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}