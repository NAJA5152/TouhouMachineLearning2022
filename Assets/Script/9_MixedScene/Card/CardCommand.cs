﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    //具体实现，还需进一步简化
    public static class CardCommand
    {
        public static void OrderHandCard()
        {
            AgainstInfo.cardSet[GameRegion.Hand].RowManagers.ForEach(singleRowInfo =>
            {
                AgainstInfo.cardSet[singleRowInfo.RowRank] = AgainstInfo.cardSet[singleRowInfo.RowRank].OrderByDescending(card => card.CardRank).ThenBy(card => card.BasePoint).ThenBy(card => card.CardID).ToList();
            });
        }
        //令卡牌自由
        public static void FreeCard(Card card) => card.BelongCardList.Remove(card);
        public static Card GenerateCard(int id)
        {
            GameObject newCard = GameObject.Instantiate(Info.CardInfo.cardModel, new Vector3(0, 100, 0), Info.CardInfo.cardModel.transform.rotation);
            newCard.transform.SetParent(GameObject.FindGameObjectWithTag("Card").transform);
            newCard.SetActive(true);
            newCard.name = "Card" + Info.CardInfo.CreatCardRank++;
            //Debug.Log("创建卡牌"+id);


            //若编辑器下则直接加载本地GameCard的dll卡牌脚本
            //否则从数据库下载卡牌脚本
            if (Application.isEditor)
            {
                //通过反射加载
                Type componentType = Assembly.Load(File.ReadAllBytes(@"Library\ScriptAssemblies\GameCard.dll")).GetType("TouhouMachineLearningSummary.CardSpace.Card" + id);
                //Type componentType = Type.GetType("TouhouMachineLearningSummary.CardSpace.Card" + id);
                //直接加载
                newCard.AddComponent(componentType);
            }
            else
            {
                newCard.AddComponent(Manager.CardAssemblyManager.GetCardScript(id));
            }

            var CardStandardInfo = CardAssemblyManager.GetCurrentCardInfos(id);
            Card card = newCard.GetComponent<Card>();
            card.CardID = CardStandardInfo.cardID;
            card.BasePoint = CardStandardInfo.point;
            card.Icon = CardStandardInfo.icon;
            card.CardDeployRegion = CardStandardInfo.cardDeployRegion;
            card.CardDeployTerritory = CardStandardInfo.cardDeployTerritory;
            card.TranslateTags = CardStandardInfo.TranslateTags;
            card.CardRank = CardStandardInfo.cardRank;
            card.CardType = CardStandardInfo.cardType;
            card.GetComponent<Renderer>().material.SetTexture("_Front", card.Icon);
            switch (card.CardRank)
            {
                case CardRank.Leader: card.GetComponent<Renderer>().material.SetColor("_side", new Color(0.43f, 0.6f, 1f)); break;
                case CardRank.Gold: card.GetComponent<Renderer>().material.SetColor("_side", new Color(0.8f, 0.8f, 0f)); break;
                case CardRank.Silver: card.GetComponent<Renderer>().material.SetColor("_side", new Color(0.75f, 0.75f, 0.75f)); break;
                case CardRank.Copper: card.GetComponent<Renderer>().material.SetColor("_side", new Color(1, 0.42f, 0.37f)); break;
                default: break;
            }
            card.Init();
            return card;
        }
        /// <summary>
        /// 从对战记录的简易卡牌模型复现原卡牌
        /// </summary>
        /// <param name="sampleCard"></param>
        /// <returns></returns>
        public static Card GenerateCard(SampleCardModel sampleCard)
        {

            GameObject newCard = GameObject.Instantiate(Info.CardInfo.cardModel, new Vector3(0, 100, 0), Info.CardInfo.cardModel.transform.rotation);
            newCard.SetActive(true);
            newCard.transform.SetParent(GameObject.FindGameObjectWithTag("Card").transform);
            newCard.name = "Card" + CardInfo.CreatCardRank++;
            //Debug.Log("创建卡牌"+id);
            newCard.AddComponent(CardAssemblyManager.GetCardScript(sampleCard.CardID));
            Card card = newCard.GetComponent<Card>();
            var CardStandardInfo = CardAssemblyManager.GetCurrentCardInfos(sampleCard.CardID);
            ///然后根据sampleCard设置具体参数，先暂时设为默认
            card.CardID = CardStandardInfo.cardID;
            card.BasePoint = CardStandardInfo.point;
            card.Icon = CardStandardInfo.icon;
            card.CardDeployRegion = CardStandardInfo.cardDeployRegion;
            card.CardDeployTerritory = CardStandardInfo.cardDeployTerritory;
            card.TranslateTags = CardStandardInfo.TranslateTags;
            card.CardRank = CardStandardInfo.cardRank;
            card.CardType = CardStandardInfo.cardType;
            card.GetComponent<Renderer>().material.SetTexture("_Front", card.Icon);
            switch (card.CardRank)
            {
                case CardRank.Leader: newCard.GetComponent<Renderer>().material.SetColor("_side", new Color(0.43f, 0.6f, 1f)); break;
                case CardRank.Gold: newCard.GetComponent<Renderer>().material.SetColor("_side", new Color(0.8f, 0.8f, 0f)); break;
                case CardRank.Silver: newCard.GetComponent<Renderer>().material.SetColor("_side", new Color(0.75f, 0.75f, 0.75f)); break;
                case CardRank.Copper: newCard.GetComponent<Renderer>().material.SetColor("_side", new Color(1, 0.42f, 0.37f)); break;
                default: break;
            }
            card.Init();
            return card;
        }
        public static async Task BanishCard(Card card) => await card.ThisCardManager.CreatGapAsync();
        public static async Task SummonCard(Card targetCard)
        {
            List<Card> TargetRow = AgainstInfo
                .cardSet[(GameRegion)targetCard.CardDeployRegion][targetCard.CurrentOrientation]
                .RowManagers.First().CardList;
            Debug.LogWarning("召唤卡牌于" + targetCard.CurrentOrientation);
            FreeCard(targetCard);
            TargetRow.Add(targetCard);
            targetCard.IsCanSee = true;
            //targetCard.moveSpeed = 0.1f;
            targetCard.isMoveStepOver = false;
            await Task.Delay(200);
            targetCard.isMoveStepOver = true;
            //targetCard.moveSpeed = 0.1f;
            await SoundEffectCommand.PlayAsync(SoundEffectType.DrawCard);
        }
        public static async Task MoveCard(Card targetCard, Location location)
        {
            FreeCard(targetCard);
            SingleRowManager TargetRow = Info.AgainstInfo.cardSet.RowManagers[location.X];
            AgainstInfo.cardSet[TargetRow.orientation][TargetRow.region].Add(targetCard, location.Y);
            targetCard.isMoveStepOver = false;
            await Task.Delay(200);
            targetCard.isMoveStepOver = true;
            _ = SoundEffectCommand.PlayAsync(SoundEffectType.DrawCard);
        }



        public static async Task DeployCard(Card targetCard, bool reTrigger)
        {
            if (!reTrigger)
            {
                FreeCard(targetCard);
                AgainstInfo.SelectRowCardList.Insert(AgainstInfo.SelectRank, targetCard);
            }
            targetCard.isMoveStepOver = false;
            await Task.Delay(200);
            targetCard.isMoveStepOver = true;
            _ = SoundEffectCommand.PlayAsync(SoundEffectType.Deploy);
        }
        /// <summary>
        /// 洗牌
        /// </summary>
        /// <param name="targetCard">要洗的卡牌</param>
        /// <param name="IsPlayerExchange">是否操控当前玩家洗牌</param>
        /// <param name="isRoundStartExchange">是否回合开始洗牌</param>
        /// <param name="WashInsertRank">洗入位置</param>
        /// <returns></returns>
        public static async Task ExchangeCard(Card targetCard, bool IsPlayerExchange = true, bool isRoundStartExchange = false, int WashInsertRank = 0)
        {
            //Debug.Log("交换卡牌");
            await WashCard(targetCard, IsPlayerExchange, WashInsertRank);
            await DrawCard(IsPlayerExchange, true);
            if (IsPlayerExchange)
            {
                CardBoardCommand.LoadBoardCardList(AgainstInfo.cardSet[isRoundStartExchange ? Orientation.Down : Orientation.My][GameRegion.Hand].CardList, CardBoardMode.ExchangeCard);
            }
        }
        internal static async Task RebackCard(Card targetCard)
        {
            //还清空卡牌状态
            await PlayCard(targetCard);
        }
        public static async Task DrawCard(bool isPlayerDraw = true, bool ActiveBlackList = false, bool isOrder = true)
        {
            //Debug.Log("抽卡");
            _ = SoundEffectCommand.PlayAsync(SoundEffectType.DrawCard);
            Card TargetCard = AgainstInfo.cardSet[isPlayerDraw ? Orientation.Down : Orientation.Up][GameRegion.Deck].CardList.FirstOrDefault();
            if (TargetCard == null)
            {
                Debug.LogError("无法进行抽卡");
            }
            else
            {
                TargetCard.IsCanSee = isPlayerDraw;
                CardSet TargetCardtemp = AgainstInfo.cardSet[isPlayerDraw ? Orientation.Down : Orientation.Up][GameRegion.Deck];

                AgainstInfo.cardSet[isPlayerDraw ? Orientation.Down : Orientation.Up][GameRegion.Deck].Remove(TargetCard);
                AgainstInfo.cardSet[isPlayerDraw ? Orientation.Down : Orientation.Up][GameRegion.Hand].Add(TargetCard);
            }
            if (isOrder)
            {
                OrderHandCard();
            }
            await Task.Delay(100);
        }
        public static async Task WashCard(Card TargetCard, bool IsPlayerWash = true, int InsertRank = 0)
        {
            Debug.Log("洗回卡牌");
            if (IsPlayerWash)
            {
                AgainstInfo.TargetCard = TargetCard;
                int MaxCardRank = AgainstInfo.cardSet[Orientation.Down][GameRegion.Deck].CardList.Count;
                AgainstInfo.washInsertRank = AiCommand.GetRandom(0, MaxCardRank);
                NetCommand.AsyncInfo(NetAcyncType.ExchangeCard);
                AgainstInfo.cardSet[Orientation.Down][GameRegion.Hand].Remove(TargetCard);
                AgainstInfo.cardSet[Orientation.Down][GameRegion.Deck].Add(TargetCard, AgainstInfo.washInsertRank);
                TargetCard.IsCanSee = false;
            }
            else
            {
                AgainstInfo.cardSet[Orientation.Up][GameRegion.Hand].Remove(TargetCard);
                AgainstInfo.cardSet[Orientation.Up][GameRegion.Deck].Add(TargetCard, InsertRank);
            }
            await Task.Delay(500);
        }

        public static async Task GenerateCard(Card targetCard, Location location)
        {
            _ = SoundEffectCommand.PlayAsync(SoundEffectType.DrawCard);
            RowCommand.SetPlayCardMoveFree(false);
            targetCard.IsCanSee = true;

            SingleRowManager TargetRow = Info.AgainstInfo.cardSet.RowManagers[location.X];
            AgainstInfo.cardSet[TargetRow.orientation][TargetRow.region].Add(targetCard, location.Y);
            if (TargetRow.CardList.Count > 6 && (
                TargetRow.region == GameRegion.Water ||
                TargetRow.region == GameRegion.Fire ||
                TargetRow.region == GameRegion.Wind ||
                TargetRow.region == GameRegion.Soil
                ))
            {
                await GameSystem.TransferSystem.MoveToGrave(targetCard);
            }
        }
        public static async Task PlayCard(Card targetCard)
        {
            await Task.Delay(0);//之后实装卡牌特效需要时间延迟配合
            _ = SoundEffectCommand.PlayAsync(SoundEffectType.DrawCard);
            RowCommand.SetPlayCardMoveFree(false);
            NetCommand.AsyncInfo(NetAcyncType.PlayCard);
            ChainManager.ShowChainCount();

            targetCard.IsCanSee = true;
            targetCard.isPrepareToPlay = false;

            FreeCard(targetCard);
            AgainstInfo.cardSet[Orientation.My][GameRegion.Used].Add(targetCard);
            AgainstInfo.playerPlayCard = null;
        }
        public static async Task DisCard(Card card)
        {
            await Task.Delay(0);//之后实装卡牌特效需要时间延迟配合
            _ = SoundEffectCommand.PlayAsync(SoundEffectType.DrawCard);
            RowCommand.SetPlayCardMoveFree(false);
            NetCommand.AsyncInfo(NetAcyncType.DisCard);

            card.IsCanSee = false;
            card.isPrepareToPlay = false;

            FreeCard(card);
            AgainstInfo.cardSet[Orientation.My][GameRegion.Grave].Add(card);
            AgainstInfo.playerDisCard = null;
        }

        public static async Task ReviveCard(Card card)
        {
            //墓地复活动画

            await PlayCard(card);
            //Card card = e.targetCard;
            //await SoundEffectCommand.PlayAsync(SoundEffectType.DrawCard);
            //card.IsCanSee = true;
            //FreeCard(card);
            //AgainstInfo.cardSet[Orientation.My][GameRegion.Used].Add(card);
            //await card.cardAbility[TriggerTime.When][TriggerType.Play][0](e);
        }

        public static async Task SealCard(Card card)
        {
            card.transform.GetChild(2).gameObject.SetActive(true);
        }
        public static async Task UnSealCard(Card card)
        {
            card.transform.GetChild(2).gameObject.SetActive(false);
        }
        public static async Task Set(Event e)
        {
            await BulletCommand.InitBulletAsync(e);
            //await Task.Delay(1000);
            int actualChangePoint = e.point - e.targetCard.ShowPoint;

            await e.targetCard.ThisCardManager.ShowTips((actualChangePoint > 0 ? "+" : "") + actualChangePoint, Color.gray, false);
            e.targetCard.ChangePoint = e.point - e.targetCard.BasePoint;
            //await Task.Delay(1000);
        }
        public static async Task Gain(Event e)
        {
            await BulletCommand.InitBulletAsync(e);
            //await Task.Delay(1000);
            int actualChangePoint = e.point;
            await e.targetCard.ThisCardManager.ShowTips("+" + actualChangePoint, Color.green, false);
            e.targetCard.ChangePoint += e.point;
            await Task.Delay(1000);
        }
        public static async Task Hurt(Event e)
        {
            await BulletCommand.InitBulletAsync(e);
            //抵消护盾
            //悬浮伤害数字
            int actualChangePoint = Math.Min(e.point, e.targetCard.ShowPoint);
            await e.targetCard.ThisCardManager.ShowTips("-" + actualChangePoint, Color.red, false);
            e.targetCard.ChangePoint -= actualChangePoint;
            await Task.Delay(1000);
        }
        //逆转
        public static async Task Reversal(Event e)
        {
            int triggerCardPoint = e.triggerCard.ShowPoint;
            int targetCardPoint = e.targetCard.ShowPoint;
            _ = GameSystem.PointSystem.Set(new Event(e.triggerCard, e.targetCard).SetPoint(triggerCardPoint));
            _ = GameSystem.PointSystem.Set(new Event(e.targetCard, e.triggerCard).SetPoint(targetCardPoint));
            await Task.Delay(1000);
        }
        public static async Task MoveToDeck(Card card, int Index = 0, bool isRandom = false)
        {
            if (card == null) return;
            await Task.Delay(500);
            Orientation targetOrientation = card.CurrentOrientation;
            FreeCard(card);
            AgainstInfo.cardSet[targetOrientation][GameRegion.Deck].RowManagers[0].CardList.Insert(Index, card);

            //重置卡牌状态
            card.cardFields.Clear();
            card.cardStates.Clear();
            card.IsCanSee = false;
            card.ChangePoint = 0;
            card.isMoveStepOver = false;
            await Task.Delay(100);
            card.isMoveStepOver = true;
            await SoundEffectCommand.PlayAsync(SoundEffectType.DrawCard);
        }

        public static async Task MoveToOpHand(Card card)
        {
            if (card == null) return;
            await Task.Delay(500);
            Orientation targetOrientation = card.OppositeOrientation;
            FreeCard(card);
            AgainstInfo.cardSet[targetOrientation][GameRegion.Hand].RowManagers[0].CardList.Insert(0, card);
            OrderHandCard();
            //重置卡牌状态
            card.IsCanSee = (targetOrientation == Orientation.Down);
            card.ChangePoint = 0;
            card.isMoveStepOver = false;
            await Task.Delay(100);
            card.isMoveStepOver = true;
            await SoundEffectCommand.PlayAsync(SoundEffectType.DrawCard);
        }
        public static async Task MoveToGrave(Card card)
        {
            if (card == null) return;
            await Task.Delay(500);
            Orientation targetOrientation = card.CurrentOrientation;
            FreeCard(card);
            AgainstInfo.cardSet[targetOrientation][GameRegion.Grave].RowManagers[0].CardList.Insert(0, card);

            //重置卡牌状态
            card.cardFields.Clear();
            card.cardStates.Clear();
            card.IsCanSee = false;
            card.ChangePoint = 0;
            card.isMoveStepOver = false;
            await Task.Delay(100);
            card.isMoveStepOver = true;
            await SoundEffectCommand.PlayAsync(SoundEffectType.DrawCard);
        }
    }
}