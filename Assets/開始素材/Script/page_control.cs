using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class page_control : MonoBehaviour {
    string[] story = {"又是一個炎熱的夏天，我一如往常地坐在電腦桌前，玩著最流行的遊戲-艾爾之光，" ,
                      "突然間，外面下起了雷陣雨，一道閃電不偏不移的打到了我家，變電箱串出了數道火光，我可憐的電腦也因此不能用了，只好打電話請人來幫忙維修，" ,
                      "在等待時間，看著暫時不能運轉得冷氣機，汗臭味慢慢地瀰漫整個房間，我不知不覺的睡著了。" ,
                      "突然聽到有人在呼叫我的名字 : { 正皓 正皓 醒醒啊 }" ,
                      "我睡眼矇矓的看著他，下一刻，發現整個都不對勁，熟悉的床，熟悉的電腦，熟悉的房間，都不見了，只看到一張草蓆，和叫我起來的村長。" };
    int now_page;

    public GameObject story_manager;
    public MapManager controller;

    public Sprite[] image;
    public Image story_piicture;
    public Text title_text;
    public Text story_text;
    public Text page_text;
    public GameObject previous_page_button;
    public GameObject next_page_button;
    public GameObject start_button;

    // Use this for initialization
    void Start () {
        now_page = 0;
        check_page();
	}
	
    public void previous_page()
    {
        now_page--;
        check_page();
    }

    public void next_page()
    {
        now_page++;
        check_page();
    }
    
    public void go_start()
    {
        story_manager.SetActive(false);
        controller.GameReadyStart();
    }

    void check_page()
    {
        story_text.text = story[now_page];
        page_text.text = "第 " + (now_page+1) + " 頁";
        story_piicture.sprite = image[now_page];
        if (now_page == 0)
        {
            previous_page_button.SetActive(false);
            next_page_button.SetActive(true);
            start_button.SetActive(false);
        }
        else if(now_page == story.Length - 1)
        {
            previous_page_button.SetActive(true);
            next_page_button.SetActive(false);
            start_button.SetActive(true);
        }
        else
        {
            previous_page_button.SetActive(true);
            next_page_button.SetActive(true);
            start_button.SetActive(false);
        }
    }
}
