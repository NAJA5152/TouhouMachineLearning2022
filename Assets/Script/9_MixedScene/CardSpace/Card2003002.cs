using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using static TouhouMachineLearningSummary.Info.AgainstInfo;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:ս��ת��
    /// ��������:����:������һ��ͭɫ��λ���뿨�飬ͬʱ��������е�����͵�ͭɫ��λ
    /// </summary>
    public class Card2003002 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Copper].CardList, 1);
                   await GameSystem.TransferSystem.MoveToDeck(GameSystem.InfoSystem.SelectUnit);

                   await GameSystem.TransferSystem.PlayCard(new Event(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck][CardRank.Copper][CardFeature.LowestUnits].CardList));
                   await GameSystem.TransferSystem.MoveToGrave(this);
               })
               .AbilityAppend();
        }
    }
}