using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maze;
using Assets;
using UnityEngine.UI;


// 操控腳色專用.
// 現在只能控制上下左右.
// 技能操控交給SkillButton.
public class SkillBar : MonoBehaviour
{
    public static SkillBar Main;
    private Animal.MoveCommand command { set { GlobalAsset.player.moveCommamd = value; } }
    private Image Image;
    private Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
    private bool mouseControlLock = false;

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void Flash(float time)
    {
        StartCoroutine(FlashColor(time));
    }

    void Start()
    {
        SkillBar.Main = this;
        Image = gameObject.GetComponent<Image>();
    }

    void Update()
    {

        if (GlobalAsset.player == null)
            return;
        if (GameStatus.pause)
            return;

        // [滑鼠控制]
        // - - - [ [ [ 這裡可能很耗效能 ] ] ] - - -
        switch (MouseVector())
        {
            case Vector2D.Up:
                if (Input.GetMouseButton(0))
                    command = Animal.MoveCommand.MoveUp;
                else
                    command = Animal.MoveCommand.TurnUp;
                break;

            case Vector2D.Down:
                if (Input.GetMouseButton(0))
                    command = Animal.MoveCommand.MoveDown;
                else
                    command = Animal.MoveCommand.TurnDown;
                break;

            case Vector2D.Left:
                if (Input.GetMouseButton(0))
                    command = Animal.MoveCommand.MoveLeft;
                else
                    command = Animal.MoveCommand.TurnLeft;
                break;

            case Vector2D.Right:
                if (Input.GetMouseButton(0))
                    command = Animal.MoveCommand.MoveRight;
                else
                    command = Animal.MoveCommand.TurnRight;
                break;
        }

        // [鍵盤控制]
        if (Input.GetKey(KeyCode.UpArrow))
        {
            command = Animal.MoveCommand.MoveUp;
            mouseControlLock = true;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            command = Animal.MoveCommand.MoveDown;
            mouseControlLock = true;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            command = Animal.MoveCommand.MoveLeft;
            mouseControlLock = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            command = Animal.MoveCommand.MoveRight;
            mouseControlLock = true;
        }

    }

    private Vector2D MouseVector()
    {
        if (Input.GetMouseButton(0))
            mouseControlLock = false;

        if (mouseControlLock)
            return Vector2D.Null;

        Vector2 vector = (Vector2)Input.mousePosition - center;
        if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
        {
            if (vector.x > 0)
                return Vector2D.Right;
            else
                return Vector2D.Left;
        }
        else
        {
            if (vector.y > 0)
                return Vector2D.Up;
            else
                return Vector2D.Down;
        }
    }

    private IEnumerator FlashColor(float time)
    {
        var originColor = Image.color;
        time /= 3;

        Image.color = Color.clear;
        yield return new WaitForSeconds(time);

        Image.color = originColor;
        yield return new WaitForSeconds(time);

        Image.color = Color.clear;
        yield return new WaitForSeconds(time);

        Image.color = originColor;
    }
}
