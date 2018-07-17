using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {


    public GameObject grid;
    public GameObject stone;
    public GameObject animal;
    public GameObject camera;

    private Maze.Map3D gameMap;
    private Maze.Map2D sceneMap;
    private Maze.Animal player;
    private GameObject playerBinded;

	// Use this for initialization
	void Start () {

        GlobalAsset.gridSprite = grid.GetComponent<SpriteRenderer>().sprite;
        GlobalAsset.stoneSprite = stone.GetComponent<SpriteRenderer>().sprite;
        GlobalAsset.animalSprite = animal.GetComponent<SpriteRenderer>().sprite;

        gameMap = new Maze.Map3D(8, 8, 1);
        sceneMap = new Maze.Map2D(gameMap);
        GlobalAsset.map = gameMap;

        player = new Maze.Animal(new Maze.Point3D(0, 0, 0));

        gameMap.InsertAt(player.position, player);
        
	}

    // Update is called once per frame
    int i=0;
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ShowMap();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            PlayerMove(Maze.Vector2D.Up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            PlayerMove(Maze.Vector2D.Down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PlayerMove(Maze.Vector2D.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            PlayerMove(Maze.Vector2D.Right);
        }
    }
    

    public void CreateObjAt(int x, int y, Maze.MazeObject obj)
    {
        GameObject temp = new GameObject();
        temp.transform.position = new Vector3(x, y, 0);
        temp.AddComponent<SpriteRenderer>();
        temp.GetComponent<SpriteRenderer>().sprite = obj.Shape();
        temp.GetComponent<SpriteRenderer>().sortingLayerName = "object";

        if(obj == player)
        {
            playerBinded = temp;
        }
    }

    public void CreateGridAt(int x, int y, Maze.Grid grid)
    {
        GameObject temp = new GameObject();
        temp.transform.position = new Vector3(x,y,0);
        temp.AddComponent<SpriteRenderer>();
        temp.GetComponent<SpriteRenderer>().sprite = grid.shape;
        temp.GetComponent<SpriteRenderer>().sortingLayerName = "grid";
    }

    public void ShowMap()
    {
        Maze.Point3D point3 = new Maze.Point3D(0, 0, 0);
        Maze.Point2D point = new Maze.Point2D(point3, Maze.Dimention.Z);

        for(int i=0; i<8; ++i)
        {
            for(int j=0; j<8; ++j)
            {
                Maze.Grid grid = sceneMap.GetAt(point);
                CreateGridAt(j, i, grid);
                if(grid.obj != null)
                    CreateObjAt(j, i, grid.obj);

                point.MoveFor(Maze.Vector2D.Right,1);
            }
            point.MoveFor(Maze.Vector2D.Up, 1);
            point.MoveFor(Maze.Vector2D.Left, 8);
        }
    }

    private void PlayerMove(Maze.Vector2D vector)
    {
        if (player.MoveFor(vector))
        {
            playerBinded.transform.Translate(ConvertTo(vector));
            camera.transform.Translate(ConvertTo(vector));
        }
            
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
}
