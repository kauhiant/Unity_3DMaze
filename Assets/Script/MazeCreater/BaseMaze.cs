using System.Collections.Generic;

public class BaseMaze
{
    public int m_width;
    public int m_height;

    public BaseCell[,] m_maze;

    //測試用 GameObject
    //public GameObject A_Image, B_Image;

    //接收 MazeGenerator 的訊息
    public void InitializeMaze(int width, int height)
    {
        //清空 m_maze
        DestroyMaze();

        m_width = width;
        m_height = height;
        m_maze = new BaseCell[m_width, m_height];
    }

    //回傳 m_maze[width, height] 物件
    public BaseCell GetCell(int width, int height)
    {
        if (width < 0 || width >= m_width ||
            height < 0 || height >= m_height)
            return null;

        return m_maze[width, height];
    }

    //將 BaseCell 資料放入 m_maze[]
    public void SetCell(int width, int height, BaseCell cell)
    {
        //若是給的直超過迷宮大小返回
        if (width < 0 || width >= m_width ||
            height < 0 || height >= m_height)
            return;

        //將當前的BaseCell存起來
        m_maze[width, height] = cell;
    }

    //清空 m_maze
    private void DestroyMaze()
    {
        if (null == m_maze)
            return;

        m_maze = null;
    }

    //判斷 m_maze[width, height] 有無物件
    public bool HasCell(int width, int height)
    {
        return null != GetCell(width, height);
    }

    //測試用函數
    /*public void TestMaze()
    {
        A_Image = GameObject.Find("Cube2").gameObject;
        B_Image = GameObject.Find("Cube").gameObject;

        int x = m_width - 1, y = m_height - 1;

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                GameObject[] t = new GameObject[4];

                t[0] = Instantiate(A_Image);
                t[0].transform.position = new Vector3(j * 2, i * 2, 0);

                if (m_maze[j, i].m_walls[1])
                    t[1] = Instantiate(A_Image);
                else
                    t[1] = Instantiate(B_Image);

                t[1].transform.position = new Vector3(j * 2, i * 2 + 1, 0);

                if (m_maze[j, i].m_walls[4])
                    t[2] = Instantiate(A_Image);
                else
                    t[2] = Instantiate(B_Image);

                t[2].transform.position = new Vector3(j * 2 + 1, i * 2, 0);

                t[3] = Instantiate(B_Image);
                t[3].transform.position = new Vector3(j * 2 + 1, i * 2 + 1, 0);
            }
        }

        for (int i = 0; i < x; i++)
        {
            GameObject[] t = new GameObject[2];

            t[0] = Instantiate(A_Image);
            t[0].transform.position = new Vector3(i * 2, y * 2, 0);

            if (m_maze[i, y].m_walls[4])
                t[1] = Instantiate(A_Image);
            else
                t[1] = Instantiate(B_Image);

            t[1].transform.position = new Vector3(i * 2 + 1, y * 2, 0);
        }

        for (int i = 0; i < y; i++)
        {
            GameObject[] t = new GameObject[2];

            t[0] = Instantiate(A_Image);
            t[0].transform.position = new Vector3(x * 2, i * 2, 0);

            if (m_maze[x,i].m_walls[1])
                t[1] = Instantiate(A_Image);
            else
                t[1] = Instantiate(B_Image);

            t[1].transform.position = new Vector3(x * 2, i * 2 + 1, 0);
        }

        GameObject final_t = new GameObject();

        final_t = Instantiate(A_Image);
        final_t.transform.position = new Vector3(x * 2, y * 2, 0);
    }*/

    public List<int> All_Coordinate()
    {
        List<int> coordinate = new List<int>();

        int x = m_width - 1, y = m_height - 1;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                if (m_maze[i, j].m_walls[1])
                {
                    coordinate.Add(i * 2);
                    coordinate.Add(j * 2 + 1);
                }

                if (m_maze[i, j].m_walls[4])
                {
                    coordinate.Add(i * 2 + 1);
                    coordinate.Add(j * 2 );
                }

                coordinate.Add(i * 2 + 1);
                coordinate.Add(j * 2 + 1);
            }
        }

        return coordinate;
    }
}