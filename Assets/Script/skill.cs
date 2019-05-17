using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class skill : MonoBehaviour {    //技能冷卻
    //public KeyCode key;
    public float cold_time;     //冷卻時間
    public GameObject mask;     //外框遮罩
    public GameObject mask2;    //圖片遮罩
    public Text text_cold_time; //冷卻時間_文字
    public bool use = false;    //使否再冷卻
    float now_cold_time;        //現在的冷卻時間
    public bool setuse=false;   //用來觸發使用

   

    // Use this for initialization
    void Start () { 

	}
	
	// Update is called once per frame
	void Update () {
        if (setuse) use_skill();    //是否使用

        if (use)        //設定冷卻時狀態
        {
            text_cold_time.text = "" + Mathf.Ceil(now_cold_time);
            now_cold_time -= Time.deltaTime;
            mask.GetComponent<Image>().fillAmount = 1-(now_cold_time/cold_time);
            if (mask.GetComponent<Image>().fillAmount >= 1)
            {
                text_cold_time.text = "";
                mask.GetComponent<Image>().fillAmount = 1;
                use = false;
                mask2.SetActive(false);
            }
        }
        
	}
    
    public void use_skill()
    {
        setuse = false;     //觸發判斷重設
        if (!use)           //設定冷卻初始狀態
        {
            text_cold_time.text = "" + cold_time;
            now_cold_time = cold_time;
            mask.GetComponent<Image>().fillAmount = 0;
            mask2.SetActive(true);
            use = true;
        }
    }
}
