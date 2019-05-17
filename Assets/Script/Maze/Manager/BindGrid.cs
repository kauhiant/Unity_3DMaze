using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    public class BindGrid
    {
        public Grid grid;
        public GameObject binded;
        

        public BindGrid(Grid grid, int x, int y)
        {
            binded = new GameObject("Grid");
            binded.transform.position = new Vector2(x, y);
            binded.AddComponent<SpriteRenderer>().sprite = grid.Sprite;
            binded.GetComponent<SpriteRenderer>().sortingLayerName = "grid";
        }

        public void Destroy()
        {
            GameObject.Destroy(binded);
        }
    }
}
