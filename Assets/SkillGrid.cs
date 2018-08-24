using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets;

public class SkillGrid : MonoBehaviour {

    public Image border;
    public SkillButton skill;
    public KeyCode hotKey;
    
	
	void Update () {
        if (Input.GetKey(hotKey))
        {
            skill.ChooseSkill();
            border.color = Color.red;
        }
        else
        {
            border.color = Color.black;
        }
	}
}
