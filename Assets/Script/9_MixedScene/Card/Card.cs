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
        public int cardID;

        public int basePoint;
        public int changePoint;
        public int showPoint => Mathf.Max(0, basePoint + changePoint);

        public Texture2D icon;
        public float moveSpeed = 0.1f;
        //卡牌默认可部署属性区域
        public BattleRegion region;
        //卡牌默认可部署所属
        public Territory territory;
        [ShowInInspector]
        public Orientation orientation => AgainstInfo.cardSet[Orientation.Down].CardList.Contains(this) ? Orientation.Down : Orientation.Up;
        //获取全局牌表区域
        public GameRegion currentRegion => AgainstInfo.cardSet.singleRowInfos.First(row => row.ThisRowCards.Contains(this)).region;
        public string cardTag;
        public CardRank cardRank;
        public CardType cardType;


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
        public bool isInit = false;
        public bool isGray = false;
        public bool isCover = false;
        public bool isLocked = false;
        public bool isFree = false;
        public bool isCanSee = false;
        public bool isCardDead => showPoint == 0 && AgainstInfo.cardSet[GameRegion.Battle].CardList.Contains(this);
        public void SetCardSeeAble(bool isCanSee) => this.isCanSee = isCanSee;
        public bool isMoveStepOver = true;
        public bool isPrepareToPlay = false;
        public bool IsAutoMove => this != AgainstInfo.playerPrePlayCard;

        public List<Card> belongCardList => RowsInfo.GetCardList(this);
        public Location location => RowsInfo.GetLocation(this);
        //[ShowInInspector]
        public Card LeftCard => location.Y > 0 ? belongCardList[location.Y - 1] : null;
        //[ShowInInspector]
        public Card RightCard => location.Y < belongCardList.Count - 1 ? belongCardList[location.Y + 1] : null;
        [ShowInInspector]
        public int twoSideVitality => (LeftCard == null || LeftCard[CardState.Seal] ? 0 : LeftCard[CardField.Vitality]) + (RightCard == null || RightCard[CardState.Seal] ? 0 : RightCard[CardField.Vitality]);

        public Text PointText => transform.GetChild(0).GetChild(0).GetComponent<Text>();
        public string CardName => Manager.CardAssemblyManager.GetCurrentCardInfos(cardID).translateName;

        [ShowInInspector]
        public string CardIntroduction
        {
            get
            {
                string describe = Manager.CardAssemblyManager.GetCurrentCardInfos(cardID).translateAbility;
                typeof(CardField).GetEnumNames().ToList().ForEach(name =>
                {
                    describe = describe.Replace($"{{{name}}}", this[(CardField)Enum.Parse(typeof(CardField), name)].ToString());
                });
                return describe;
            }
        }

        public Dictionary<TriggerTime, Dictionary<TriggerType, List<Func<TriggerInfo, Task>>>> cardAbility = new Dictionary<TriggerTime, Dictionary<TriggerType, List<Func<TriggerInfo, Task>>>>();

        private void Update() => RefreshCardUi();

        public void RefreshCardUi()
        {
            PointText.text = cardType == CardType.Unite ? showPoint.ToString() : "";
            if (changePoint > 0)
            {
                PointText.color = Color.green;
            }
            else if (changePoint < 0)
            {
                PointText.color = Color.red;
            }
            else
            {
                PointText.color = Color.black;
            }
        }
        /// <summary>
        /// 注册默认共通的卡牌效果
        /// </summary>
        public virtual void Init()
        {
            isInit = true;

            foreach (TriggerTime tirggerTime in Enum.GetValues(typeof(TriggerTime)))
            {
                cardAbility[tirggerTime] = new Dictionary<TriggerType, List<Func<TriggerInfo, Task>>>();
                foreach (TriggerType triggerType in Enum.GetValues(typeof(TriggerType)))
                {
                    cardAbility[tirggerTime][triggerType] = new List<Func<TriggerInfo, Task>>();
                }
            }
            AbalityRegister(TriggerTime.When, TriggerType.Discard)
              .AbilityAdd(async (triggerInfo) =>
              {
                  await Command.CardCommand.DisCard(triggerInfo.targetCard);
                  Debug.Log("执行丢弃操作");
              })
              .AbilityAppend();

            cardAbility[TriggerTime.When][TriggerType.Gain] = new List<Func<TriggerInfo, Task>>()
            {
                async (triggerInfo) =>
                {
                    await Command.CardCommand.Gain(triggerInfo);
                }
            };
            //默认受伤效果：当卡牌受到伤害时则会受到伤害，当卡牌死亡时，就会死
            cardAbility[TriggerTime.When][TriggerType.Hurt] = new List<Func<TriggerInfo, Task>>()
            {
                async (triggerInfo) =>
                {
                    await Command.CardCommand.Hurt(triggerInfo);
                    if (isCardDead)
                    {
                        await GameSystem.TransSystem.DeadCard(triggerInfo);
                    }
                }
            };
            cardAbility[TriggerTime.When][TriggerType.Dead] = new List<Func<TriggerInfo, Task>>()
            {
                async (triggerInfo) =>
                {
                   await Command.CardCommand.MoveToGrave(this);
                }
            };
            cardAbility[TriggerTime.When][TriggerType.Destory] = new List<Func<TriggerInfo, Task>>()
            {
                async (triggerInfo) =>
                {
                    triggerInfo.point=showPoint;
                    await Command.CardCommand.Hurt(triggerInfo);
                }
            };
            cardAbility[TriggerTime.When][TriggerType.Cure] = new List<Func<TriggerInfo, Task>>()
            {
                async (triggerInfo) =>
                {
                    triggerInfo.point=-Math.Min(0, triggerInfo.targetCard.changePoint);
                     await Command.CardCommand.Gain(triggerInfo);
                }
            };
            //cardAbility[TriggerTime.When][TriggerType.SelectUnite] = new List<Func<TriggerInfo, Task>>()
            //{
            //    async (triggerInfo) =>
            //    {
            //        await Command.StateCommand.WaitForSelecUnit(triggerInfo.triggerCard, triggerInfo.targetCards, triggerInfo.selectNum, triggerInfo.autoSelect);
            //    }
            //};
            cardAbility[TriggerTime.When][TriggerType.Move] = new List<Func<TriggerInfo, Task>>()
            {
                async (triggerInfo) =>
                {
                  await Command.CardCommand.MoveCard(triggerInfo.targetCard,triggerInfo.location);
                }
            };
            cardAbility[TriggerTime.When][TriggerType.Banish] = new List<Func<TriggerInfo, Task>>()
            {
                async (triggerInfo) =>
                {
                  await Command.CardCommand.BanishCard(this);
                }
            };
            cardAbility[TriggerTime.When][TriggerType.Summon] = new List<Func<TriggerInfo, Task>>()
            {
                async (triggerInfo) =>
                {
                    Debug.LogError("开始召唤");
                  await Command.CardCommand.SummonCard(this);
                }
            };
            cardAbility[TriggerTime.When][TriggerType.Revive] = new List<Func<TriggerInfo, Task>>()
            {
                async (triggerInfo) =>
                {
                  await Command.CardCommand.ReviveCard(triggerInfo);
                  //await Command.CardCommand.PlayCard(this);
                }
            };
            //卡牌状态变化时效果
            cardAbility[TriggerTime.When][TriggerType.Seal] = new List<Func<TriggerInfo, Task>>()
            {
                async (triggerInfo) =>
                {
                    await Command.CardCommand.SealCard(this);
                }
            };
            //登记卡牌回合状态变化时效果
            cardAbility[TriggerTime.When][TriggerType.TurnEnd] = new List<Func<TriggerInfo, Task>>()
            {
                async (triggerInfo) =>
                {
                    //我死啦
                    if (isCardDead)
                    {

                    }
                    //延命
                    if (false)
                    {
                        //摧毁自身同时触发咒术
	                }
                }
            };
            cardAbility[TriggerTime.When][TriggerType.RoundEnd] = new List<Func<TriggerInfo, Task>>()
            {
                async (triggerInfo) =>
                {
                    if (AgainstInfo.cardSet[GameRegion.Battle].CardList.Contains(this))
                    {
                        Debug.Log("移除啦");
                        await Command.CardCommand.MoveToGrave(this);
                    }
                }
            };
        }
        public void SetMoveTarget(Vector3 TargetPosition, Vector3 TargetEulers)
        {
            targetPosition = TargetPosition;
            targetQuaternion = Quaternion.Euler(TargetEulers + new Vector3(0, 0, isCanSee ? 0 : 180));
            if (isInit)
            {
                transform.position = targetPosition;
                transform.rotation = targetQuaternion;
                isInit = false;
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
            material.SetFloat("_IsTemp", isGray ? 0 : 1);
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, Time.deltaTime * 10);
            PointText.text = cardType == CardType.Unite ? showPoint.ToString() : "";
        }
        protected CardAbilityManeger AbalityRegister(TriggerTime time, TriggerType type) => new CardAbilityManeger(this, time, type);
    }
}
