using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:����
    /// ��������:�������д������С��������ֵ����ӽ���ͭɫ��λ
    /// </summary>
    public class Card2203003 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   var targetCard = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck][CardRank.Copper]
                   .CardList
                   .Where(card => card.ShowPoint <= this[CardField.Pary])
                   .OrderBy(card => card.ShowPoint)
                   .FirstOrDefault();

                   await GameSystem.TransferSystem.PlayCard(new Event(this, targetCard));
                   await GameSystem.TransferSystem.MoveToGrave(this);

               })
               .AbilityAppend();
        }
    }
}