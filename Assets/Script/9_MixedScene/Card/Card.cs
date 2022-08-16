using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary
{


    public class Card : MonoBehaviour
    {
        public Manager.CardManager ThisCardManager => GetComponent<Manager.CardManager>();
        public int CardID { get; set; }

        public int BasePoint { get; set; }
        public int ChangePoint { get; set; }
        public int ShowPoint => Mathf.Max(0, BasePoint + ChangePoint);

        public Texture2D Icon { get; set; }
        public float MoveSpeed { get; set; } = 0.1f;
        //卡牌默认可部署属性区域
        public BattleRegion CardDeployRegion { get; set; }
        //卡牌默认可部署所属
        public Territory CardDeployTerritory { get; set; }
        //卡牌当前可部署所属
        public Territory CardCurrentTerritory => AgainstInfo.cardSet[Orientation.Down].CardList.Contains(this) ? Territory.My : Territory.Op;
        [ShowInInspector]
        //卡牌默认可部署所属
        public Orientation CurrentOrientation => AgainstInfo.cardSet[Orientation.Down].CardList.Contains(this) ? Orientation.Down : Orientation.Up;
        public Orientation OppositeOrientation => CurrentOrientation == Orientation.Down ? Orientation.Up : Orientation.Down;
        //获取全局牌表区域
        public GameRegion CurrentRegion => AgainstInfo.cardSet.RowManagers.First(row => row.CardList.Contains(this)).region;
        //卡牌的当前顺序
        public int CurrentIndex => AgainstInfo.cardSet.RowManagers.First(row => row.CardList.Contains(this)).CardList.IndexOf(this);

        //按水->土->风->火->水的顺序获取下一个区域属性
        public GameRegion LastBattleRegion => CurrentRegion switch
        {
            GameRegion.Water => GameRegion.Soil,
            GameRegion.Fire => GameRegion.Water,
            GameRegion.Wind => GameRegion.Fire,
            GameRegion.Soil => GameRegion.Wind,
            _ => CurrentRegion,
        };
        //按水->火->风->土->水的顺序获取下一个区域属性
        public GameRegion NextBattleRegion => CurrentRegion switch
        {
            GameRegion.Water => GameRegion.Fire,
            GameRegion.Fire => GameRegion.Wind,
            GameRegion.Wind => GameRegion.Soil,
            GameRegion.Soil => GameRegion.Water,
            _ => CurrentRegion,
        };
        public string TranslateTags { get; set; }
        public CardRank CardRank { get; set; }
        public CardType CardType { get; set; }


        [ShowInInspector]
        public Dictionary<CardField, int> cardFields = new Dictionary<CardField, int>();
        public int this[CardField cardField]
        {
            get => cardFields.ContainsKey(cardField) ? cardFields[cardField] : 0;
            set
            {
                cardFields[cardField] = value;
                if (value <= 0)
                {
                    cardFields.Remove(cardField);
                }
            }
        }
        [ShowInInspector]
        public List<CardState> cardStates = new List<CardState>();
        public bool this[CardState cardState]
        {
            get => cardStates.Contains(cardState);
            set
            {
                if (value)
                {
                    if (!cardStates.Contains(cardState))
                    {
                        cardStates.Add(cardState);
                    }
                }
                else
                {
                    DeckConfigCommand
                    cardStates.Remove(cardState);
                }
            }
        }

        public Vector3 targetPosition;
        public Quaternion targetQuaternion;

        //状态相关参数
        public bool IsInit { get; set; } = false;
        public bool IsGray { get; set; } = false;
        public bool IsCover { get; set; } = false;
        public bool IsLocked { get; set; } = false;
        public bool IsFree { get; set; } = false;
        //后期补充，揭示状态始终显示，覆盖状态始终不显示
        public bool IsCanSee { get => isCanSee && !this[CardState.Close] || this[CardState.Pry]; set => isCanSee = value; }
        public bool IsCardReadyToGrave => ShowPoint == 0 && AgainstInfo.cardSet[GameRegion.Battle].CardList.Contains(this);
        public bool isMoveStepOver = true;
        public bool isPrepareToPlay = false;
        public bool IsAutoMove => this != AgainstInfo.playerPrePlayCard;

        public List<Card> BelongCardList => Command.RowCommand.GetCardList(this);
        public Location Location => Command.RowCommand.GetLocation(this);
        public Card LeftCard => Location.Y > 0 ? BelongCardList[Location.Y - 1] : null;
        public Card RightCard => Location.Y < BelongCardList.Count - 1 ? BelongCardList[Location.Y + 1] : null;
        public List<Card> TwoSideCard
        {
            get
            {
                var targetList = new List<Card>();
                if (LeftCard != null) targetList.Add(LeftCard);
                if (RightCard != null) targetList.Add(RightCard);
                return targetList;
            }
        }

        public Text PointText => transform.GetChild(0).GetChild(0).GetComponent<Text>();
        public Transform FieldIconContent => transform.GetChild(0).GetChild(1);
        public Transform StateIconContent => transform.GetChild(0).GetChild(2);
        [ShowInInspector]
        public string TranslateName => Manager.CardAssemblyManager.GetCurrentCardInfos(CardID).TranslateName;
        [ShowInInspector]
        public string TranslateAbility => Manager.CardAssemblyManager.GetCurrentCardInfos(CardID).TranslateAbility;
        [ShowInInspector]
        public string TranslateDescribe => Manager.CardAssemblyManager.GetCurrentCardInfos(CardID).TranslateDescribe;



        public Dictionary<TriggerTime, Dictionary<TriggerType, List<Func<Event, Task>>>> cardAbility = new Dictionary<TriggerTime, Dictionary<TriggerType, List<Func<Event, Task>>>>();
        private bool isCanSee = false;


        /// <summary>
        /// 注册默认共通的卡牌效果
        /// </summary>
        public virtual void Init()
        {
            ///////////////////////////////////////////////////////初始化////////////////////////////////////////////////////////////////////////

            IsInit = true;
            //初始化卡牌效果并填充空效果
            foreach (TriggerTime tirggerTime in Enum.GetValues(typeof(TriggerTime)))
            {
                cardAbility[tirggerTime] = new Dictionary<TriggerType, List<Func<Event, Task>>>();
                foreach (TriggerType triggerType in Enum.GetValues(typeof(TriggerType)))
                {
                    cardAbility[tirggerTime][triggerType] = new List<Func<Event, Task>>();
                }
            }
            //////////////////////////////////////////////////编写默认被动效果///////////////////////////////////////////////////////////////////

            ///////////////////////////////////////////////////////所属移动///////////////////////////////////////////////////////////////////////
            //当创造时从牌库中构建
            AbalityRegister(TriggerTime.When, TriggerType.Generate)
             .AbilityAdd(async (e) => { await Command.CardCommand.GenerateCard(e.targetCard, e.location); })
             .AbilityAppend();
            //当弃牌时移动至墓地
            AbalityRegister(TriggerTime.When, TriggerType.Discard)
              .AbilityAdd(async (e) => { await Command.CardCommand.DisCard(e.targetCard); })
              .AbilityAppend();
            //当死亡时移至墓地
            AbalityRegister(TriggerTime.When, TriggerType.Dead)
            .AbilityAdd(async (e) => { await Command.CardCommand.MoveToGrave(this); })
            .AbilityAppend();
            //当复活时移至出牌区
            AbalityRegister(TriggerTime.When, TriggerType.Revive)
            .AbilityAdd(async (e) => { await Command.CardCommand.ReviveCard(e.targetCard); })
            .AbilityAppend();
            //当移动时移到指定位置
            AbalityRegister(TriggerTime.When, TriggerType.Move)
            .AbilityAdd(async (e) => { await Command.CardCommand.MoveCard(e.targetCard, e.location); })
            .AbilityAppend();
            //当间隙时从游戏中除外
            AbalityRegister(TriggerTime.When, TriggerType.Banish)
            .AbilityAdd(async (e) => { await Command.CardCommand.BanishCard(this); })
            .AbilityAppend();
            //当召唤时从卡组中拉出
            AbalityRegister(TriggerTime.When, TriggerType.Summon)
            .AbilityAdd(async (e) => { await Command.CardCommand.SummonCard(this); })
            .AbilityAppend();

            ///////////////////////////////////////////////////////点数变动移动///////////////////////////////////////////////////////////////////////

            //当设置点数时，修改变更点数值
            AbalityRegister(TriggerTime.When, TriggerType.Set)
            .AbilityAdd(async (e) =>
            {
                var targetShowPoint = e.targetCard.ShowPoint;
                await Command.CardCommand.Set(e);
                //如果原本点数大于设置点数
                if (targetShowPoint < e.point)
                {
                    await GameSystem.PointSystem.Increase(e);
                }
                if (targetShowPoint > e.point)
                {
                    await GameSystem.PointSystem.Decrease(e);
                }
            })
            .AbilityAppend();
            //当获得增益时获得点数增加
            AbalityRegister(TriggerTime.When, TriggerType.Gain)
            .AbilityAdd(async (e) =>
            {
                await Command.CardCommand.Gain(e);
                if (e.point > 0)//如果不处于死亡状态
                {
                    await GameSystem.PointSystem.Increase(e);
                }
            })
            .AbilityAppend();
            //默认受伤效果：当卡牌受到伤害时则会受到伤害，当卡牌死亡时，触发卡牌死亡机制
            AbalityRegister(TriggerTime.When, TriggerType.Hurt)
            .AbilityAdd(async (e) =>
            {
                if (this[CardState.Congealbounds])
                {
                    await GameSystem.StateSystem.ClearState(new Event(e.triggerCard, this));
                }
                else
                {
                    if (this[CardField.Shield] > 0)
                    {
                        //计算剩余盾量
                        var shieldPoint = this[CardField.Shield] - e.point;
                        //计算剩余伤害
                        e.point = e.point - this[CardField.Shield];
                        //调整护盾值
                        await GameSystem.FieldSystem.SetField(new Event(e.triggerCard, this).SetPoint(shieldPoint));
                    }
                    await Command.CardCommand.Hurt(e);
                    if (e.point > 0)
                    {
                        await GameSystem.PointSystem.Decrease(e);
                    }
                }
            })
            .AbilityAppend();
            //当治愈时
            AbalityRegister(TriggerTime.When, TriggerType.Cure)
            .AbilityAdd(async (e) =>
            {
                e.point = -Math.Min(0, e.targetCard.ChangePoint);
                await Command.CardCommand.Gain(e);
            })
           .AbilityAppend();
            //当被摧毁时以自身点数对自己造成伤害
            AbalityRegister(TriggerTime.When, TriggerType.Destory)
                .AbilityAdd(async (e) =>
                {
                    e.point = ShowPoint;
                    await GameSystem.PointSystem.Hurt(e);
                })
               .AbilityAppend();
            //当点数逆转时触发
            AbalityRegister(TriggerTime.When, TriggerType.Reverse)
                .AbilityAdd(async (e) => { await Command.CardCommand.Reversal(e); })
               .AbilityAppend();

            ///////////////////////////////////////////////////////附加状态///////////////////////////////////////////////////////////////////////
            //卡牌状态附加时效果
            AbalityRegister(TriggerTime.When, TriggerType.StateAdd)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.UiSystem.ShowIcon(this, e.targetState);
                    await Command.SoundEffectCommand.PlayAsync(SoundEffectType.CardSelect);
                    switch (e.targetState)
                    {
                        case CardState.Lurk:; break;
                        case CardState.Seal: await Command.CardCommand.SealCard(this); break;
                        case CardState.None:
                            break;
                        case CardState.Invisibility:
                            break;
                        case CardState.Pry:
                            break;
                        case CardState.Close:
                            break;
                        case CardState.Fate:
                            break;
                        case CardState.Furor:
                            await GameSystem.UiSystem.ShowTips(this, "狂躁", new Color(0.6f, 0.2f, 0));
                            break;
                        case CardState.Docile:
                            await GameSystem.UiSystem.ShowTips(this, "温顺", new Color(0, 0, 0.5f));
                            break;
                        case CardState.Poisoning:
                            break;
                        case CardState.Rely:
                            break;
                        case CardState.Water:
                            if (cardStates.Contains(CardState.Water))
                            {
                                cardStates.Remove(CardState.Water);
                                await GameSystem.UiSystem.ShowTips(this, "治愈", new Color(0, 1, 0));
                            }
                            else if (cardStates.Contains(CardState.Fire))
                            {
                                cardStates.Remove(CardState.Fire);
                                await GameSystem.UiSystem.ShowTips(this, "中和", new Color(0, 1, 0));
                            }
                            else if (cardStates.Contains(CardState.Wind))
                            {
                                cardStates.Remove(CardState.Wind);
                                await GameSystem.UiSystem.ShowTips(this, "扩散", new Color(0, 1, 0));

                            }
                            else if (cardStates.Contains(CardState.Soil))
                            {
                                cardStates.Remove(CardState.Soil);
                                await GameSystem.UiSystem.ShowTips(this, "泥泞", new Color(1, 1, 0));
                            }
                            else
                            {
                                cardStates.Add(CardState.Water);
                            }
                            return;
                        case CardState.Fire:
                            if (cardStates.Contains(CardState.Water))
                            {
                                cardStates.Remove(CardState.Water);
                                await GameSystem.UiSystem.ShowTips(this, "中和", new Color(0, 1, 0));
                            }
                            else if (cardStates.Contains(CardState.Fire))
                            {
                                cardStates.Remove(CardState.Fire);
                                await GameSystem.UiSystem.ShowTips(this, "烧灼", new Color(0, 1, 0));
                            }
                            else if (cardStates.Contains(CardState.Wind))
                            {
                                cardStates.Remove(CardState.Wind);
                                await GameSystem.UiSystem.ShowTips(this, "扩散", new Color(0, 1, 0));

                            }
                            else if (cardStates.Contains(CardState.Soil))
                            {
                                cardStates.Remove(CardState.Soil);
                                await GameSystem.UiSystem.ShowTips(this, "熔岩", new Color(1, 1, 0));
                            }
                            else
                            {
                                cardStates.Add(CardState.Fire);
                            }
                            return;
                        case CardState.Wind:
                            if (cardStates.Contains(CardState.Water))
                            {
                                cardStates.Remove(CardState.Water);
                                await GameSystem.UiSystem.ShowTips(this, "扩散", new Color(0, 1, 0));
                            }
                            else if (cardStates.Contains(CardState.Fire))
                            {
                                cardStates.Remove(CardState.Fire);
                                await GameSystem.UiSystem.ShowTips(this, "扩散", new Color(0, 1, 0));
                            }
                            else if (cardStates.Contains(CardState.Wind))
                            {
                                cardStates.Remove(CardState.Wind);
                                await GameSystem.UiSystem.ShowTips(this, "烈风", new Color(0, 1, 0));

                            }
                            else if (cardStates.Contains(CardState.Soil))
                            {
                                cardStates.Remove(CardState.Soil);
                                await GameSystem.UiSystem.ShowTips(this, "扩散", new Color(1, 1, 0));
                            }
                            else
                            {
                                cardStates.Add(CardState.Wind);
                            }
                            return;
                        case CardState.Soil:
                            if (cardStates.Contains(CardState.Water))
                            {
                                cardStates.Remove(CardState.Water);
                                await GameSystem.UiSystem.ShowTips(this, "泥泞", new Color(0, 1, 0));
                            }
                            else if (cardStates.Contains(CardState.Fire))
                            {
                                cardStates.Remove(CardState.Fire);
                                await GameSystem.UiSystem.ShowTips(this, "熔岩", new Color(0, 1, 0));
                            }
                            else if (cardStates.Contains(CardState.Wind))
                            {
                                cardStates.Remove(CardState.Wind);
                                await GameSystem.UiSystem.ShowTips(this, "扩散", new Color(0, 1, 0));

                            }
                            else if (cardStates.Contains(CardState.Soil))
                            {
                                cardStates.Remove(CardState.Soil);
                                await GameSystem.UiSystem.ShowTips(this, "固化", new Color(1, 1, 0));
                            }
                            else
                            {
                                cardStates.Add(CardState.Soil);
                            }
                            return;
                        case CardState.Hold:
                            break;
                        case CardState.Congealbounds:
                            break;
                        case CardState.Forbidden:
                            break;
                        case CardState.Black:
                            if (cardStates.Contains(CardState.White))
                            {
                                await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Battle][CardRank.NoGold].CardList, 1, true);
                                await GameSystem.PointSystem.Hurt(new Event(this, GameSystem.InfoSystem.SelectUnits).SetPoint(1));
                                cardStates.Remove(CardState.White);
                            }
                            break;
                        case CardState.White:
                            if (cardStates.Contains(CardState.Black))
                            {
                                await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.NoGold].CardList, 1, true);
                                await GameSystem.PointSystem.Gain(new Event(this, GameSystem.InfoSystem.SelectUnits).SetPoint(1));
                                cardStates.Remove(CardState.Black);
                            }
                            break;
                        case CardState.Apothanasia:
                            await GameSystem.UiSystem.ShowTips(this, "续命", new Color(1, 0, 0));
                            break;
                            break;
                        default: break;
                    }
                    this[e.targetState] = true;
                })
               .AbilityAppend();
            //卡牌状态取消时效果
            AbalityRegister(TriggerTime.When, TriggerType.StateClear)
                        .AbilityAdd(async (e) =>
                        {
                            await GameSystem.UiSystem.ShowIconBreak(this, e.targetState);
                            this[e.targetState] = false;
                            //动画效果
                            switch (e.targetState)
                            {
                                case CardState.Lurk:; break;
                                case CardState.Seal: await Command.CardCommand.UnSealCard(this); break;
                                default: break;
                            }
                        })
                       .AbilityAppend();

            ///////////////////////////////////////////////////////附加字段///////////////////////////////////////////////////////////////////////
            //卡牌字段设置时效果
            AbalityRegister(TriggerTime.When, TriggerType.FieldSet)
                        .AbilityAdd(async (e) =>
                        {
                            if (e.point > this[e.targetFiled])
                            {
                                await GameSystem.UiSystem.ShowIcon(this, e.targetFiled);
                            }
                            else if (e.point < this[e.targetFiled])
                            {
                                await GameSystem.UiSystem.ShowIconBreak(this, e.targetFiled);
                            }
                            else
                            {
                                return;
                            }
                            Debug.Log($"触发类型：{e.targetFiled}当字段设置，对象卡牌{CardID}原始值{this[e.targetFiled]},设置值{e.point}");
                            this[e.targetFiled] = e.point;
                            Debug.Log($"触发结果：{this[e.targetFiled]}");
                            //移除掉为0的字段
                            if (this[e.targetFiled] > 0)
                            {
                                switch (e.targetFiled)
                                {
                                    case CardField.Timer: break;
                                    case CardField.Inspire: break;

                                    default: break;
                                }
                            }
                            else
                            {
                                cardFields.Remove(e.targetFiled);
                            }
                        })
                       .AbilityAppend();
            //卡牌字段改变时效果
            AbalityRegister(TriggerTime.When, TriggerType.FieldChange)
                        .AbilityAdd(async (e) =>
                        {
                            if (e.point > 0)
                            {
                                await GameSystem.UiSystem.ShowIcon(this, e.targetFiled);
                            }
                            else if (e.point < 0)
                            {
                                await GameSystem.UiSystem.ShowIconBreak(this, e.targetFiled);
                            }
                            else
                            {
                                return;
                            }

                            Debug.Log($"触发类型：{e.targetFiled}当字段变化，对象卡牌{CardID}原始值{this[e.targetFiled]},变化值{e.point}");
                            this[e.targetFiled] += e.point;
                            Debug.Log($"触发结果：{this[e.targetFiled]}");
                            switch (e.targetFiled)
                            {
                                case CardField.Timer: break;
                                case CardField.Inspire: break;
                                default: break;
                            }
                        })
                       .AbilityAppend();
            ///////////////////////////////////////////////////////对战流程///////////////////////////////////////////////////////////////////////

            //结算卡牌的回合开始时触发的自动类型效果
            AbalityRegister(TriggerTime.After, TriggerType.TurnEnd)
                .AbilityAdd(async (e) =>
                {
                    //我死啦
                    if (IsCardReadyToGrave)
                    {
                        //延命
                        if (this[CardState.Apothanasia])
                        {
                            await GameSystem.StateSystem.ClearState(new Event(this, GameSystem.InfoSystem.SelectUnits).SetTargetState(CardState.Apothanasia));
                            await GameSystem.PointSystem.Gain(new Event(this, this).SetPoint(1));
                        }
                    }
                    //将判定为死掉卡牌移入墓地，触发遗愿联锁效果
                    //我死啦
                    if (IsCardReadyToGrave)
                    {
                        //摧毁自身同时触发咒术
                        Debug.LogError(CardID + BasePoint + "=" + ChangePoint);
                        await GameSystem.TransferSystem.DeadCard(e);
                    }
                })
               .AbilityAppend();
            //小局结束时，移入所有卡牌至墓地
            AbalityRegister(TriggerTime.After, TriggerType.RoundEnd)
                .AbilityAdd(async (e) =>
                {
                    if (AgainstInfo.cardSet[GameRegion.Battle].CardList.Contains(this))
                    {
                        await Command.CardCommand.MoveToGrave(this);
                    }
                })
               .AbilityAppend();
        }
        public void SetCardTransform(Vector3 TargetPosition, Vector3 TargetEulers)
        {
            targetPosition = TargetPosition;
            targetQuaternion = Quaternion.Euler(TargetEulers + new Vector3(0, 0, IsCanSee ? 0 : 180));
            if (IsInit)
            {
                transform.position = targetPosition;
                transform.rotation = targetQuaternion;
                IsInit = false;
            }
        }
        public void RefreshState()
        {
            Material material = GetComponent<Renderer>().material;
            if (AgainstInfo.playerFocusCard == this)
            {
                material.SetFloat("_IsFocus", 1);
                material.SetFloat("_IsRed", 0);
            }
            else if (AgainstInfo.opponentFocusCard == this)
            {
                material.SetFloat("_IsFocus", 1);
                material.SetFloat("_IsRed", 1);
            }
            else
            {
                material.SetFloat("_IsFocus", 0);
            }
            material.SetFloat("_IsTemp", IsGray ? 0 : 1);
            transform.position = Vector3.Lerp(transform.position, targetPosition, MoveSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, Time.deltaTime * 10);
            PointText.text = CardType == CardType.Unit ? ShowPoint.ToString() : "";
        }
        /// <summary>
        /// 注册一个能力
        /// </summary>
        /// <param name="time">触发时机</param>
        /// <param name="type">触发方式</param>
        /// <returns>一个触发效果的配置管理器</returns>
        public Manager.CardAbilityManeger AbalityRegister(TriggerTime time, TriggerType type) => new Manager.CardAbilityManeger(this, time, type);
        private void Update()
        {
            RefreshCardUi();

            void RefreshCardUi()
            {
                //数字
                if (ChangePoint > 0)
                {
                    PointText.color = Color.green;
                }
                else if (ChangePoint < 0)
                {
                    PointText.color = Color.red;
                }
                else
                {
                    PointText.color = Color.black;
                }
                PointText.text = CardType == CardType.Unit ? ShowPoint.ToString() : "";
                //字段
                for (int i = 0; i < 4; i++)
                {
                    if (cardFields.Count > 4 && i == 3)
                    {
                        //icon是省略号
                    }
                    else if (i < cardFields.Count)
                    {
                        FieldIconContent.GetChild(i).GetComponent<Image>().sprite = Command.UiCommand.GetFieldAndStateSprite(cardFields.ToList()[i].Key);
                        FieldIconContent.GetChild(i).GetChild(0).GetComponent<Text>().text = cardFields.ToList()[i].Value.ToString();
                        FieldIconContent.GetChild(i).gameObject.SetActive(true);
                    }
                    else
                    {
                        FieldIconContent.GetChild(i).gameObject.SetActive(false);
                    }
                }
                //状态
                for (int i = 0; i < 3; i++)
                {
                    if (cardStates.Count > 3 && i == 2)
                    {
                        //icon是省略号
                    }
                    else if (i < cardStates.Count)
                    {
                        StateIconContent.GetChild(i).GetComponent<Image>().sprite = Command.UiCommand.GetFieldAndStateSprite(cardStates[i]);
                        StateIconContent.GetChild(i).gameObject.SetActive(true);
                    }
                    else
                    {
                        StateIconContent.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
        }


        [Button]
        public void AddStateAndField()
        {
            for (int i = 1; i < 8; i++)
            {
                cardFields.Add((CardField)i, i);
            }
            for (int i = 1; i < 18; i++)
            {
                cardStates.Add((CardState)i);
            }
        }

        [Button] public void 水() => _ = GameSystem.StateSystem.SetState(new Event(this, this).SetTargetState(CardState.Water));
        [Button] public void 火() => _ = GameSystem.StateSystem.SetState(new Event(this, this).SetTargetState(CardState.Fire));
        [Button] public void 风() => _ = GameSystem.StateSystem.SetState(new Event(this, this).SetTargetState(CardState.Wind));
        [Button] public void 土() => _ = GameSystem.StateSystem.SetState(new Event(this, this).SetTargetState(CardState.Soil));
    }
}