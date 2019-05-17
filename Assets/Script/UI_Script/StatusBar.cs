using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public Slider HP;
    public Slider EP;
    public Slider Hungry;

    

    public void UpdateFor(Maze.Animal animal)
    {
        HP.value = animal.hp.BarRate;
        EP.value = animal.ep.BarRate;
        Hungry.value = animal.hungry.BarRate;
    }
    
    public void SetActive(bool active)
    {
        this.gameObject.SetActive(active);
    }
}