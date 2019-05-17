using System.Collections.Generic;

public class BaseCell
{
    // m_x , m_y 為 BaseCell 位置 (可考慮用 Vector 做 , Vector 本身較為耗效能 但可讀性更高
    // 及因程式碼中有 傳位置資料 Vector 傳的更為快速)
    
    // m_maze 指定為 MazeGenerator 的 BaseMaze

    // m_walls[] 為周邊牆壁 m_walls[0~4] 分別為 空上下左右
    // m_walls[] , False = 牆壁開啟 , True = 牆壁關閉

    // m_neighbours 為周邊牆壁狀態 m_neighbours[0~4] 分別為 空上下左右
    // m_neighbours[] , 0 = 牆壁(不會更動) , 1 = 未確定 , 2 = 空

    public int m_x, m_y;
    public BaseMaze m_maze;
    public bool[] m_walls;
    public byte[] m_neighbours;

    //初始 BaseCell 只會執行一次
    public BaseCell(int w, int h, MazeGenerator mazeGenerator)
    {
        //設定 BaseCell 位置 初始預設為 [0, 0]
        m_x = w;
        m_y = h;

        //將 m_maze 的資料指向 MazeGenerator 的 BaseMaze
        m_maze = mazeGenerator.maze;

        //向 m_maze 傳當前 BaseCell 的資料
        m_maze.SetCell(w, h, this);

        //設定牆壁 初始預設為空
        SetupWalls(0);

        //設定牆壁狀態
        SetupNeighbours();
    }

    //新增 BaseCell 
    public BaseCell(BaseCell previousCell, int direction, MazeGenerator mazeGenerator)
    {
        //取得前一個 BaseCell 的位置
        m_x = previousCell.m_x;
        m_y = previousCell.m_y;

        //判斷方向並更改當前 BaseCell 的位置
        switch (direction)
        {
            case 1:
                m_y++;
                break;
            case 2:
                m_y--;
                break;
            case 3:
                m_x--;
                break;
            case 4:
                m_x++;
                break;
        }

        //將 m_maze 的資料指向 MazeGenerator 的 BaseMaze
        m_maze = mazeGenerator.maze;

        //向 m_maze 傳當前 BaseCell 的資料
        m_maze.SetCell(m_x, m_y, this);

        //設定牆壁給予參數 方向
        SetupWalls(direction);

        //設定牆壁狀態
        SetupNeighbours();
    }

    //設定 m_walls[] 的牆壁
    private void SetupWalls(int initialDirection)
    {
        //宣告各方向
        m_walls = new bool[5];

        //initialDirection 的反向變數
        int OppostieDirection;

        //取得 initialDirection 的反向
        switch (initialDirection)
        {
            case 1:
                OppostieDirection = 2;
                break;
            case 2:
                OppostieDirection = 1;
                break;
            case 3:
                OppostieDirection = 4;
                break;
            case 4:
                OppostieDirection = 3;
                break;
            default:
                OppostieDirection = 0;
                break;
        }

        //關掉反向的牆壁
        if (initialDirection != 0)
        {
            for (int i = 1; i < 5; i++)
            {
                if (i == OppostieDirection)
                    DisableWalls(i);
            }
        }
    }

    //關閉牆壁
    public void DisableWalls(int direction)
    {
        //設定牆壁為關閉
        m_walls[direction] = true;
    }

    //設定牆壁狀態 (考慮 byte 預設值 0 考慮 0 = 空? 但用預設感覺有點危險(bool 就算了ㄏㄏ))
    public void SetupNeighbours()
    {
        //宣告狀態
        m_neighbours = new byte[5];

        //將狀態預設為  2 = 空
        for (int i = 1; i < 5; i++)
        {
            m_neighbours[i] = 2;
        }

        //若 BaseCell 周邊的牆壁為最外圍 則將狀態設為 0 = 牆壁
        if (m_y + 1 == m_maze.m_height)
            m_neighbours[1] = 0;
        if (m_y  < 1)
            m_neighbours[2] = 0;
        if (m_x < 1)
            m_neighbours[3] = 0;
        if (m_x + 1 == m_maze.m_width)
            m_neighbours[4] = 0;

        //更新牆壁狀態
        UpdateNeighbours();
    }

    //更新牆壁狀態
    public void UpdateNeighbours()
    {
        int x, y;

        //遍歷各牆壁
        for (int i = 1; i < 5; i++)
        {
            switch (i)
            {
                case 1:
                    x = m_x;
                    y = m_y + 1;
                    break;
                case 2:
                    x = m_x;
                    y = m_y - 1;
                    break;
                case 3:
                    x = m_x - 1;
                    y = m_y;
                    break;
                case 4:
                    x = m_x + 1;
                    y = m_y;
                    break;
                default:
                    x = m_x;
                    y = m_y;
                    break;
            }

            //若牆壁位置在外圍則繼續下一牆壁位置
            if (x < 0 || x == m_maze.m_width ||
                y < 0 || y == m_maze.m_height)
                continue;

            //如果牆壁位置有物件則將狀態設為 1 = 未確定
            if (m_maze.HasCell(x, y))
                m_neighbours[i] = 1;
        }
    }

    //取得隨機牆壁方向
    public int GetRandomDirection()
    {
        //更新牆壁狀態
        UpdateNeighbours();

        List<int> directions = new List<int>();
        bool noNeighbour = true;

        //將牆壁狀態為 2 = 空 的牆壁加進 List
        for (int i = 1; i < 5; i++)
        {
            if (m_neighbours[i] == 2)
            {
                noNeighbour = false;
                directions.Add(i);
            }
        }

        if (noNeighbour)
            return 0;

        int randomNum = UnityEngine.Random.Range(0, directions.Count);

        DisableWalls(directions[randomNum]);

        return directions[randomNum];
    }
}