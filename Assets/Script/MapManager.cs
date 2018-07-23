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
        
        manager = new Maze.MapManager(sceneMap, player.posit, 8);

        playerBinded = manager.FindMazeObject(player);
        camera.transform.position = playerBinded.transform.position;
        camera.transform.Translate(Vector3.back);
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

    private void updateObject(GameObject gameObject, string commamd)
    {
        switch (command)
        {
            case "moveUp":
                MoveForward(gameObject, Vector2.up);
                break;

            case "moveDown":
                MoveForward(gameObject, Vector2.down);
                break;

            case "moveLeft":
                MoveForward(gameObject, Vector2.left);
                break;

            case "moveRight":
                MoveForward(gameObject, Vector2.right);
                break;
        }
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
/*
            case "changePlain":
                ChangePlain();
                break;

            case "attack":
                player.Attack();
                break;*/
        }

        command = null;

        //UpdateMap();
        //ClearMap();
        //ShowMap();

        foreach(Maze.Animal each in enemys)
        {
            if (each != player)
                each.Auto();
        }


    }
    /*

    private void CreateObjAt(int x, int y, Maze.MazeObject obj)
    {
        GameObject temp = new GameObject();
        temp.transform.position = new Vector3(x, y, 0);
        temp.AddComponent<SpriteRenderer>();
        temp.GetComponent<SpriteRenderer>().sprite = obj.Shape();
        temp.GetComponent<SpriteRenderer>().sortingLayerName = "object";

        objs.Add(temp);

        if(obj == player)
        {
            playerBinded = temp;
            camera.transform.localPosition = (playerBinded.transform.localPosition);
            camera.transform.Translate(Vector3.back);
        }
    }

    private void CreateGridAt(int x, int y, Maze.Grid grid)
    {
        GameObject temp = new GameObject();
        temp.transform.position = new Vector3(x,y,0);
        temp.AddComponent<SpriteRenderer>();
        temp.GetComponent<SpriteRenderer>().sprite = grid.shape;
        temp.GetComponent<SpriteRenderer>().sortingLayerName = "grid";
        grids.Add(temp);
    }

    private void RemoveGridAt(int x, int y)
    {

    }

    private void ChangePlain()
    {
        player.ChangePlain();
        ClearMap();
        ShowMap();
    }

    private void ClearMap()
    {
        while(grids.Count != 0)
        {
            Destroy(grids[0]);
            grids.RemoveAt(0);
        }

        while(objs.Count != 0)
        {
            Destroy(objs[0]);
            objs.RemoveAt(0);
        }
        
        
    }

    private void ShowMap()
    {
        Maze.Iterator iter = new Maze.Iterator(player.posit, 8);

        do
        {
            Maze.Point2D point = iter.Iter;

            Maze.Grid grid = sceneMap.GetAt(point);
            if(grid != null)
            {
                CreateGridAt(point.x.value, point.y.value, grid);
                if(grid.obj != null)
                    CreateObjAt(point.x.value, point.y.value, grid.obj);
            }
        } while (iter.MoveToNext());
    }

    private void UpdateMap()
    {
        Maze.Iterator iter = new Maze.Iterator(player.posit, 8);
        do
        {
            Maze.Point2D point = iter.Iter;

            Maze.Grid grid = sceneMap.GetAt(point);
            if (grid != null)
            {
                switch (grid.objEvent)
                {
                    case "move":
                        playerBinded.transform.Translate(ConvertTo(player.vect));
                        camera.transform.Translate(ConvertTo(player.vect));
                        break;

                    case "turnTo":
                        playerBinded.GetComponent<SpriteRenderer>().sprite = player.Shape();
                        break;

                    default:
                        break;
                }
                grid.objEvent = null;
            }
        } while (iter.MoveToNext());
    }
    */
    private void PlayerMove(Maze.Vector2D vector)
    {
        player.MoveFor(vector);
        manager.moveForward(vector);
        playerBinded.transform.Translate(ConvertTo(player.vect));
        camera.transform.Translate(ConvertTo(player.vect));

    }

    private Vector2 ConvertTo(Maze.Vector2D vector)
    {
        switch (vector)
        {
            case Maze.Vector2D.Up:
                return Vector2.up;
            case Maze.Vector2D.Down:
                return Vector2.down;
            case Maze.Vector2D.Left:
                return Vector2.left;
            case Maze.Vector2D.Right:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }

    private void MoveForward(GameObject gameObject, Vector2 vector)
    {
        gameObject.transform.Translate(vector);
    }
}
