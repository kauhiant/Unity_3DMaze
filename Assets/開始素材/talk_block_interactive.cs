﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class talk_block_interactive : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Animator>().SetTrigger("show");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Animator>().SetTrigger("hide");
    }

    public void start_button()
    {
        Application.LoadLevel(1);
    }
}
