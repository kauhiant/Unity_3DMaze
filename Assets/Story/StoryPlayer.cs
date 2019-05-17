using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Story;
using Maze;

public class StoryPlayer : Story.Action{

    private delegate void dosomething();
    private dosomething action;
    private Queue<dosomething> actions = new Queue<dosomething>();

    private static TalkBox TalkBox
    {
        get { return GlobalAsset.talkBox; }
    }

    public override void Play()
    {
        GameStatus.pause = true;
        if (actions.Count > 0)
        {
            var action = actions.Dequeue();
            action();
        }
        else
        {
            GameStatus.pause = false;
            TalkBox.Hide();
        }
    }

    public StoryPlayer AddMessage(string message)
    {
        actions.Enqueue(() => TalkBox.Show(message, this.Play));
        return this;
    }

    public StoryPlayer AddMessageWithAction(string message, TalkBox.Action action)
    {
        actions.Enqueue(() => {
            action();
            TalkBox.Show(message, this.Play);
        });
        return this;
    }

}


public class ActionPlayer : Story.Action
{
    public delegate void Del();
    private Del doSomething;

    public ActionPlayer(Del doSomething)
    {
        this.doSomething = doSomething;
    }

    public override void Play()
    {
        doSomething();
    }
}
