using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��֮ͷ
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
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this,this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
              .AbilityAdd(async (triggerInfo) =>
              {
                  await GameSystem.TransSystem.MoveCard(new TriggerInfoModel(this, this).SetLocation(OppositeOrientation, NextBattleRegion, -1));
                  await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, this.LeftCard).SetPoint(1));
              }, Condition.Default)
              .AbilityAppend();
        }
    }
}