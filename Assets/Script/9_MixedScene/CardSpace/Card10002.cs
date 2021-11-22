using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.CardSpace
{
    public partial class Card10002 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this,region,territory);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfo(this).SetTargetCard(this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
                .AbilityAdd(async (triggerInfo) =>
                {
                    // await GameSystem.TransSystem.MoveCard(new TriggerInfo(this).SetTargetCard(AgainstInfo.cardSet[Orientation.Op].CardList).SetLocation(Orientation.Op, currentRegion, -1).SetMeanWhile());
                    List<Task> tasks = new List<Task>();
                    tasks.Add(GameSystem.TransSystem.MoveCard(new TriggerInfo(this).SetTargetCard(AgainstInfo.cardSet[Orientation.Op][GameRegion.Water].CardList).SetLocation(Orientation.Op, GameRegion.Water, 0)));
                    tasks.Add(GameSystem.TransSystem.MoveCard(new TriggerInfo(this).SetTargetCard(AgainstInfo.cardSet[Orientation.Op][GameRegion.Fire].CardList).SetLocation(Orientation.Op, GameRegion.Fire, -1)));
                    tasks.Add(GameSystem.TransSystem.MoveCard(new TriggerInfo(this).SetTargetCard(AgainstInfo.cardSet[Orientation.Op][GameRegion.Wind].CardList).SetLocation(Orientation.Op, GameRegion.Wind, -1)));
                    tasks.Add(GameSystem.TransSystem.MoveCard(new TriggerInfo(this).SetTargetCard(AgainstInfo.cardSet[Orientation.Op][GameRegion.Soil].CardList).SetLocation(Orientation.Op, GameRegion.Soil, 0)));
                    tasks.Add(GameSystem.TransSystem.MoveCard(new TriggerInfo(this).SetTargetCard(AgainstInfo.cardSet[Orientation.My][GameRegion.Wind].CardList).SetLocation(Orientation.My, GameRegion.Wind, -1)));
                    await Task.WhenAll(tasks.ToArray());
                }, Condition.Default)
                .AbilityAppend();
        }
    }
}
