using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using static TouhouMachineLearningSummary.Info.AgainstInfo;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:战略转移
    /// </summary>
    public class Card2003002 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Copper][GameEnum.CardTag.Fairy].CardList, 1);
                   await GameSystem.TransSystem.RecycleCard(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits));

                   await GameSystem.TransSystem.PlayCard(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits));
                   await GameSystem.TransSystem.MoveToGrave(new TriggerInfoModel(this, this));
               })
               .AbilityAppend();
        }
    }
}