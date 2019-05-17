using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets;

// 一個技能對應一個 SkillGrid.
// 一個 SkillGrid 可以放一個 SkillButton.
public class SkillGrid : MonoBehaviour {

    public Image border;
    public SkillButton skill;
    public KeyCode hotKey;

    

    void Update () {
        if (Input.GetKeyDown(hotKey))
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
