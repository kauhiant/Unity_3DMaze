using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {

    public GameObject camera;
    public GameObject littleMap;
    public GameObject UI_HP;
    public GameObject UI_EP;
    public GameObject UI_Hungry;

    public float clockTime = 0.3f;

    /// <summary>
    /// 6 : right, down, left, up, in, out.
    /// </summary>
    public Sprite[] animalShapes = new Sprite[6];

    public Sprite attack;
    public Sprite straight;
    public Sprite horizon;
    public Sprite create;

    public Sprite gridSprite;
    public Sprite stoneSprite;
    public Sprite createrSprite;
    public Sprite foodSprite;
    public Sprite wallSprite;

    private Maze.Map3D gameMap;
    private Maze.Map2D sceneMap;
    private Maze.Animal player;

    private Command command = Command.None;

    private Maze.MapManager manager;

    private bool isAuto = false;

	// Use this for initialization.
	void Start () {

        Maze.Animal.SetShape(new Maze.Shape(animalShapes));
        Maze.Grid.SetSprite(gridSprite);
        GlobalAsset.attack = attack;
        GlobalAsset.straight = straight;
        GlobalAsset.horizon = horizon;
        GlobalAsset.create = create;
        
        Maze.Stone.SetSprite(stoneSprite);
        Maze.Creater.SetSprite(createrSprite);
        Maze.Food.SetSprite(foodSprite);
        Maze.Wall.SetSprite(wallSprite);

        gameMap = new Maze.Map3D(64, 64, 3);
        sceneMap = new Maze.Map2D(gameMap);
        Maze.MazeObject.SetMaze(gameMap);


        

        
        for(int layer=0; layer<3; ++layer)
        {
            for (int i = 0; i < 6; ++i)
            {
                Maze.Point3D position = gameMap.GetRandomPointOn(layer);
                Maze.Creater creater = new Maze.Creater(position, colorIndex(i));
                if (gameMap.GetAt(position).InsertObj(creater))
                {
                    GlobalAsset.creaters.Add(creater);
                }
            }

            for (int i = 0; i < 200; ++i)
            {
                Maze.Point3D point = gameMap.GetRandomPointOn(layer);
                Maze.Animal animal = new Maze.Animal(point, GlobalAsset.creaters[i % GlobalAsset.creaters.Count], 20);
                if (gameMap.GetAt(point).InsertObj(animal))
                    GlobalAsset.animals.Add(animal);
            }
        }
        
        
        time = 0;
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
            if (manager == null)
                GameStart();
        }
        
        
        Clock();
    }
    
    private void GameStart()
    {
        player = GlobalAsset.animals[GlobalAsset.animals.Count-1];
        player.Strong(10000);
        GlobalAsset.player = player;
        manager = new Maze.MapManager(sceneMap, camera, 8);
    }


    float time;
    public void Clock()
    {
        if (manager != null && manager.GameOver)
            return;
        
        time += Time.deltaTime;
        if (time < clockTime) return;
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
                if (isAuto)
                    each.Auto();
                else
                    PlayerAction();
            }
            else
                each.Auto();

            
            each.Clock();
        }

        for(int i=0; i<GlobalAsset.creaters.Count; ++i)
        {
            if (GlobalAsset.creaters[i].IsDead())
            {
                GlobalAsset.creaters.RemoveAt(i);
                --i;
                continue;
            }
            GlobalAsset.creaters[i].Clock();
        }

        gameMap.Clock();

        if(manager != null)
        {
            manager.UpdateScene();
            UpdataUI();
        }
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

    private void UpdataUI()
    {
        UI_HP.GetComponent<Slider>().value = GlobalAsset.player.hp.BarRate;
        UI_EP.GetComponent<Slider>().value = GlobalAsset.player.ep.BarRate;
        UI_Hungry.GetComponent<Slider>().value = GlobalAsset.player.hungry.BarRate;
    }

    enum Command
    {
        Up,Down,Left,Right,Plain,Attack,Straight,Horizon,Wall,None
    }
}
