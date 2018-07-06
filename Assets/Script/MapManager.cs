using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {


    public GameObject grid;
    private Maze.Map3D gameMap;
    private Maze.Map2D sceneMap;

	// Use this for initialization
	void Start () {
        gameMap = new Maze.Map3D(8, 8, 3);
        sceneMap = new Maze.Map2D(gameMap);
	}

    // Update is called once per frame
    int i=0;
	void Update () {
        if (Input.GetKeyDown(KeyCode.A)) {
            ShowMap();
        }
	}

    public void CreateObjAt(int x, int y, GameObject gameObject) {
        Instantiate(gameObject, new Vector2(x, y), gameObject.transform.rotation);
    }

    public void ShowMap()
    {
        for(int i=0; i<8; ++i)
        {
            for(int j=0; j<8; ++j)
            {
                if(gameMap.GetAt(new Maze.Point3D(i,j,0)).obj != null)
                    CreateObjAt(i, j, grid);
            }
        }
    }
}
