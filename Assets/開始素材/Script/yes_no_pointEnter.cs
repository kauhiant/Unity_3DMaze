using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class yes_no_pointEnter : MonoBehaviour {

    

    public void change_color_pointEnter()
    {
        this.GetComponent<Image>().color = Color.yellow;
    }

    public void change_color_pointExit()
    {
        this.GetComponent<Image>().color = Color.white;
    }
}
