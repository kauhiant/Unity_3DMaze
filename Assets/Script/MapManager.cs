using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {


    public GameObject grid;
    public GameObject stone;
    public GameObject camera;

    private Maze.Map3D gameMap;
    private Maze.Map2D sceneMap;

	// Use this for initialization
	void Start () {

        GlobalAsset.gridSprite = grid.GetComponent<SpriteRenderer>().sprite;
        GlobalAsset.stoneSprite = stone.GetComponent<SpriteRenderer>().sprite;

        gameMap = new Maze.Map3D(8, 8, 3);
        sceneMap = new Maze.Map2D(gameMap);

	}

    // Update is called once per frame
    int i=0;
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ShowMap();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            camera.transform.Translate(Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            camera.transform.Translate(Vector3.down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            camera.transform.Translate(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            camera.transform.Translate(Vector3.right);
        }
    }

    /*
    public void CreateObjAt(int x, int y, GameObject gameObject) {
        Instantiate(gameObject, new Vector2(x, y), gameObject.transform.rotation);
    }*/

    public void CreateObjAt(int x, int y, Maze.MazeObject obj)
    {
        GameObject temp = new GameObject();
        temp.transform.position = new Vector3(x, y, 0);
        temp.AddComponent<SpriteRenderer>();
        temp.GetComponent<SpriteRenderer>().sprite = obj.Shape();
        temp.GetComponent<SpriteRenderer>().sortingLayerName = "object";


        Console.WriteLine("create obj");
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
                CreateGridAt(i, j, grid);
                if(grid.obj != null)
                    CreateObjAt(i, j, grid.obj);

                point.MoveFor(Maze.Vector2D.Right,1);
            }
            point.MoveFor(Maze.Vector2D.Up, 1);
            point.MoveFor(Maze.Vector2D.Left, 8);
        }
    }
}
