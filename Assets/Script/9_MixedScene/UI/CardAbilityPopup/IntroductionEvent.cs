using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
//对卡组列表卡牌模型中进行相关操作
public class IntroductionEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    public UnityEvent onPointerEnter;
    public UnityEvent onPointerExit;
    public UnityEvent onMouse;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right||eventData.button == PointerEventData.InputButton.Left && eventData.clickTime > 2)
        {
            Debug.Log("召唤面板");
        }
    }

    //public UnityEvent onClick;
    public void OnPointerEnter(PointerEventData eventData) => onPointerEnter.Invoke();
    public void OnPointerExit(PointerEventData eventData) => onPointerExit.Invoke();
    //public void OnClick(PointerEventData eventData) => onClick.Invoke();
   
}
