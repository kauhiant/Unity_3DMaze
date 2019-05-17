using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class window_Bar : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {
    public GameObject target;
    public float show_position_x, hide_position_x,position_y;

    public void OnPointerEnter(PointerEventData eventData)
    {
        target.GetComponent<RectTransform>().localPosition = new Vector3(show_position_x, position_y, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        target.GetComponent<RectTransform>().localPosition = new Vector3(hide_position_x, position_y, 0);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
