//using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

public class BacktrackingAlgorithm : AlgorithmBase
{
    private Stack<BaseCell> m_cellStack;

    public BacktrackingAlgorithm(MazeGenerator mazeGenerator) : base(mazeGenerator)
    {

    }

    //創建資料主程式
    public override void Update()
    {
        //將模式設為創建中
        m_isGenerating = true;

        m_cellStack = new Stack<BaseCell>();

        //將新創建物件加入至堆疊 並呼叫最初始的 BaseCell
        m_cellStack.Push(new BaseCell(0, 0, m_mazeGenerator));

        BaseCell cellCache = null;
        int directionCache = 0;

        //執行到迷宮創建完成
        while (m_cellStack.Count != 0)
        {
            //取得堆疊最上面的 BaseCell
            cellCache = m_cellStack.Peek();

            //取得 cellCache 的隨機牆壁方向
            directionCache = cellCache.GetRandomDirection();
            
            //判斷 cellCache 的牆壁所有方向是不是不空的
            if (directionCache != 0)
            {
                m_cellStack.Push(new BaseCell(cellCache, directionCache, m_mazeGenerator));
            }
            else
            {
                m_cellStack.Pop();
            }
        }

        //迷宮創建結束
        m_isGenerating = false;
        
        //Debug
        //m_mazeGenerator.maze.TestMaze();
    }
}