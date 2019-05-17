using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Marquee : MonoBehaviour {
    public Text[] Marquee_Text;     
    public string[] show_text;      //要顯示的文字
    public int text_length;         //能放入記個字

    int now_string = 0, now_char = 0;   //現在到第幾串，第幾個字
    bool empty_text=true;               //2個文字顯示器， 偷懶用bool代替
    

    public void set_Marquee_text()
    {
        if (show_text[now_string].Length-now_char > text_length)     //大概10的文字會塞滿一串
        {
            if (empty_text) //設定文字
            {
                Marquee_Text[0].text = show_text[now_string].Substring(now_char, text_length);
            }
            else
            {
                Marquee_Text[1].text = show_text[now_string].Substring(now_char, text_length);
            }
            empty_text = !empty_text;
            now_char += text_length;
        }
        else
        {
            if (empty_text)
            {
                Marquee_Text[0].text = show_text[now_string].Substring(now_char, show_text[now_string].Length - now_char);
            }
            else
            {
                Marquee_Text[1].text = show_text[now_string].Substring(now_char, show_text[now_string].Length - now_char);
            }
            empty_text = !empty_text;
            now_char = 0;
            now_string++;
            if (now_string >= show_text.Length) now_string = 0;
        }
    }
    
}
