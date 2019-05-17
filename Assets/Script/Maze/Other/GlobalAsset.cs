using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAsset
{
    static public GameController controller;
    static public Sprite[] gridSprites;

    static public Sprite attack;
    static public Sprite straight;
    static public Sprite horizon;
    static public Sprite create;

    static public Sprite playerMark;
    static public Sprite createrMark;

    static public PositionHint positionHint;
    static public HintBox hintBox;
    static public TalkBox talkBox;
    static public GraphicTest smallMap;

    static public float clockTime = 0.4f;
    static public Maze.Animal player;
    static public int seenRange = 8;
    static public int mapSize = 64;

    static public List<Maze.Animal> animals = new List<Maze.Animal>();
    static public List<Maze.Creater> creaters = new List<Maze.Creater>();

    static public void Reset()
    {
        player = null;
        animals.Clear();
        creaters.Clear();
    }
    

    static public Color[] colors = // 目前固定的7種顏色.
        {Color.white, Color.red,  Color.green, Color.blue, Color.cyan, Color.yellow, Color.magenta};


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

    static public int[] PowerOfColorOn(int layer)
    {
        int[] counts = new int[colors.Length];
        foreach (var each in creaters)
        {
            if (each.position.Z.value == layer)
            {
                for (int i = 0; i < colors.Length; ++i)
                {
                    if (each.Color.Equals(colors[i]))
                        counts[i] += each.Level;
                }
            }
        }

        return counts;
    }

    static public Maze.Animal LastestAnimal()
    {
        if (animals.Count == 0) return null;
        return animals[GlobalAsset.animals.Count - 1];
    }

    static public Maze.Animal LastestAnimalOnLayerColor(int layer, Color color)
    {
        Maze.Animal animal = null;
        int index = animals.Count - 1;
        while (index >= 0)
        {
            if (animals[index].position.Z.value == layer && animals[index].Color.Equals(color))
            {
                animal = animals[index];
                break;
            }
            --index;
        }
        return animal;
    }
}
