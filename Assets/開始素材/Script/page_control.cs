using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class page_control : MonoBehaviour {
    string[] story = {"維度，或稱維數是什麼樣的概念呢？該用物理還是哲學的方式來解釋呢？" ,
                      "但就以現在而言，我們拿我們看到的面向來看吧" ,
                      "0是點、1是線、2是面、3為世界，那4維呢？還是空間嗎？抑或是時間？我們無從知曉" ,
                      "而現在，我們來個小遊戲，以資料結構而言是三維，但只呈現二維的部分，你會覺得這是幾維呢？" ,
                      "還請按下GO的按鈕來體驗看看" };
    int now_page;

    public GameObject story_manager;
    public GameController controller;

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
