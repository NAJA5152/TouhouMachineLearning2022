using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TouhouMachineLearningSummary.Model;
namespace TouhouMachineLearningSummary.Info
{

    public class DialgueInfo : MonoBehaviour
    {
        public static DialgueInfo instance;
        void Awake() => instance = this;
        public GameObject DialogueCanvas;

        public GameObject Left;
        public GameObject Right;
        public Text Text;
        public static bool RunNextOperations { get; set; } = false;
        public static int CurrentPoint { get; set; } = 0;
        public static int SelectBranch { get; set; } = 0;
        public static DialogueModel currnetDialogueModel { get; set; } = new DialogueModel();
        public static List<DialogueModel> DialogueModels { get; set; } = new List<DialogueModel>();
    }
}
