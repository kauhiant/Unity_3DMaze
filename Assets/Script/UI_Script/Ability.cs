using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ability : MonoBehaviour {

    public Text hp;
    public Text ep;
    public Text hungry;
    public Text power;
    public Text armor;
    

    public void UpdateByAnimal(Maze.Animal animal)
    {
        this.hp.text = animal.hp.ToString();
        this.ep.text = animal.ep.ToString();
        this.hungry.text = animal.hungry.ToString();

        this.power.text = animal.Power.ToString();
        this.armor.text = animal.Armor.ToString();
    }
}
