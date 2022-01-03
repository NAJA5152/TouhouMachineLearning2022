using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
namespace TouhouMachineLearningSummary.Manager
{
    public class AgainstSummaryItemManager : MonoBehaviour
    {
        public Text playerName;
        public Text time;
        public Text tag;
        public Text result;
        AgainstSummaryManager Summary { get; set; }
        public void ChangeTag()
        {

        }
        public void Favioraty()
        {

        }
        public async void Replay()
        {
            AgainstManager.Init();
            AgainstManager.ReplayStart(Summary);
        }
        public void Init(AgainstSummaryManager summary)
        {
            Summary = summary;
            playerName.text = summary.Player1Info.Name + " VS " + summary.Player2Info.Name;
            time.text = summary.UpdateTime.ToString();
            result.text = "Ê¤¸º";
        }
    }
}

