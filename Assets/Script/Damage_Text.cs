using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage_Text : MonoBehaviour {

    public Text text;
    public Animator animator;

    private static Damage_Text_Controller damage_Text_Controller;
    
    private void Awake()
    {
        damage_Text_Controller = GameObject.FindGameObjectWithTag("All_Damage_Text").GetComponent<Damage_Text_Controller>();
    }

    public void Play_Animator(int hurt)
    {
        text.text = (-hurt).ToString();

        Invoke("Recovery", 0.8f);
    }

    private void Recovery()
    {
        if (damage_Text_Controller == null)
            Re_Get_Controller();

        damage_Text_Controller.Recovery_GameObject(gameObject);
    }

    private void Re_Get_Controller()
    {
        damage_Text_Controller = GameObject.FindGameObjectWithTag("All_Damage_Text").GetComponent<Damage_Text_Controller>();
    }


}
