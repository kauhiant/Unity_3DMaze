using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {


    public GameObject grid;
    public GameObject stone;
    public GameObject animal;
    public GameObject camera;

    public float clockTime = 0.5f;

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

        for(int i=0; i<10; ++i)
        {
            Maze.Animal enemy = new Maze.Animal(new Maze.Point3D(0,0,0));
            if (GlobalAsset.map.RandomInsertAt(enemy, 1))
                enemys.Add(enemy);
        }

        player = new Maze.Animal(new Maze.Point3D(0, 3, 0));

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
        }

        //UpdateMap();
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
                PlayerMove(Maze.Vector2D.Up);
                break;

            case "moveDown":
                PlayerMove(Maze.Vector2D.Down);
                break;

            case "moveLeft":
                PlayerMove(Maze.Vector2D.Left);
                break;

            case "moveRight":
                PlayerMove(Maze.Vector2D.Right);
                break;
        }

        Maze.Grid grid = sceneMap.GetAt(player.posit);
        grid.objEvent = Maze.ObjEvent.None;

        command = null;

        foreach(Maze.Animal each in enemys)
        {
            if (each != player)
                each.Auto();
        }

        manager.updateScene();
    }
    
    private void PlayerMove(Maze.Vector2D vector)
    {
        Maze.Grid grid = sceneMap.GetAt(player.posit);
        player.MoveFor(vector);
        updateObject(playerBinded, grid.objEvent);
    }
    
    private void updateObject(GameObject gameObject, Maze.ObjEvent command)
    {
        switch (command)
        {
            case Maze.ObjEvent.moveU:
                MoveForward(gameObject, Vector2.up);
                break;

            case Maze.ObjEvent.moveD:
                MoveForward(gameObject, Vector2.down);
                break;

            case Maze.ObjEvent.moveL:
                MoveForward(gameObject, Vector2.left);
                break;

            case Maze.ObjEvent.moveR:
                MoveForward(gameObject, Vector2.right);
                break;

            case Maze.ObjEvent.shape:
                Debug.Log("turn vector");
                playerBinded.GetComponent<SpriteRenderer>().sprite = player.Shape();
                break;

            case Maze.ObjEvent.None:
                Debug.Log("no event");
                break;
        }
    }

    private void MoveForward(GameObject gameObject, Vector2 vector)
    {
        if(gameObject == playerBinded)
        {
            manager.moveForward(player.vect);
        }
        gameObject.transform.Translate(vector);
    }
}
