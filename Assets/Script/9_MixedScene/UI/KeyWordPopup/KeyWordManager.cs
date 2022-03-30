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

        //�ж�һ���б�ǩ�ĵ����Ƿ����ֶ�
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
            Debug.Log("�����" + info.character);
        }
    }

}
