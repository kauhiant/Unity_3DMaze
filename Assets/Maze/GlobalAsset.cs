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

    static public void Reset()
    {
        player = null;
        animals.Clear();
        creaters.Clear();
    }

    static public void setClockTime(float clockTime)
    {
        GlobalAsset.clockTime = clockTime;
    }


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

    static public Maze.Animal lastestAnimal()
    {
        if (animals.Count == 0) return null;
        return animals[GlobalAsset.animals.Count - 1];
    }

    static public Maze.Animal lastestAnimalOnLayer(int layer)
    {
        Maze.Animal animal = null;
        int index = animals.Count - 1;
        while(index >= 0)
        {
            if(animals[index].position.Z.value == layer)
            {
                animal = animals[index];
                break;
            }
            --index;
        }
        return animal;
    }

    static public Maze.Animal lastestAnimalOnLayerColor(int layer, Color color)
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
