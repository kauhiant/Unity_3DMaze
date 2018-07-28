using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {
    public GameObject camera;

    public float clockTime = 0.3f;

    public Sprite[] animalShapes = new Sprite[6];

    public Sprite attack;
    public Sprite straight;
    public Sprite horizon;
    public Sprite food;
    public Sprite grid;
    public Sprite stone;

    private Maze.Map3D gameMap;
    private Maze.Map2D sceneMap;
    private Maze.Animal player;

    private Command command = Command.None;

    private Maze.MapManager manager;
    private List<Maze.Animal> enemys = new List<Maze.Animal>();

	// Use this for initialization
	void Start () {
        GlobalAsset.gridSprite = grid;
        GlobalAsset.stoneSprite = stone;
        GlobalAsset.anamalShape = new Maze.Shape(animalShapes);

        GlobalAsset.attack = attack;
        GlobalAsset.straight = straight;
        GlobalAsset.horizon = horizon;
        GlobalAsset.food = food;

        gameMap = new Maze.Map3D(31, 20, 3);
        sceneMap = new Maze.Map2D(gameMap);
        GlobalAsset.map = gameMap;


        player = new Maze.Animal(new Maze.Point3D(0, 3, 1),RandomColor(),20);
        GlobalAsset.player = player;
        gameMap.HardInsertAt(player.position, player);
        manager = new Maze.MapManager(sceneMap, player, camera, 8);

        for (int i = 0; i < 100; ++i)
        {
            Maze.Animal enemy = new Maze.Animal(new Maze.Point3D(0, 0, 0),RandomColor(),10);
            if (GlobalAsset.map.RandomInsertAt(enemy, 1))
                enemys.Add(enemy);
        }
        
        time = 0;
        clockLock = false;
    }

    // Update is called once per frame
	void FixedUpdate ()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            command = Command.Up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            command = Command.Down;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            command = Command.Left;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            command = Command.Right;
        }
        else if (Input.GetKey(KeyCode.C))
        {
            command = Command.Plain;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            command = Command.Attack;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            command = Command.Straight;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            command = Command.Horizon;
        }
        
        
        Clock();
    }
    

    float time;
    bool clockLock;
    public void Clock()
    {
        if (manager.gameOver) return;
        time += Time.deltaTime;
        if (time < clockTime) return;
        if (clockLock) return;
        clockLock = true;
        time = 0;

        Maze.SkillManager.clear();

        switch (command)
        {
            case Command.Up:
                player.MoveFor(Maze.Vector2D.Up);
                break;

            case Command.Down:
                player.MoveFor(Maze.Vector2D.Down);
                break;

            case Command.Left:
                player.MoveFor(Maze.Vector2D.Left);
                break;

            case Command.Right:
                player.MoveFor(Maze.Vector2D.Right);
                break;

            case Command.Plain:
                player.ChangePlain();
                manager.changePlain();
                break;

            case Command.Attack:
                player.Attack();
                break;

            case Command.Straight:
                player.Straight();
                break;

            case Command.Horizon:
                player.Horizon();
                break;
        }
        command = Command.None;
        
        for(int i=0; i< enemys.Count; ++i)
        {
            Maze.Animal each = enemys[i];
            if (each.isDead)
            {
                enemys.RemoveAt(i);
                --i;
                continue;
            }
            each.Auto(10);
        }
        
        manager.updateScene();
        
        clockLock = false;
    }

    private Color RandomColor()
    {
        switch (UnityEngine.Random.Range(0, 6))
        {
            case 0:
                return Color.white;
            case 1:
                return Color.red;
            case 2:
                return Color.green; 
            case 3:
                return Color.blue;
            case 4:
                return Color.cyan;
            case 5:
                return Color.yellow;
            default:
                return Color.magenta;
        }
    }
    

    enum Command
    {
        Up,Down,Left,Right,Plain,Attack,Straight,Horizon,None
    }
}
