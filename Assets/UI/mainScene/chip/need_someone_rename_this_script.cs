using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class need_someone_rename_this_script : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {
    public GameObject target;
    public GameObject[] level1_chip;
    public int i;
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        target.GetComponent<RectTransform>().localPosition = new Vector3(659,0,0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        target.GetComponent<RectTransform>().localPosition = new Vector3(1158,0,0);
    }

    public void get_chip(GameObject[] level, int x)
    {
        level[x].SetActive(true);
    }

    public void lose_chip(GameObject[] level, int x)
    {
        level[x].SetActive(false);
    }

    public void use_for_deBug_get()
    {
        get_chip(level1_chip, i);
    }
    public void use_for_deBug_lose()
    {
        lose_chip(level1_chip, i);
    }
}
