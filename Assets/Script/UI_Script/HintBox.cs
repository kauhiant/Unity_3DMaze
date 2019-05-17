using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintBox : MonoBehaviour {

    public GameObject self;
    public Text title;
    public Text message;
    

    public void ShowMessage(string message, string title = "")
    {
        self.SetActive(true);
        this.title.text = title;
        this.message.text = message;
    }

    public void Hide()
    {
        self.SetActive(false);
    }
}
