using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��˳֮ͷ
    /// ��������:���ҷ�ͬ�����Ҳ�����һ��ͷ��ÿ�غϿ�ʼ����൥����һ������
    /// </summary>
    public class Card2013007 : Card
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
                 await GameSystem.TransferSystem.MoveCard(new TriggerInfoModel(this, this).SetLocation(CurrentOrientation, NextBattleRegion, -1));
                 await GameSystem.PointSystem.Gain(new TriggerInfoModel(this, this.LeftCard).SetPoint(1));
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}