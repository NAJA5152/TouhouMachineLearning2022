using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:������а
    /// ��������:ÿ���������������һ����λʱ�����䲿��Ч����Чǰ��ѡ���ҷ�����һ���ǽ�λ�����������
    /// </summary>
    public class Card2010001 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.UiSystem.ShowFigure(this);
                   await GameSystem.TransferSystem.DeployCard(new Event(this, this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.Before, TriggerType.Deploy)
               .AbilityAdd(async (e) =>
               {
                   List<Card> targetCards = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Silver, CardRank.Copper].CardList;
                   targetCards.Remove(e.triggerCard);
                   await GameSystem.SelectSystem.SelectUnit(this, targetCards, 1);
                   await GameSystem.PointSystem.Reversal(new Event(e.triggerCard, GameSystem.InfoSystem.SelectUnits));
               }, Condition.Default, Condition.OnMyTurn)
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
             .AbilityAdd(async (e) =>
             {
                 await GameSystem.TransferSystem.GenerateCard(new Event(this, targetCard: null).SetTargetCardId(2013006).SetLocation(CurrentOrientation, CurrentRegion, -1));
                 await GameSystem.TransferSystem.GenerateCard(new Event(this, targetCard: null).SetTargetCardId(2013007).SetLocation(OppositeOrientation, CurrentRegion, -1));
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}