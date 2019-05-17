using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionHint : MonoBehaviour {

    public float maxAliveTime = 1f;
    public float initScale = 10f;

    private GameObject self { get {return this.gameObject; } }
    private float aliveTime = 0f;
    

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (self.activeSelf == false)
            return;

        aliveTime += Time.deltaTime;
        if (aliveTime >= maxAliveTime)
        {
            self.SetActive(false);
            return;
        }

        float scale = (maxAliveTime - aliveTime) / maxAliveTime * (initScale - 2) + 2;
        self.transform.localScale = new Vector2(scale, scale);
	}

    public void ShowHintAt(Vector2 position) {
        self.transform.position = position;
        self.transform.localScale = new Vector2(initScale, initScale);
        this.aliveTime = 0f;
        this.gameObject.SetActive(true);
    }

    public void ShowHintAt(Maze.Point2D position)
    {
        ShowHintAt(Maze.VectorConvert.Convert(position));
    }
}
