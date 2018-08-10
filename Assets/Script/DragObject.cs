using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragObject : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler {   //事件觸發 分別是 開始抓住物件 拖曳物件 放開物件
    public static GameObject itemBeginDragged;          
    public static bool successful = false;      //是否成功放到下方技能欄

    public void OnBeginDrag(PointerEventData eventData)
    {
            itemBeginDragged = Instantiate(gameObject, gameObject.transform.position, Quaternion.identity);   //複製一個一模一樣的物件
            itemBeginDragged.transform.SetParent(transform.parent.parent, false);   //放到適當位置                 
            itemBeginDragged.GetComponent<CanvasGroup>().blocksRaycasts = false;    //設定...我也不知道 跟著教學片
            successful = false;     //初始值
    }

    public void OnDrag(PointerEventData eventData)
    {
        itemBeginDragged.transform.position = Input.mousePosition;      //物件位置=滑鼠座標
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemBeginDragged.GetComponent<CanvasGroup>().blocksRaycasts = true;     //設定...我也不知道 跟著教學片
        if (!successful) Destroy(itemBeginDragged);     //如果沒有放到下面技能欄 銷毀
        itemBeginDragged = null;        //物件初始化
    }
}
