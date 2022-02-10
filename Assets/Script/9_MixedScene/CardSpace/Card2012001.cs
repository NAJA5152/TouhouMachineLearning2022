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

            AbalityRegister(TriggerTime.When, TriggerType.StateAdd)
              .AbilityAdd(async (triggerInfo) => { 
                  if (triggerInfo.targetState== CardState.Furor)
                  {
                      //����Ч��
                  }
                  if (triggerInfo.targetState == CardState.Docile)
                  {
                      //��˳Ч��
                  }
              },Condition.Default)
              .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Gain)
               .AbilityAdd(async (triggerInfo) => { await Command.CardCommand.Gain(triggerInfo); })
               .AbilityReplace();
                //�滻
                AbalityRegister(TriggerTime.When, TriggerType.Hurt)
                .AbilityAdd(async (triggerInfo) =>
                {
                    if (this[cardState: CardState.Congealbounds])
                    {
                        await GameSystem.StateSystem.ClearState(new TriggerInfoModel(triggerInfo.triggerCard, this));
                    }
                    else
                    {
                        await Command.CardCommand.Hurt(triggerInfo);
                        if (!this[CardState.Docile])//�����������˳״̬
                        {
                            await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Docile));
                        }
                    }
                })
                .AbilityReplace();
        }
    }
}