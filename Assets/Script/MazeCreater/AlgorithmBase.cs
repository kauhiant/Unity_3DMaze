//using System.Collections;

public class AlgorithmBase
{
    //迷宮是否在創建?
    public bool IsGenerating
    {
        get { return m_isGenerating; }
    }

    protected MazeGenerator m_mazeGenerator;
    protected bool m_isGenerating = false;


    //取得MazeGenerator
    public AlgorithmBase(MazeGenerator mazeGenerator)
    {
        m_mazeGenerator = mazeGenerator;
    }

    //繼承用基底
    public virtual void Update()
    {
        
    }
}