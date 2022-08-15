using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:����ز�
    /// ��������:ѡ����һ��ͭɫ��λ���������´��
    /// </summary>
    public class Card2013001 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[CardRank.Silver][CardRank.Copper][CardType.Unit].CardList, 1);
                   await GameSystem.TransferSystem.ReviveCard(new Event(this, GameSystem.InfoSystem.SelectUnit));
                   await GameSystem.TransferSystem.PlayCard(new Event(GameSystem.InfoSystem.SelectUnit, GameSystem.InfoSystem.SelectUnit));
                   await GameSystem.TransferSystem.MoveToGrave(this);
               })
               .AbilityAppend();
        }
    }
}