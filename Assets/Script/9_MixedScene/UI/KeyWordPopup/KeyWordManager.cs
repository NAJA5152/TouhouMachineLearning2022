using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.EventSystems;
namespace TouhouMachineLearningSummary.Manager
{
    public partial class KeyWordManager : MonoBehaviour, IPointerClickHandler
    {
        static KeyWordManager manager;
        void Awake() => manager = this;

        public TextMeshProUGUI textMesh;
        static List<KeyWordModel> words = new List<KeyWordModel>();
        public static void RefreshText(string text)
        {
            words = TranslateManager.CheckKeyWord(text);

            words.Select(x => x.tag).Distinct().ToList().ForEach(tag =>
            {
                text = text.Replace(tag, $"<u>{tag}</u>");
            });
            manager.textMesh.text = text;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            Vector3 pos = new Vector3(eventData.position.x, eventData.position.y, 0);
            int linkIndex = TMP_TextUtilities.FindNearestCharacter(textMesh, pos, null, true);
            if (linkIndex > -1)
            {
                TMP_CharacterInfo info = textMesh.textInfo.characterInfo[linkIndex];
                Debug.Log("µã»÷ÁË" + info.character);
            }
        }

    }
}

