using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:������
    /// ��������:���꣺�ڵз�ͬ�����Ҳ�����һ��ͷ��ÿ�غϿ�ʼ�����ƶ�������һ�����Ҳಢ����൥λ���1���˺� ��˳�����ҷ�ͬ�����Ҳ�����һ��ͷ��ÿ�غϿ�ʼ����൥����һ������
    /// </summary>
    public class Card2012002 : Card
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
            AbalityRegister(TriggerTime.When, TriggerType.Increase)
               .AbilityAdd(async (triggerInfo) =>
               {
                   if (!this[CardState.Furor])//��������ڿ���״̬
                    {
                       await GameSystem.StateSystem.ClearState(new TriggerInfoModel(this, this).SetTargetState(CardState.Docile));
                       await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Furor));

                       await GameSystem.TransferSystem.GenerateCard(new TriggerInfoModel(this, targetCard: null).SetTargetCardId(2013006).SetLocation( CurrentOrientation, CurrentRegion,-1));
                   }
               }, Condition.Default)
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Decrease)
               .AbilityAdd(async (triggerInfo) =>
               {
                   if (!this[CardState.Docile])//�����������˳״̬
                   {
                       await GameSystem.StateSystem.ClearState(new TriggerInfoModel(this, this).SetTargetState(CardState.Furor));
                       await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Docile));

                       await GameSystem.TransferSystem.GenerateCard(new TriggerInfoModel(this, targetCard: null).SetTargetCardId(2013007).SetLocation(CurrentOrientation, CurrentRegion, -1));

                   }
               }, Condition.Default)
                .AbilityAppend();
        }
    }
}