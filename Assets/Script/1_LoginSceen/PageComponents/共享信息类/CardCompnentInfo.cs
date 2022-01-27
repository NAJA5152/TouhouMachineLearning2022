using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;

namespace TouhouMachineLearningSummary.Info
{
    /// <summary>
    /// 与卡牌ui组件相关的列表
    /// </summary>
    public class CardCompnentInfo : MonoBehaviour
    {
        //全卡牌解锁的管理员账户
        public static bool IsAdmin = true;
        public static bool cardListCanChange = false;

        [Header("卡组组件")]
        public GameObject _cardDeckNameModel;
        public GameObject _cardDeckContent;
        public GameObject _cardDeckCardModel;

        public GameObject _changeButton;
        public GameObject _okButton;
        public GameObject _cancelButton;

        public static GameObject cardDeckNameModel;
        public static GameObject cardDeckContent;
        public static GameObject cardDeckCardModel;

        public static GameObject changeButton;
        public static GameObject okButton;
        public static GameObject cancelButton;

        public static List<GameObject> deckCardModels = new List<GameObject>();

        public static Model.CardDeck tempDeck;
        List<GameObject> ShowCardList;
        //获得指定卡组的去重并按品质排序后的列表
        public static List<int> distinctCardIds => tempDeck.CardIds
            .Distinct()
            .OrderBy(id => Manager.CardAssemblyManager.GetLastCardInfo(id).cardRank)
            .ThenByDescending(id => Manager.CardAssemblyManager.GetLastCardInfo(id).point)
            .ToList();


        /// ////////////////////////////////////////////////////////牌库信息/////////////////////////////
        [Header("牌库组件")]
        public GameObject _cardLibraryContent;
        public GameObject _cardLibraryCardModel;

        public static GameObject cardLibraryContent;
        public static GameObject cardLibraryCardModel;

        public static List<GameObject> libraryCardModels = new List<GameObject>();
        public static bool isEditDeckMode = true;
        [Header("卡牌能力详情组件")]
        public GameObject _targetCardTexture;
        public GameObject _targetCardName;
        public GameObject _targetCardTag;
        public GameObject _targetCardAbility;

        public static GameObject targetCardTexture;
        public static GameObject targetCardName;
        public static GameObject targetCardTag;
        public static GameObject targetCardAbility;
        public static bool isCampIntroduction=false;
        public static Camp focusCamp =  Camp.Neutral;
        /// ////////////////////////////////////////////////////////阵营选择信息信息/////////////////////////////

        [Header("阵营组件")]
        public static List<GameObject> campCardModels = new List<GameObject>();
        public static Camp targetCamp= Camp.Neutral;
        public  GameObject campContent;
        public  GameObject CampModel;
        public Texture2D TaoismTex;
        public Texture2D ShintoismTex;
        public Texture2D BuddhismTex;
        public Texture2D scienceTex;

        /// ////////////////////////////////////////////////////////牌组信息/////////////////////////////
        [Header("牌组信息组件")]
        [ShowInInspector]
        public static int seleceDeckRank = 0;
        public GameObject deckModel;
        public List<GameObject> deckModels;
        public static float fre = -310f;
        public static float bias = 0.13f;
        public static float show = -0.66f;
        public static bool isDragMode;//是否处于手动拖拽模式
        public static bool isCardClick;
        [ShowInInspector]
        public static List<float> values = new List<float>();
        public Transform content;
        //单例
        public static CardCompnentInfo instance;
        private void Awake()
        {
            instance = this;
            //牌组组件
            cardDeckNameModel = _cardDeckNameModel;
            cardDeckContent = _cardDeckContent;
            cardDeckCardModel = _cardDeckCardModel;

            //牌库组件
            cardLibraryContent = _cardLibraryContent;
            cardLibraryCardModel = _cardLibraryCardModel;
            cardLibraryCardModel.SetActive(false);
            //卡牌能力详情组件
            targetCardTexture = _targetCardTexture;
            targetCardName = _targetCardName;
            targetCardTag = _targetCardTag;
            targetCardAbility = _targetCardAbility;
            changeButton = _changeButton;
            okButton = _okButton;
            cancelButton = _cancelButton;
        }
    }
}