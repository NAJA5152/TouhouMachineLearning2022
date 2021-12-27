using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace TouhouMachineLearningSummary.Manager
{
    public class CollectionManager : MonoBehaviour
    {
        public GameObject againstSummaryComponent;
        public GameObject againstSummaryItem;
        public GameObject achievementComponent;
        public GameObject musicComponent;
        public GameObject cgComponent;
        public static CollectionManager Manager { get; set; }
        void Awake() => Manager = this;
        public static void Init()
        {
            Manager.againstSummaryComponent.SetActive(false);
            Manager.achievementComponent.SetActive(false);
            Manager.musicComponent.SetActive(false);
            Manager.cgComponent.SetActive(false);
        }
        public static async void InitAgainstSummaryComponent()
        {
            Init();
            Manager.againstSummaryComponent.SetActive(true);
            var summarys = await Command.NetCommand.DownloadOwnerAgentSummaryAsync(Info.AgainstInfo.onlineUserInfo.Account, 0, 20);
            summarys.ForEach(summary =>
            {
                var item = Instantiate(Manager.againstSummaryItem);
                item.GetComponent<AgainstSummaryItemManager>().Init(summary);
            });

        }
        public static void InitAchievementComponent()
        {

        }
        public static void InitMusicComponent()
        {

        }
        public static void InitCGComponent()
        {

        }

    }
}
