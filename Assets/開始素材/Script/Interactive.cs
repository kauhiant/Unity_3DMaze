using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interactive : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {
    public GameObject talk_block;

    public void OnPointerEnter(PointerEventData eventData)
    {
        talk_block.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        talk_block.SetActive(false);
    }

    
}
