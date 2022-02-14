using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace TouhouMachineLearningSummary.Info
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
        public Transform ContentInstance;
        public GameObject CardBoardInstance;
        public GameObject CardIntroductionModel;

        public GameObject Notice_Model;

        private void Awake() => Instance = this;
        [ShowInInspector]
        public static bool isNoticeBoardShow = false;

        public static List<GameObject> ShowCardLIstOnBoard = new List<GameObject>();

        public static GameObject loginCanvas => Instance.loginCanvas_Model;

        public static GameObject Arrow => Instance.Arrow_Model;
        public static GameObject ArrowEndPoint => Instance.ArrowEndPoint_Model;
        public static Transform Content => Instance.ContentInstance;
        public static GameObject CardModel => Instance.CardInstanceModel;
        public static GameObject CardBoard => Instance.CardBoardInstance;
        public static GameObject NoticeBoard => Instance.NoticeBoard_Model;
        public static GameObject Notice => Instance.Notice_Model;
    }
}