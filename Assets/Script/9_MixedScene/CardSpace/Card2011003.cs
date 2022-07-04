using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��յ����
    /// ��������:�غϿ�ʼ���������е�λ�׼�ѭ��һ��
    /// </summary>
    public class Card2011003 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.VariationSystem.SetVariation(VariationType.Reverse);
                   await GameSystem.TransferSystem.MoveToGrave(this);
               })
               .AbilityAppend();
        }
    }
}