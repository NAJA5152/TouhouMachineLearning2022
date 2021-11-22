using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    public class BookInfo : MonoBehaviour
    {
        //组件化
        [Header("书本模型")]
        [Sirenix.OdinInspector.ShowInInspector]
        public static bool isBookOpen;
        public static float angle = 0;

        [Header("书本模型")]

        public GameObject _coverModel;
        public GameObject _axisModel;

        public static GameObject coverModel;
        public static GameObject axisModel;

        [Header("UI组件")]
        public GameObject _singleModeSelectComponent;
        public GameObject _multiplayerModeSelectComponent;
        public GameObject _practiceComponent;
        public GameObject _cardListComponent;
        public GameObject _cardDeckListComponent;
        public GameObject _cardLibraryComponent;
        public GameObject _mapComponent;
        public GameObject _cardDetailComponent;
        public  GameObject _campSelectComponent;


        public static GameObject singleModeSelectComponent;
        public static GameObject multiplayerModeSelectComponent;
        public static GameObject practiceComponent;
        public static GameObject cardListComponent;
        public static GameObject cardDeckListComponent;
        public static GameObject cardLibraryComponent;
        public static GameObject mapComponent;
        public static GameObject cardDetailComponent;
        public static GameObject campSelectComponent;

        // Start is called before the first frame update
        void Awake()
        {
            coverModel = _coverModel;
            axisModel = _axisModel;

            singleModeSelectComponent = _singleModeSelectComponent;
            multiplayerModeSelectComponent = _multiplayerModeSelectComponent;
            practiceComponent = _practiceComponent;
            cardListComponent = _cardListComponent;
            cardDeckListComponent = _cardDeckListComponent;
            cardLibraryComponent = _cardLibraryComponent;
            mapComponent = _mapComponent;
            cardDetailComponent = _cardDetailComponent;
            campSelectComponent = _campSelectComponent;
        }
    }
}

