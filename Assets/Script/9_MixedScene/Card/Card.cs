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
        public Orientation orientation => AgainstInfo.cardSet[Orientation.Down].CardList.Contains(this) ? Orientation.Down : Orientation.Up;
        //获取全局牌表区域
        public GameRegion CurrentRegion => AgainstInfo.cardSet.SingleRowInfos.First(row => row.CardList.Contains(this)).region;
        public string cardTag { get; set; }
        public CardRank cardRank { get; set; }
        public CardType cardType { get; set; }


        [ShowInInspector]
        public Dictionary<CardField, int> cardFields = new Dictionary<CardField, int>();
        public int this[CardField cardField]
        {
            get => cardFields.ContainsKey(cardField) ? cardFields[cardField] : 0;
            set => cardFields[cardField] = value;
        }
        [ShowInInspector]
        public Dictionary<CardState, bool> cardStates = new Dictionary<CardState, bool>();
        public bool this[CardState cardState]
        {
            get => cardStates.ContainsKey(cardState) ? cardStates[cardState] : false;
            set => cardStates[cardState] = value;
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
            .AbilityAdd(async (triggerInfo) => { await Command.CardCommand.Set(triggerInfo); })
            .AbilityAppend();
            //当获得增益时获得点数增加
            AbalityRegister(TriggerTime.When, TriggerType.Gain)
            .AbilityAdd(async (triggerInfo) => { await Command.CardCommand.Gain(triggerInfo); })
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
                    if (cardFields[CardField.Apothanasia] > 0)
                    {
                        cardFields[CardField.Apothanasia]--;
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
                this[triggerInfo.targetState] = true;
                switch (triggerInfo.targetState)
                {
                    case CardState.Lurk:; break;
                    case CardState.Seal: await Command.CardCommand.SealCard(this); break;
                    default: break;
                }
            })
           .AbilityAppend();
            //卡牌状态取消时效果
            AbalityRegister(TriggerTime.When, TriggerType.StateClear)
            .AbilityAdd(async (triggerInfo) =>
            {
                this[triggerInfo.targetState] = false;
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
                    case CardField.Vitality: break;
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
                //Command.EffectCommand.Bullet_Gain(triggerInfo);
                //await Command.AudioCommand.PlayAsync(GameAudioType.Biu);
                if (this[triggerInfo.targetFiled]<=0)
                {
                    cardFields.Remove(triggerInfo.targetFiled);
                }
                switch (triggerInfo.targetFiled)
                {
                    case CardField.Timer: break;
                    case CardField.Vitality: break;
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
            PointText.text = cardType == CardType.Unite ? ShowPoint.ToString() : "";
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
            PointText.text = cardType == CardType.Unite ? ShowPoint.ToString() : "";
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
                    StateIconContent.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("FieldAndState\\" + cardFields.ToList()[i].Key.ToString()); ;
                    StateIconContent.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    StateIconContent.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}
