using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class button_script : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {   //滑鼠靠近顯示該技能的名稱和說明
    public HintBox hintBox;
    public string skillName;
    public string skillIntro;
    
    public void OnPointerEnter(PointerEventData eventData)      //當滑鼠"移到"物件上面
    {
        hintBox.ShowMessage(skillIntro, skillName);
    }

    public void OnPointerExit(PointerEventData eventData)       //當滑鼠"離開"物件上面
    {
        hintBox.Hide();
    }
    
}
