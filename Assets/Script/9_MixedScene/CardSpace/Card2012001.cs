using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:���ؼ�
    /// </summary>
    public class Card2012001 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();

            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this, this));
               })
               .AbilityAppend();

            //�滻ԭ����ֵЧ������ӱ�����˳�ж�
            AbalityRegister(TriggerTime.When, TriggerType.Set)
               .AbilityAdd(async (triggerInfo) =>
               {
                   var targetShowPoint = triggerInfo.targetCard.ShowPoint;
                   await Command.CardCommand.Set(triggerInfo);
                   //���ԭ�������������õ���
                   if (targetShowPoint < triggerInfo.point)
                   {
                       await GameSystem.PointSystem.Increase(triggerInfo);
                   }
                   if (targetShowPoint > triggerInfo.point)
                   {
                       await GameSystem.PointSystem.Decrease(triggerInfo);
                   }
               })
               .AbilityReplace();
            //�滻ԭ������Ч���������˳�ж�
            AbalityRegister(TriggerTime.When, TriggerType.Gain)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await Command.CardCommand.Gain(triggerInfo);
                   if (triggerInfo.point>0)//�����������˳״̬
                   {
                       await GameSystem.PointSystem.Increase(triggerInfo);
                   }
               })
               .AbilityReplace();

            //�滻ԭ������Ч���������˳�ж�
            AbalityRegister(TriggerTime.When, TriggerType.Hurt)
                .AbilityAdd(async (triggerInfo) =>
                {
                    if (this[cardState: CardState.Congealbounds])
                    {
                        await GameSystem.StateSystem.ClearState(new TriggerInfoModel(triggerInfo.triggerCard, this));
                    }
                    else
                    {
                        bool isBreak = triggerInfo.point > this[CardField.Shield];
                        await Command.CardCommand.Hurt(triggerInfo);
                        if (!this[CardState.Docile])//�����������˳״̬
                        {
                            await GameSystem.StateSystem.ClearState(new TriggerInfoModel(this, this).SetTargetState(CardState.Furor));
                            await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Docile));
                        }
                    }
                })
                .AbilityReplace();

            AbalityRegister(TriggerTime.When, TriggerType.StateAdd)
                .AbilityAdd(async (triggerInfo) =>
                {
                    if (triggerInfo.targetState == CardState.Furor)
                    {
                        UnityEngine.Debug.Log("����");
                        if (!this[CardState.Docile])//�����������˳״̬
                        {
                            await GameSystem.StateSystem.ClearState(new TriggerInfoModel(this, this).SetTargetState(CardState.Furor));
                            await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Docile));
                        }
                        if (!this[CardState.Furor])//�����������˳״̬
                        {
                            await GameSystem.StateSystem.ClearState(new TriggerInfoModel(this, this).SetTargetState(CardState.Docile));
                            await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Furor));
                        }
                        //����Ч��
                    }
                    if (triggerInfo.targetState == CardState.Docile)
                    {
                        //��˳Ч��
                        UnityEngine.Debug.Log("��˳");

                    }
                }, Condition.Default)
                .AbilityAppend();
        }
    }
}