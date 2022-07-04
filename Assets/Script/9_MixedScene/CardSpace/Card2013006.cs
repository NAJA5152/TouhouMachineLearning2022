using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��֮ͷ
    /// ��������:ÿ�غϿ�ʼ�����ƶ�������һ�����Ҳಢ����൥λ���1���˺�
    /// </summary>
    public class Card2013006 : Card
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
                  await GameSystem.TransferSystem.MoveCard(new TriggerInfoModel(this, this).SetLocation(OppositeOrientation, NextBattleRegion, -1));
                  await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, this.LeftCard).SetPoint(1));
              }, Condition.Default)
              .AbilityAppend();
        }
    }
}