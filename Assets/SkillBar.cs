using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maze;
using Assets;


// 操控腳色專用.
// 現在只能控制上下左右.
// 既能操控交給SkillButton.
public class SkillBar : MonoBehaviour {
    private Animal.Command command { set { GlobalAsset.player.command = value; } }
    
    
	void Start () {
		
	}
	
	void Update () {

        if (GlobalAsset.player == null)
            return;

        // [方向]
        if (Input.GetKey(KeyCode.UpArrow))
        {
            command = Animal.Command.Up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            command = Animal.Command.Down;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            command = Animal.Command.Left;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            command = Animal.Command.Right;
        }
    }
}
