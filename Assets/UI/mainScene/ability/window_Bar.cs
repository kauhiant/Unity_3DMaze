using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class window_Bar : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {
    public GameObject target;
    public float show_position_x, hide_position_x, position_y;
    public float MoveSpeed;
    float now_position;
    bool window_move = false, show_or_hide = false;

    

    public void OnPointerEnter(PointerEventData eventData)
    {
        window_move = true;
        show_or_hide = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        window_move = true;
        show_or_hide = false;
    }
    void Update()
    {
        
        if (window_move) move();
    }

    void move()
    {
        now_position = target.GetComponent<RectTransform>().localPosition.x;
        if (show_or_hide)
        {
            if (now_position + MoveSpeed <= show_position_x)
            {
                target.GetComponent<RectTransform>().localPosition = new Vector3(now_position + MoveSpeed, position_y, 0);
            }
            else
            {
                target.GetComponent<RectTransform>().localPosition = new Vector3(show_position_x, position_y, 0);
                window_move = false;
            }
        }
        else
        {
            if (now_position - MoveSpeed >= hide_position_x)
            {
                target.GetComponent<RectTransform>().localPosition = new Vector3(now_position - MoveSpeed, position_y, 0);
            }
            else
            {
                target.GetComponent<RectTransform>().localPosition = new Vector3(hide_position_x, position_y, 0);
                window_move = false;
            }
        } 
    }
}
