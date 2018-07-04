using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public GameObject grid;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    int i=0;
	void Update () {
        if (Input.GetKeyDown(KeyCode.A)) {
            CreateObjAt(i, i);
            ++i;
        }
	}

    public void CreateObjAt(int x, int y) {
        Instantiate(grid, new Vector3(i, i, 0), transform.rotation);
    }
}
