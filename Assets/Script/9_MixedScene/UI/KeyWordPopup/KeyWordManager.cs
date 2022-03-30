using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class KeyWordManager : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI text;

    public RefreshText(string text)
    {

        //判断一个有标签的单词是否有字段
        string HasIntroduction(string text)
        {
            
        }

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 pos = new Vector3(eventData.position.x, eventData.position.y, 0);
        int linkIndex = TMP_TextUtilities.FindNearestCharacter(text, pos, null, true);
        if (linkIndex > -1)
        {
            TMP_CharacterInfo info = text.textInfo.characterInfo[linkIndex];
            Debug.Log("点击了" + info.character);
        }
    }

}
