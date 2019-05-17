using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TalkBox : MonoBehaviour {

    public GameObject self;
    public Text talkerName; // not used.
    public Image talkerImage; // not used.
    public Image talkerNameBackground; // not used.
    public Text text;

    public delegate void Action();
    private Action callWhenEnd = null;

    public bool end = false;
    private bool endOfLine = false; // false: showingChar, true:endOfLine.
    private int charCount = 0;
    private string message;

    

    public void Show(string message, Action callWhenEnd=null)
    {
        self.SetActive(true);
        SetText(message);
        this.callWhenEnd = callWhenEnd;
    }
    

    public void Hide()
    {
        self.SetActive(false);
    }

    IEnumerator ShowMessageByEachChar()
    {
        charCount = 0;
        endOfLine = false;
        text.text = "";

        while(charCount != message.Length)
        {
            text.text += message[charCount++];
            yield return new WaitForSecondsRealtime(0.1f);
        }

        endOfLine = true;
    }
    
    private void SetText(string message)
    {
        end = false;
        StopAllCoroutines();
        this.message = message;
        StartCoroutine(ShowMessageByEachChar());
    }

    private void ShowAllChar()
    {
        text.text = message;
        charCount = message.Length;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (endOfLine)
            {
                end = true;

                if (callWhenEnd != null)
                    callWhenEnd();
                else
                    Hide();
            }
            else
            {
                ShowAllChar();
            }
        }
    }
}
