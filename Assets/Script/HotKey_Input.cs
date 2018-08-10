using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotKey_Input : MonoBehaviour {
    public GameObject All_skill;            //技能
    public GameObject skill_text_block; //技能說明文字
    public GameObject[] under_skill;
    bool skill_status=false;  //預設關閉
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (skill_status)   //當技能欄關閉時 要讓技能說明文字也一起關閉
            {
                skill_text_block.SetActive(false);
            }
            skill_status = !skill_status;   //狀態反轉
            All_skill.SetActive(skill_status);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) { use_skill(0); }     //技能使用 1~5
        if (Input.GetKeyDown(KeyCode.Alpha2)) { use_skill(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { use_skill(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { use_skill(3); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { use_skill(4); }
    }

    void use_skill(int i)
    {
        foreach (Transform child in under_skill[i].transform)      //清除原先所有的孩子
        {
            child.GetComponent<skill>().setuse = true;              //設定使用技能
        }
    }
}
