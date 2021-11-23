using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card20016 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectBoardCard(this,AgainstInfo.cardSet[Orientation.My][GameRegion.Grave][CardRank.Copper][CardTag.Fairy][CardType.Unite].CardList);
                   await GameSystem.TransSystem.MoveToGrave(new TriggerInfo(this).SetTargetCard(this));
                   await GameSystem.TransSystem.ReviveCard(new TriggerInfo(this).SetTargetCard(AgainstInfo.selectActualCards));
               })
               .AbilityAppend();
        }
    }
}