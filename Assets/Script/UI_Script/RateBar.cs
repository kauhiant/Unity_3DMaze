using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RateBar : MonoBehaviour {

    public Slider self;
    public Image front;
    public Image back;
    

    public void setRate(float rate)
    {
        self.value = rate;
    }

    public void setColor(Color color)
    {
        front.color = color;
    }

    public void setActive(bool active)
    {
        self.gameObject.SetActive(active);
    }
}
