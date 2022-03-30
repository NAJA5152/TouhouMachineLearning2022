using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyWordManager : MonoBehaviour,IPointerClickHandler
{
    TextMeshProUGUI text;
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 pos = new Vector3(eventData.position.x, eventData.position.y, 0);
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, pos, Camera.main);
        if (linkIndex > -1)
        {
            Debug.Log("µã»÷ÁË"+linkIndex);
            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];

        }
    }
   
}
