using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:�漣*ʥ������
    /// ��������:��Ĺ����ѡ��һ��С�ڵ���������ֵ�ķǽ�λ����������
    /// </summary>
    public class Card2202002 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {

                   var cardList = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck][CardRank.Copper]
                        .CardList
                        .Where(card => card.ShowPoint <= this[CardField.Pary])
                        .ToList();
                   await GameSystem.SelectSystem.SelectBoardCard(this, cardList);
                   await GameSystem.TransferSystem.PlayCard(new Event(this, GameSystem.InfoSystem.SelectUnit));
                   await GameSystem.TransferSystem.MoveToGrave(this);

               })
               .AbilityAppend();
        }
    }
}