using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkBox : MonoBehaviour {

    public GameObject self;
    public Text talkerName;
    public Image talkerNameBackground;
    public Text text;
    public Image talkerImage;
    

    public void Show(string message, string talkerName = null, Sprite talkerImage = null)
    {
        self.SetActive(true);
        setText(message);
        setTalkerName(talkerName);
        setTalkerImage(talkerImage);
    }
    

    public void Hide()
    {
        self.SetActive(false);
    }



    private void setText(string message)
    {
        text.text = message;
    }

    private void setTalkerName(string name)
    {
        talkerName.text = name;

        if (name == null || name == "")
        {
            talkerNameBackground.gameObject.SetActive(false);
        }
        else
        {
            talkerNameBackground.gameObject.SetActive(true);
        }


    }

    private void setTalkerImage(Sprite sprite)
    {
        talkerImage.sprite = sprite;

        if (sprite == null)
        {
            talkerImage.gameObject.SetActive(false);
        }
        else
        {
            talkerImage.gameObject.SetActive(true);
        }
    }
}
