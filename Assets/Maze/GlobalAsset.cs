using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAsset
{
    static public bool gameOver;
    

    static public Sprite attack;
    static public Sprite straight;
    static public Sprite horizon;
    static public Sprite create;
    static public Sprite mark;
    
    

    static public Maze.Animal player;

    static public List<Maze.Animal> animals = new List<Maze.Animal>();
    static public List<Maze.Creater> creaters = new List<Maze.Creater>();
}
