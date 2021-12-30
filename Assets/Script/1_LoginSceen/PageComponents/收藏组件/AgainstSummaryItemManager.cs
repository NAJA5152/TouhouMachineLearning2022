using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
namespace TouhouMachineLearningSummary.Manager
{
    public class AgainstSummaryItemManager : MonoBehaviour
    {
        public string id;
        public Text playerName;
        public Text time;
        public Text tag;
        public Text result;
        AgainstSummaryManager summary;
        public void ChangeTag()
        {

        }
        public void Favioraty()
        {

        }
        public async void Replay()
        {
            AgainstManager.Init();
            AgainstManager.SetReplayMode(summary);
           await AgainstManager.AutoStart();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init(AgainstSummaryManager summary)
        {
            playerName.text = summary.Player1Info.Name + " VS " + summary.Player2Info.Name;
            time.text = summary.UpdateTime.ToString();
            result.text= "Ê¤¸º";
        }
    }
}

