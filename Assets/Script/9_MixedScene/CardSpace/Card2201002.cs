using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:������
    /// ��������:�ҷ��غϽ���ʱ���������Ҳ���ڵ�λ���������ƶ�һ��λ�ò�����Է�һ���˺������ͬ����Ч�����ܻ���bug��
    /// </summary>
    public class Card2201002 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new Event(this, this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
               .AbilityAdd(async (e) =>
               {
                   if (RightCard != null)
                   {
                       await GameSystem.TransferSystem.MoveCard(new Event(this, this).SetLocation(RightCard.CurrentOrientation, RightCard.CurrentRegion, RightCard.CurrentIndex));
                       await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Battle][CardRank.NoGold].CardList, 1, true);
                       await GameSystem.PointSystem.Hurt(new Event(this, GameSystem.InfoSystem.SelectUnits).SetPoint(1).SetBullet(new BulletModel()));
                   }
               }, Condition.Default,Condition.OnMyTurn)
               .AbilityAppend();
        }
    }
}