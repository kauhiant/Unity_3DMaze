using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Assets;

public class ShowSkillHint : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {   //滑鼠靠近顯示該技能的名稱和說明
    public HintBox hintBox;
    public SkillButton skill;
    

    public void OnPointerEnter(PointerEventData eventData)      //當滑鼠"移到"物件上面
    {
        hintBox.ShowMessage(skill.detail, skill.name);
    }

    public void OnPointerExit(PointerEventData eventData)       //當滑鼠"離開"物件上面
    {
        hintBox.Hide();
    }
    
}
