using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RateBar2 : MonoBehaviour {
    public GameObject slider;
    public GameObject[] front;
    

    public void SetBar(int[] value)
    {
        float x = 0;        //物件的X座標
        float all = 0;      //計算總數
        for (int i = 0; i < 7; i++) all += value[i];    //加總用
        for(int i = 0; i < 7; i++)
        {
            float widthRate = value[i] / all;
            front[i].GetComponent<RectTransform>().localScale = new Vector3(widthRate, 1, 1);  //設定大小
            front[i].GetComponent<RectTransform>().localPosition = new Vector3(x, -25, 0);          //設定位置
            x += widthRate * slider.GetComponent<RectTransform>().rect.width;      //定位
        }
    }

    public void SetActive(bool active)
    {
        this.gameObject.SetActive(active);
    }
}
