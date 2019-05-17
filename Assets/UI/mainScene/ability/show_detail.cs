using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class show_detail : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public HintBox hintBox;
    public string title, detail;
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        detail = detail.Replace('n', '\n');
        hintBox.ShowMessage(this.detail, this.title);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hintBox.Hide();
    }
}


