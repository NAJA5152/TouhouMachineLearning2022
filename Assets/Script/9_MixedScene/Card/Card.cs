using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Model
{


    public class Card : MonoBehaviour
    {
        public CardManager ThisCardManager => GetComponent<CardManager>();
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
        [ShowInInspector]
        //卡牌默认可部署所属
        public Orientation CurrentOrientation => AgainstInfo.cardSet[Orientation.Down].CardList.Contains(this) ? Orientation.Down : Orientation.Up;
        public Orientation OppositeOrientation => CurrentOrientation== Orientation.Down ? Orientation.Up : Orientation.Down ;
        //获取全局牌表区域
        public GameRegion CurrentRegion => AgainstInfo.cardSet.RowManagers.First(row => row.CardList.Contains(this)).region;
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
        public string CardTag { get; set; }
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
                    cardStates.Remove(cardState);
                }
            }
        }
        public Territory belong => AgainstInfo.cardSet[Orientation.Down].CardList.Contains(this) ? Territory.My : Territory.Op;
        public Vector3 targetPosition;
        public Quaternion targetQuaternion;

        //状态相关参数
        public bool IsInit { get; set; } = false;
        public bool IsGray { get; set; } = false;
        public bool IsCover { get; set; } = false;
        public bool IsLocked { get; set; } = false;
        public bool IsFree { get; set; } = false;
        public bool IsCanSee { get; set; } = false;
        public bool IsCardReadyToGrave => ShowPoint == 0 && AgainstInfo.cardSet[GameRegion.Battle].CardList.Contains(this);
        public void SetCardSeeAble(bool isCanSee) => this.IsCanSee = isCanSee;
        public bool isMoveStepOver = true;
        public bool isPrepareToPlay = false;
        public bool IsAutoMove => this != AgainstInfo.playerPrePlayCard;

        public List<Card> belongCardList => Command.RowCommand.GetCardList(this);
        public Location Location => Command.RowCommand.GetLocation(this);
        public Card LeftCard => Location.Y > 0 ? belongCardList[Location.Y - 1] : null;
        public Card RightCard => Location.Y < belongCardList.Count - 1 ? belongCardList[Location.Y + 1] : null;
        public Text PointText => transform.GetChild(0).GetChild(0).GetComponent<Text>();
        public Transform FieldIconContent => transform.GetChild(0).GetChild(1);
        public Transform StateIconContent => transform.GetChild(0).GetChild(2);
        public string CardName => Manager.CardAssemblyManager.GetCurrentCardInfos(CardID).translateName;

        [ShowInInspector]
        public string CardIntroduction
        {
            get
            {
                string describe = Manager.CardAssemblyManager.GetCurrentCardInfos(CardID).translateAbility;
                typeof(CardField).GetEnumNames().ToList().ForEach(name =>
                {
                    describe = describe.Replace($"${name}", this[(CardField)Enum.Parse(typeof(CardField), name)].ToString());
                });
                return describe;
            }
        }

        public Dictionary<TriggerTime, Dictionary<TriggerType, List<Func<TriggerInfoModel, Task>>>> cardAbility = new Dictionary<TriggerTime, Dictionary<TriggerType, List<Func<TriggerInfoModel, Task>>>>();


        /// <summary>
        /// 注册默认共通的卡牌效果
        /// </summary>
        public virtual void Init()
        {
            IsInit = true;
            //初始化卡牌效果并填充空效果
            foreach (TriggerTime tirggerTime in Enum.GetValues(typeof(TriggerTime)))
            {
                cardAbility[tirggerTime] = new Dictionary<TriggerType, List<Func<TriggerInfoModel, Task>>>();
                foreach (TriggerType triggerType in Enum.GetValues(typeof(TriggerType)))
                {
                    cardAbility[tirggerTime][triggerType] = new List<Func<TriggerInfoModel, Task>>();
                }
            }
            //当创造时从牌库中构建
            AbalityRegister(TriggerTime.When, TriggerType.Generate)
             .AbilityAdd(async (triggerInfo) => { await Command.CardCommand.GenerateCard(triggerInfo.targetCard, triggerInfo.location); })
             .AbilityAppend();
            //当弃牌时移动至墓地
            AbalityRegister(TriggerTime.When, TriggerType.Discard)
              .AbilityAdd(async (triggerInfo) => { await Command.CardCommand.DisCard(triggerInfo.targetCard); })
              .AbilityAppend();
            //当死亡时移至墓地
            AbalityRegister(TriggerTime.When, TriggerType.Dead)
            .AbilityAdd(async (triggerInfo) => { await Command.CardCommand.MoveToGrave(this); })
            .AbilityAppend();
            //当复活时移至出牌区
            AbalityRegister(TriggerTime.When, TriggerType.Revive)
            .AbilityAdd(async (triggerInfo) => { await Command.CardCommand.ReviveCard(triggerInfo); })
            .AbilityAppend();
            //当移动时移到指定位置
            AbalityRegister(TriggerTime.When, TriggerType.Move)
            .AbilityAdd(async (triggerInfo) => { await Command.CardCommand.MoveCard(triggerInfo.targetCard, triggerInfo.location); })
            .AbilityAppend();
            //当间隙时从游戏中除外
            AbalityRegister(TriggerTime.When, TriggerType.Banish)
            .AbilityAdd(async (triggerInfo) => { await Command.CardCommand.BanishCard(this); })
            .AbilityAppend();
            //当召唤时从卡组中拉出
            AbalityRegister(TriggerTime.When, TriggerType.Summon)
            .AbilityAdd(async (triggerInfo) => { await Command.CardCommand.SummonCard(this); })
            .AbilityAppend();
            //当设置点数时，修改变更点数值
            AbalityRegister(TriggerTime.When, TriggerType.Set)
            .AbilityAdd(async (triggerInfo) =>
            {
                var targetShowPoint = triggerInfo.targetCard.ShowPoint;
                await Command.CardCommand.Set(triggerInfo);
                //如果原本点数大于设置点数
                if (targetShowPoint < triggerInfo.point)
                {
                    await GameSystem.PointSystem.Increase(triggerInfo);
                }
                if (targetShowPoint > triggerInfo.point)
                {
                    await GameSystem.PointSystem.Decrease(triggerInfo);
                }
            })
            .AbilityAppend();
            //当获得增益时获得点数增加
            AbalityRegister(TriggerTime.When, TriggerType.Gain)
            .AbilityAdd(async (triggerInfo) =>
            {
                await Command.CardCommand.Gain(triggerInfo);
                if (triggerInfo.point > 0)//如果不处于温顺状态
                {
                    await GameSystem.PointSystem.Increase(triggerInfo);
                }
            })
            .AbilityAppend();
            //默认受伤效果：当卡牌受到伤害时则会受到伤害，当卡牌死亡时，触发卡牌死亡机制
            AbalityRegister(TriggerTime.When, TriggerType.Hurt)
            .AbilityAdd(async (triggerInfo) =>
            {
                if (this[CardState.Congealbounds])
                {
                    await GameSystem.StateSystem.ClearState(new TriggerInfoModel(triggerInfo.triggerCard, this));
                }
                else
                {
                    if (this[CardField.Shield] > 0)
                    {
                        //计算剩余盾量
                        var shieldPoint = this[CardField.Shield] - triggerInfo.point;
                        //计算剩余盾量
                        triggerInfo.point = triggerInfo.point - this[CardField.Shield];
                    }
                    await Command.CardCommand.Hurt(triggerInfo);
                    if (triggerInfo.point > 0)
                    {
                        await GameSystem.PointSystem.Decrease(triggerInfo);
                    }
                }
            })
            .AbilityAppend();
            //当治愈时
            AbalityRegister(TriggerTime.When, TriggerType.Cure)
            .AbilityAdd(async (triggerInfo) =>
            {
                triggerInfo.point = -Math.Min(0, triggerInfo.targetCard.ChangePoint);
                await Command.CardCommand.Gain(triggerInfo);
            })
           .AbilityAppend();
            //当被摧毁时以自身点数对自己造成伤害
            AbalityRegister(TriggerTime.When, TriggerType.Destory)
            .AbilityAdd(async (triggerInfo) =>
            {
                triggerInfo.point = ShowPoint;
                await Command.CardCommand.Hurt(triggerInfo);
            })
           .AbilityAppend();
            //当点数逆转时触发
            AbalityRegister(TriggerTime.When, TriggerType.Reverse)
            .AbilityAdd(async (triggerInfo) => { await Command.CardCommand.Reversal(triggerInfo); })
           .AbilityAppend();
            //登记卡牌回合状态变化时效果
            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
            .AbilityAdd(async (triggerInfo) =>
            {


                //我死啦
                if (IsCardReadyToGrave)
                {
                    //延命
                    if (this[CardField.Apothanasia] > 0)
                    {
                        this[CardField.Apothanasia]--;
                        await GameSystem.FieldSystem.ChangeField(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits).SetTargetField(CardField.Apothanasia, -1));
                    }
                    else
                    {
                        //摧毁自身同时触发咒术
                        await GameSystem.TransSystem.DeadCard(triggerInfo);
                    }
                }

            })
           .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.RoundEnd)
            .AbilityAdd(async (triggerInfo) =>
            {
                if (AgainstInfo.cardSet[GameRegion.Battle].CardList.Contains(this))
                {
                    await Command.CardCommand.MoveToGrave(this);
                }
            })
           .AbilityAppend();
            //卡牌状态附加时效果
            AbalityRegister(TriggerTime.When, TriggerType.StateAdd)
            .AbilityAdd(async (triggerInfo) =>
            {
                switch (triggerInfo.targetState)
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
                    case CardState.Secret:
                        break;
                    case CardState.Furor:
                        await ThisCardManager.ShowTips("狂躁", new Color(0.6f, 0.2f, 0));
                        break;
                    case CardState.Docile:
                        await ThisCardManager.ShowTips("温顺", new Color(0, 0, 0.5f));
                        break;
                    case CardState.Poisoning:
                        break;
                    case CardState.Rely:
                        break;
                    case CardState.Water:
                        if (cardStates.Contains(CardState.Water))
                        {
                            cardStates.Remove(CardState.Water);
                            await ThisCardManager.ShowTips("治愈", new Color(0, 1, 0));
                        }
                        else if (cardStates.Contains(CardState.Fire))
                        {
                            cardStates.Remove(CardState.Fire);
                            await ThisCardManager.ShowTips("中和", new Color(0, 1, 0));
                        }
                        else if (cardStates.Contains(CardState.Wind))
                        {
                            cardStates.Remove(CardState.Wind);
                            await ThisCardManager.ShowTips("扩散", new Color(0, 1, 0));

                        }
                        else if (cardStates.Contains(CardState.Soil))
                        {
                            cardStates.Remove(CardState.Soil);
                            await ThisCardManager.ShowTips("泥泞", new Color(1, 1, 0));
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
                            await ThisCardManager.ShowTips("中和", new Color(0, 1, 0));
                        }
                        else if (cardStates.Contains(CardState.Fire))
                        {
                            cardStates.Remove(CardState.Fire);
                            await ThisCardManager.ShowTips("烧灼", new Color(0, 1, 0));
                        }
                        else if (cardStates.Contains(CardState.Wind))
                        {
                            cardStates.Remove(CardState.Wind);
                            await ThisCardManager.ShowTips("扩散", new Color(0, 1, 0));

                        }
                        else if (cardStates.Contains(CardState.Soil))
                        {
                            cardStates.Remove(CardState.Soil);
                            await ThisCardManager.ShowTips("熔岩", new Color(1, 1, 0));
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
                            await ThisCardManager.ShowTips("扩散", new Color(0, 1, 0));
                        }
                        else if (cardStates.Contains(CardState.Fire))
                        {
                            cardStates.Remove(CardState.Fire);
                            await ThisCardManager.ShowTips("扩散", new Color(0, 1, 0));
                        }
                        else if (cardStates.Contains(CardState.Wind))
                        {
                            cardStates.Remove(CardState.Wind);
                            await ThisCardManager.ShowTips("烈风", new Color(0, 1, 0));

                        }
                        else if (cardStates.Contains(CardState.Soil))
                        {
                            cardStates.Remove(CardState.Soil);
                            await ThisCardManager.ShowTips("扩散", new Color(1, 1, 0));
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
                            await ThisCardManager.ShowTips("泥泞", new Color(0, 1, 0));
                        }
                        else if (cardStates.Contains(CardState.Fire))
                        {
                            cardStates.Remove(CardState.Fire);
                            await ThisCardManager.ShowTips("熔岩", new Color(0, 1, 0));
                        }
                        else if (cardStates.Contains(CardState.Wind))
                        {
                            cardStates.Remove(CardState.Wind);
                            await ThisCardManager.ShowTips("扩散", new Color(0, 1, 0));

                        }
                        else if (cardStates.Contains(CardState.Soil))
                        {
                            cardStates.Remove(CardState.Soil);
                            await ThisCardManager.ShowTips("固化", new Color(1, 1, 0));
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
                    default: break;
                }
                this[triggerInfo.targetState] = true;
            })
           .AbilityAppend();
            //卡牌状态取消时效果
            AbalityRegister(TriggerTime.When, TriggerType.StateClear)
            .AbilityAdd(async (triggerInfo) =>
            {
                this[triggerInfo.targetState] = false;
                //动画效果
                switch (triggerInfo.targetState)
                {
                    case CardState.Lurk:; break;
                    case CardState.Seal: await Command.CardCommand.UnSealCard(this); break;
                    default: break;
                }
            })
           .AbilityAppend();
            //卡牌字段设置时效果
            AbalityRegister(TriggerTime.When, TriggerType.FieldSet)
            .AbilityAdd(async (triggerInfo) =>
            {
                Debug.Log($"触发类型：{triggerInfo.targetFiled}当字段设置，对象卡牌{this.CardID}原始值{this[triggerInfo.targetFiled]},设置值{triggerInfo.point}");
                this[triggerInfo.targetFiled] = triggerInfo.point;
                Debug.Log($"触发结果：{this[triggerInfo.targetFiled]}");
                switch (triggerInfo.targetFiled)
                {
                    case CardField.Timer: break;
                    case CardField.Inspire: break;
                    case CardField.Apothanasia:
                        {
                            await ThisCardManager.ShowTips("续命", new Color(1, 0, 0));
                            break;
                        }
                    default: break;
                }
            })
           .AbilityAppend();
            //卡牌字段改变时效果
            AbalityRegister(TriggerTime.When, TriggerType.FieldChange)
            .AbilityAdd(async (triggerInfo) =>
            {
                Debug.Log($"触发类型：{triggerInfo.targetFiled}当字段变化，对象卡牌{this.CardID}原始值{this[triggerInfo.targetFiled]},变化值{triggerInfo.point}");
                this[triggerInfo.targetFiled] += triggerInfo.point;
                Debug.Log($"触发结果：{this[triggerInfo.targetFiled]}");
                switch (triggerInfo.targetFiled)
                {
                    case CardField.Timer: break;
                    case CardField.Inspire: break;
                    case CardField.Apothanasia: await ThisCardManager.ShowTips("续命", new Color(1, 0, 0)); break;
                    default: break;
                }
            })
           .AbilityAppend();
        }
        public void SetMoveTarget(Vector3 TargetPosition, Vector3 TargetEulers)
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
            PointText.text = CardType == CardType.Unite ? ShowPoint.ToString() : "";
        }
        public CardAbilityManeger AbalityRegister(TriggerTime time, TriggerType type) => new CardAbilityManeger(this, time, type);
        private void Update() => RefreshCardUi();

        public void RefreshCardUi()
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
            PointText.text = CardType == CardType.Unite ? ShowPoint.ToString() : "";
            //字段
            for (int i = 0; i < 4; i++)
            {
                if (cardFields.Count > 4 && i == 3)
                {
                    //icon是省略号
                }
                else if (i < cardFields.Count)
                {
                    FieldIconContent.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("FieldAndState\\" + cardFields.ToList()[i].Key.ToString());
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
                    StateIconContent.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("FieldAndState\\" + cardStates[i].ToString()); ;
                    StateIconContent.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    StateIconContent.GetChild(i).gameObject.SetActive(false);
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

        [Button] public void 水() => _ = GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Water));
        [Button] public void 火() => _ = GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Fire));
        [Button] public void 风() => _ = GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Wind));
        [Button] public void 土() => _ = GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Soil));
    }
}