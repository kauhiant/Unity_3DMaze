using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rename_this_script : MonoBehaviour {
    string[] s = { "利用: ↑ ↓ ← → 來移動", "Q W E R T 施放技能" };
    public Text title;
    int count = 0;
    // Use this for initialization
    
    public void NextButton()
    {
        if (++count >= s.Length)
        {
            SkipButton();
        }
        else
        {
            title.text = s[count];
        }  
    }

    public void SkipButton()
    {
        Destroy(gameObject);
    }
}
