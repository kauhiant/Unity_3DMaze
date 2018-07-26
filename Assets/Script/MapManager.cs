using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {


    public GameObject grid;
    public GameObject stone;
    public GameObject animal;
    public GameObject camera;

    public float clockTime = 0.3f;

    public Sprite[] animalShapes = new Sprite[6];

    private Maze.Map3D gameMap;
    private Maze.Map2D sceneMap;
    private Maze.Animal player;

    private Command command = Command.None;

    private Maze.MapManager manager;
    private List<Maze.Animal> enemys = new List<Maze.Animal>();

	// Use this for initialization
	void Start () {
        GlobalAsset.gridSprite = grid.GetComponent<SpriteRenderer>().sprite;
        GlobalAsset.stoneSprite = stone.GetComponent<SpriteRenderer>().sprite;
        GlobalAsset.animalSprite = animal.GetComponent<SpriteRenderer>().sprite;
        GlobalAsset.anamalShape = new Maze.Shape(animalShapes);

        gameMap = new Maze.Map3D(20, 20, 3);
        sceneMap = new Maze.Map2D(gameMap);
        GlobalAsset.map = gameMap;


        player = new Maze.Animal(new Maze.Point3D(0, 3, 1));
        GlobalAsset.player = player;
        gameMap.HardInsertAt(player.position, player);
        manager = new Maze.MapManager(sceneMap, player, camera, 8);

        for (int i = 0; i < 100; ++i)
        {
            Maze.Animal enemy = new Maze.Animal(new Maze.Point3D(0, 0, 0));
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
        else if (Input.GetKey(KeyCode.Space))
        {
            command = Command.Attack;
        }
        
        
        Clock();
    }
    

    float time;
    bool clockLock;
    public void Clock()
    {
        time += Time.deltaTime;
        if (time < clockTime) return;
        if (clockLock) return;
        clockLock = true;
        time = 0;

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
        }
        command = Command.None;
        Maze.Point3D playerPosition = player.position.Copy();
        
        for(int i=0; i< enemys.Count; ++i)
        {
            Maze.Animal each = enemys[i];
            each.Auto(10);
        }
        
        manager.updateScene();
        
        clockLock = false;
    }
    

    enum Command
    {
        Up,Down,Left,Right,Plain,Attack,None
    }
}
