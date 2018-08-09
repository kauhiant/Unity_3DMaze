using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAsset
{
    static public Sprite attack;
    static public Sprite straight;
    static public Sprite horizon;
    static public Sprite create;

    static public Sprite playerMark;
    static public Sprite createrMark;

    static public float clockTime=0.3f;
    static public Maze.Animal player;

    static public List<Maze.Animal> animals = new List<Maze.Animal>();
    static public List<Maze.Creater> creaters = new List<Maze.Creater>();

    static public float RateOfColorOn(Color color, int layer)
    {
        int count = 0;
        int total = 0;
        foreach(Maze.Creater each in creaters)
        {
            if (each.position.Z.value == layer)
            {
                total += each.Level;

                if (each.Color.Equals(color))
                    count += each.Level;
            }

        }

        return (float)count / (float)total;
    }
}
