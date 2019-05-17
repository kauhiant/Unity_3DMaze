using UnityEngine;

public class MazeGenerator
{
    public BaseMaze maze;
    public int width;
    public int height;
    private AlgorithmBase algorithm;

    public void Start_CreatMaze(int Map_width, int Map_height)
    {
        width = Map_width;
        height = Map_height;

        maze = new BaseMaze();

        //將演算法指定為BacktrackingAlgorithm
        algorithm = new BacktrackingAlgorithm(this);

        //初始創建迷宮
        GenerateMaze();
    }
    
    //給予 BaseMaze 迷宮大小
    private void InitializeMaze()
    {
        maze.InitializeMaze(width, height);
    }


    private bool IsGenerating()
    {
        if (null != algorithm && algorithm.IsGenerating)
        {
            return true;
        }

        return false;
    }


    public void GenerateMaze()
    {
        //如果現在還在創建中 返回
        if (IsGenerating())
        {
            return;
        }

        //創建基礎迷宮大小
        InitializeMaze();

        //正式創建主程式碼
        algorithm.Update();
    }
}