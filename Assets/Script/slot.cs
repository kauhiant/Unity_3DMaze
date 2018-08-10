using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class slot : MonoBehaviour ,IDropHandler{
    public GameObject under_skill;      //用來確認是否放在下方技能欄
    
    public void OnDrop(PointerEventData eventData)      //拖拉結束觸發
    { 
        if (gameObject.transform.parent.gameObject==under_skill)    //判斷是否在下方技能欄
        {
            DragObject.successful = true;               //取消銷毀
            foreach (Transform child in transform)      //清除原先所有的孩子
            {
                GameObject.Destroy(child.gameObject);
            }
            DragObject.itemBeginDragged.transform.SetParent(transform); //塞進去  用設定父親
            Destroy(DragObject.itemBeginDragged.GetComponent<DragObject>());     //刪除拖曳程式 在下方技能欄的技能不能再拖曳
            foreach (Transform child in DragObject.itemBeginDragged.transform)   //設定技能外框 顯示
            {
                if (child.name == "Outline_Mask")
                {
                    child.GetComponent<Image>().fillAmount = 1;
                }
            }
        }
        else  //否則銷毀
        {
            Destroy(DragObject.itemBeginDragged);
        }
    }
}
