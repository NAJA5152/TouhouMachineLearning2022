using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TouhouMachineLearningSummary.Model;
using Sirenix.OdinInspector;

namespace TouhouMachineLearningSummary.Info
{

    public class DialogueInfo : MonoBehaviour
    {
        public static DialogueInfo instance;
        void Awake() => instance = this;
        public GameObject dialogueCanvas;
        public GameObject selectUi;
        public GameObject left;
        public GameObject right;
        public Text name ;
        public Text text;
        public static bool SelectMode { get; set; } = false;
        //跳过对话
        public static bool IsJump { get; set; } = false;
        public static bool IsSelectOver { get; set; } = false;
        public static bool IsShowNextText { get; set; } = false;
        public static int CurrentPoint { get; set; } = 0;
        public static int SelectBranch { get; set; } = 0;
        [ShowInInspector]
        public static DialogueModel currnetDialogueModel { get; set; } = new DialogueModel();
        [ShowInInspector]
        public static List<DialogueModel> DialogueModels { get; set; } = new List<DialogueModel>();
    }
}
