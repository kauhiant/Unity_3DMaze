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
    private GameObject playerBinded;

    private string command;

    private Maze.MapManager manager;
    private List<GameObject> grids = new List<GameObject>();
    private List<GameObject> objs = new List<GameObject>();
    private List<Maze.Animal> enemys = new List<Maze.Animal>();

	// Use this for initialization
	void Start () {

        GlobalAsset.gridSprite = grid.GetComponent<SpriteRenderer>().sprite;
        GlobalAsset.stoneSprite = stone.GetComponent<SpriteRenderer>().sprite;
        GlobalAsset.animalSprite = animal.GetComponent<SpriteRenderer>().sprite;
        GlobalAsset.anamalShape = new Maze.Shape(animalShapes);

        gameMap = new Maze.Map3D(32, 32, 3);
        sceneMap = new Maze.Map2D(gameMap);
        GlobalAsset.map = gameMap;

        for(int i=0; i<30; ++i)
        {
            Maze.Animal enemy = new Maze.Animal(new Maze.Point3D(0,0,0));
            if (GlobalAsset.map.RandomInsertAt(enemy, 1))
                enemys.Add(enemy);
        }

        player = new Maze.Animal(new Maze.Point3D(0, 3, 1));

        gameMap.InsertAt(player.position, player);
        
        manager = new Maze.MapManager(sceneMap, player, camera, 8);

        playerBinded = manager.FindMazeObject(player);
	}

    // Update is called once per frame
    float time = 0;
	void Update ()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            command = "moveUp";
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            command = "moveDown";
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            command = "moveLeft";
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            command = "moveRight";
        }
        else if (Input.GetKey(KeyCode.C))
        {
            command = "changePlain";
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            command = "attack";
            Debug.Log(player.posit.ToString());
        }
        
        time += Time.deltaTime;
        if (time < clockTime) return;
        time = 0;
        Clock();
    }
    
    
    public void Clock()
    {
        switch (command)
        {
            case "moveUp":
                player.MoveFor(Maze.Vector2D.Up);
                break;

            case "moveDown":
                player.MoveFor(Maze.Vector2D.Down);
                break;

            case "moveLeft":
                player.MoveFor(Maze.Vector2D.Left);
                break;

            case "moveRight":
                player.MoveFor(Maze.Vector2D.Right);
                break;

            case "changePlain":
                player.ChangePlain();
                manager.changePlain();
                break;
        }

        command = null;

        foreach(Maze.Animal each in enemys)
        {
            if (each != player)
                each.Auto();
        }

        manager.updateScene();
    }
    
}
