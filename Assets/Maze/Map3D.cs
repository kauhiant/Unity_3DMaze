using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    class Map3D
    {
        private Grid[][][] map;

        public Map3D (int widthX, int widthY, int layers)
        {

            map = new Grid[widthX][][];
            for(int i=0; i<widthX; ++i)
            {
                map[i] = new Grid[widthY][];
                for(int j=0; j<widthY; ++j)
                {
                    map[i][j] = new Grid[layers];
                    for(int k=0; k<layers; ++k)
                    {
                        map[i][j][k] = new Grid();
                        map[i][j][k].shape = GlobalAsset.gridSprite;
                    }
                }
            }

            InitMap();
        }



        public bool IsInThisMap(Point3D position)
        {
            return (position.x.value < map.Length &&
                position.y.value < map[0].Length &&
                position.z.value < map[0][0].Length);
        }

        public Grid GetAt(Point3D position)
        {
            if (IsInThisMap(position))
                return map[position.x.value][position.y.value][position.z.value];

            else
                return null;
        }



        public bool InsertAt(Point3D position, MazeObject obj)
        {
            if (!IsInThisMap(position))
                return false;

            if (GetAt(position) != null)
                return false;

            GetAt(position).obj = obj;
            return true;
        }

        public void RemoveAt(Point3D position)
        {
            if (IsInThisMap(position))
                GetAt(position).obj = null;
        }

        public void SetBackgroundAt(Point3D position, UnityEngine.Sprite shape)
        {
            if (IsInThisMap(position))
                GetAt(position).shape = shape;
        }

        private void InitMap()
        {
            for(int i=0; i<map.Length; ++i)
            {
                for(int j=0; j<map[0].Length; ++j)
                {
                    for(int k=0; k<map[0][0].Length; ++k)
                    {
                        if(Random.Range(0f,1f) < 0.4f)
                            map[i][j][k].obj = new Stone(new Point3D(i,j,k));
                    }
                }
            }
        }
    }
}

