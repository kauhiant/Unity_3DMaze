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

    private List<GameObject> grids = new List<GameObject>();
    private List<GameObject> objs = new List<GameObject>();

	// Use this for initialization
	void Start () {

        GlobalAsset.gridSprite = grid.GetComponent<SpriteRenderer>().sprite;
        GlobalAsset.stoneSprite = stone.GetComponent<SpriteRenderer>().sprite;
        GlobalAsset.animalSprite = animal.GetComponent<SpriteRenderer>().sprite;

        gameMap = new Maze.Map3D(8, 8, 3);
        sceneMap = new Maze.Map2D(gameMap);
        GlobalAsset.map = gameMap;

        player = new Maze.Animal(new Maze.Point3D(0, 3, 0));

        gameMap.InsertAt(player.position, player);
        
	}

    // Update is called once per frame
    int i=0;
	void Update () {
        if (Input.GetKeyDown(KeyCode.S))
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
        else if (Input.GetKeyDown(KeyCode.X))
        {
            ChangePlain(Maze.Dimention.X);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            ChangePlain(Maze.Dimention.Y);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            ChangePlain(Maze.Dimention.Z);
        }
    }
    

    public void CreateObjAt(int x, int y, Maze.MazeObject obj)
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

    public void CreateGridAt(int x, int y, Maze.Grid grid)
    {
        GameObject temp = new GameObject();
        temp.transform.position = new Vector3(x,y,0);
        temp.AddComponent<SpriteRenderer>();
        temp.GetComponent<SpriteRenderer>().sprite = grid.shape;
        temp.GetComponent<SpriteRenderer>().sortingLayerName = "grid";
        grids.Add(temp);
    }

    public void ChangePlain(Maze.Dimention dimention)
    {
        player.ChangePlain(dimention);
        ClearMap();
        ShowMap();
    }

    public void ClearMap()
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

    public void ShowMap()
    {
        Maze.Iterator iter = new Maze.Iterator(player.posit, 5);

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
