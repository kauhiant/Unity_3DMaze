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
    public Sprite createSprite;
    public Sprite food;
    public Sprite grid;
    public Sprite stone;
    public Sprite wall;
    public Sprite createrSprite;

    private Maze.Map3D gameMap;
    private Maze.Map2D sceneMap;
    private Maze.Animal player;

    private Command command = Command.None;

    private Maze.MapManager manager;

    private bool isAuto = false;

	// Use this for initialization
	void Start () {
        GlobalAsset.gridSprite = grid;
        GlobalAsset.stoneSprite = stone;
        GlobalAsset.animalShape = new Maze.Shape(animalShapes);

        GlobalAsset.attack = attack;
        GlobalAsset.straight = straight;
        GlobalAsset.horizon = horizon;
        GlobalAsset.create = createSprite;
        GlobalAsset.food = food;
        GlobalAsset.wall = wall;
        GlobalAsset.createrSprite = createrSprite;

        gameMap = new Maze.Map3D(31, 20, 3);
        sceneMap = new Maze.Map2D(gameMap);
        GlobalAsset.map = gameMap;
        

        for (int i = 0; i < 6; ++i)
        {
            Maze.Point3D position = GlobalAsset.map.GetRandomPointOn(1);
            Maze.Creater creater = new Maze.Creater(position, colorIndex(i));
            if (GlobalAsset.map.InsertAt(position,creater))
                GlobalAsset.creaters.Add(creater);
        }

        for(int i = 0; i< 50; ++i)
        {
            Maze.Point3D point = GlobalAsset.map.GetRandomPointOn(i%3);
            Maze.Animal animal = new Maze.Animal(point, GlobalAsset.creaters[i%GlobalAsset.creaters.Count], 10);
            if (GlobalAsset.map.InsertAt(point,animal))
                GlobalAsset.animals.Add(animal);
        }


        player = GlobalAsset.animals[GlobalAsset.animals.Count-1];
        GlobalAsset.player = player;
        manager = new Maze.MapManager(sceneMap, camera, 8);

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
        else if (Input.GetKey(KeyCode.F))
        {
            command = Command.Wall;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            SwitchAuto();
        }
        
        
        Clock();
    }
    

    float time;
    bool clockLock;
    public void Clock()
    {
        if (manager.GameOver) return;
        time += Time.deltaTime;
        if (time < clockTime) return;
        if (clockLock) return;
        clockLock = true;
        time = 0;

        Maze.SkillManager.clear();
        
        
        for(int i=0; i< GlobalAsset.animals.Count; ++i)
        {
            Maze.Animal each = GlobalAsset.animals[i];

            if (each.isDead)
            {
                GlobalAsset.animals.RemoveAt(i);
                --i;
                continue;
            }

            if (each == player)
            {
                if(isAuto)
                    each.Clock();
                else
                    PlayerAction();
            }
            else
                each.Clock();
            
        }

        for(int i=0; i<GlobalAsset.creaters.Count; ++i)
        {
            if (GlobalAsset.creaters[i].isDead())
            {
                GlobalAsset.creaters.RemoveAt(i);
                --i;
                continue;
            }
            GlobalAsset.creaters[i].update();
        }
        
        manager.UpdateScene();
        
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

    private Color colorIndex(int index)
    {
        switch (index)
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
    

    private void SwitchAuto()
    {
        if (isAuto)
        {
            isAuto = !isAuto;
            command = Command.None;
        }
        else
        {
            isAuto = !isAuto;
        }
    }

    private void PlayerAction()
    {
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
                manager.ChangePlain();
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

            case Command.Wall:
                player.Build();
                break;
        }
        command = Command.None;
    }

    enum Command
    {
        Up,Down,Left,Right,Plain,Attack,Straight,Horizon,Wall,None
    }
}
