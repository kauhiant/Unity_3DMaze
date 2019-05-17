using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Draw;

public class GraphicTest : MonoBehaviour
{
    struct Pair
    {
        int x;
        int y;
    }

    public bool isPause;
    public Image image;
    public Color backGround;

    private MyCanvas map;
    private int width;
    private int height;

    private float grid_width = 1;
    private float grid_height = 1;

    private void Start()
    {
        width = (int)(image.rectTransform.rect.width);
        height = (int)(image.rectTransform.rect.height);
        map = new MyCanvas(width, height, backGround);
        image.sprite = map.GetSprite();

        int size = GlobalAsset.mapSize;
        this.Griding(size, size);

        StartCoroutine(Test());

        GlobalAsset.smallMap = this;
    }

    public void Griding(int x_num, int y_num)
    {
        grid_width = width / (float)(x_num + 1);
        grid_height = height / (float)(y_num + 1);
    }

    public void Clear()
    {
        map.Clear();
    }

    public GraphicTest DrawGridAt(int x,int y, Color color, int size=1, bool border=false)
    {
        int xOfGrid = (int) ((1 + x) * grid_width);
        int yOfGrid = (int)((1 + y) * grid_height);
        int wOfGrid = (int)(grid_width * size);
        int hOfGrid = (int)(grid_height * size);

        if(border)
            map.DrawGridAt(xOfGrid, yOfGrid, wOfGrid, hOfGrid, color, 1,Color.black);
        else
            map.DrawGridAt(xOfGrid, yOfGrid, wOfGrid, hOfGrid, color,1);

        return this;
    }

    public void UpdateGraphic()
    {
        this.image.sprite = this.map.GetSprite();
    }

    IEnumerator Test()
    {
        int size = GlobalAsset.mapSize;
        Color[] colors = { Color.blue, Color.cyan, Color.gray, Color.green, Color.grey, Color.magenta, Color.red, Color.yellow };

        this.Griding(size, size);
        this.map.Clear();
        this.UpdateGraphic();
        yield return new WaitForSeconds(1);

        for(int i = 0; ; ++i)
        {
            if (isPause)
            {
                yield return new WaitForSeconds(GlobalAsset.clockTime);
                continue;
            }


            if (i == size)
            {
                i = 0;
                this.UpdateGraphic();
                this.Clear();
                yield return new WaitForSeconds(GlobalAsset.clockTime);
            }

            


            int x = Random.Range(0, size);
            int y = Random.Range(0, size);
            int s = Random.Range(1, 11);
            int c = Random.Range(0, colors.Length);
            this.DrawGridAt(x, y, colors[c], s);
        }

    }


    


}
