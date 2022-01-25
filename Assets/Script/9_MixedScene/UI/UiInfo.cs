using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace TouhouMachineLearningSummary.Info.GameUI
{
    public class UiInfo : MonoBehaviour
    {
        public static UiInfo Instance { get; set; }
        public GameObject DownPass;
        public GameObject UpPass;
        public static GameObject MyPass => AgainstInfo.IsMyTurn ? Instance.DownPass : Instance.UpPass;
        public static GameObject OpPass => AgainstInfo.IsMyTurn ? Instance.UpPass : Instance.DownPass;

        [LabelText("登录界面")]
        public GameObject loginCanvas_Model;

        public Animator NoticeAnim;
        public GameObject Arrow_Model;
        public GameObject CardInstanceModel;
        public GameObject NoticeBoard_Model;
        public GameObject ArrowEndPoint_Model;
        public Transform ConstantInstance;
        public GameObject CardBoardInstance;
        public GameObject CardIntroductionModel;

        public GameObject Notice_Model;

        //卡牌面板进度
        [ShowInInspector]
        //public  float cardBoardProcess => ConstantInstance.GetComponent<RectTransform>().rect.x;
        //public  RectOffset cardBoardProcess => ConstantInstance.GetComponent<HorizontalLayoutGroup>();
        //public static float targetCardBoardProcess=> cardBoardProcess;
        private void Awake() => Instance = this;
        private void Update()
        {
            if (true)
            {
                //ConstantInstance.GetComponent<RectTransform>().rect.x=>
            }
        }
        //public static string CardBoardTitle = "";
        [ShowInInspector]
        public static bool isNoticeBoardShow = false;

        public static List<GameObject> ShowCardLIstOnBoard = new List<GameObject>();

        public static GameObject loginCanvas => Instance.loginCanvas_Model;

        public static GameObject Arrow => Instance.Arrow_Model;
        public static GameObject ArrowEndPoint => Instance.ArrowEndPoint_Model;
        public static Transform Constant => Instance.ConstantInstance;
        public static GameObject CardModel => Instance.CardInstanceModel;
        public static GameObject CardBoard => Instance.CardBoardInstance;
        public static GameObject NoticeBoard => Instance.NoticeBoard_Model;
        public static GameObject Notice => Instance.Notice_Model;
    }
}