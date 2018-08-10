using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public GameObject camera;
    public GameObject littleMap;
    public GameObject UI_statusBars;
    public GameObject UI_HP;
    public GameObject UI_EP;
    public GameObject UI_Hungry;
    public GameObject UI_Skill;

    public HintBox hintBox;
    public TalkBox talkBox;

    public AudioSource clockAudio;

    public float ClockTime
    {
        set
        {
            GlobalAsset.clockTime = value;
        }
        get
        {
            return GlobalAsset.clockTime;
        }
    }

    /// <summary>
    /// index : right, down, left, up, in, out.
    /// </summary>
    public Sprite[] animalShapes = new Sprite[6];

    public Sprite attack;
    public Sprite straight;
    public Sprite horizon;
    public Sprite create;

    public Sprite playerMark;
    public Sprite createrMark;

    public Sprite gridSprite;
    public Sprite stoneSprite;
    public Sprite createrSprite;
    public Sprite foodSprite;
    public Sprite wallSprite;
    

    private Maze.Map3D gameMap;
    private Maze.Map2D sceneMap;
    private Maze.Animal player;
    private Maze.MapManager manager;

    private bool isAuto = false;
    private Command command = Command.None;



	// Use this for initialization.
	void Start () {

        Maze.Animal.SetShape(new Maze.Shape(animalShapes));
        Maze.Grid.SetSprite(gridSprite);
        Maze.Stone.SetSprite(stoneSprite);
        Maze.Creater.SetSprite(createrSprite);
        Maze.Food.SetSprite(foodSprite);
        Maze.Wall.SetSprite(wallSprite);

        GlobalAsset.attack = attack;
        GlobalAsset.straight = straight;
        GlobalAsset.horizon = horizon;
        GlobalAsset.create = create;

        GlobalAsset.playerMark = playerMark;
        GlobalAsset.createrMark = createrMark;


        gameMap = new Maze.Map3D(64, 64, 3);
        sceneMap = new Maze.Map2D(gameMap);
        Maze.MazeObject.SetMaze(gameMap);

        
        // 在每個平面執行動作.
        for (int layer=0; layer<Maze.MazeObject.World.Layers; ++layer)
        {
            // 創造6個不同顏色的村莊.
            for (int i = 0; i < 6; ++i)
            {
                Maze.Point3D position = gameMap.GetRandomPointOn(layer);
                Maze.Creater creater = new Maze.Creater(position, colorIndex(i));

                if (gameMap.GetAt(position).InsertObj(creater))
                    GlobalAsset.creaters.Add(creater);

                else
                    --i;
            }

            // 創造小於200的各色生物.
            for (int i = 0; i < 200; ++i)
            {
                Maze.Point3D point = gameMap.GetRandomPointOn(layer);
                Maze.Animal animal = new Maze.Animal(point, GlobalAsset.creaters[i % GlobalAsset.creaters.Count]);
                if (gameMap.GetAt(point).InsertObj(animal))
                    GlobalAsset.animals.Add(animal);
            }
        }
        
        
        timer = 0;
    }

    // Update is called once per frame
	void FixedUpdate ()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            command = Command.Up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            command = Command.Down;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            command = Command.Left;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            command = Command.Right;
        }
        else if (Input.GetKey(KeyCode.T))
        {
            command = Command.Plain;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            command = Command.Attack;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            command = Command.Straight;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            command = Command.Horizon;
        }
        else if (Input.GetKey(KeyCode.R))
        {
            command = Command.Wall;
        }

        if (GameReady && Input.GetKeyDown(KeyCode.Return))
        {
            if (manager == null)
                GameStart();
            else if (player.isDead)
                GameRestart();
        }

        if (manager != null)
            manager.Update(Time.deltaTime);

        Clock(Time.deltaTime);
    }
    

    // 開始遊戲，將現存的生物中的其中一隻變成玩家.
    // 並創造 MapManager 開始畫圖.
    private void GameStart()
    {
        HideTalkBox();
        player = GlobalAsset.animals[GlobalAsset.animals.Count-1];
        player.Strong(100);
        GlobalAsset.player = player;
        manager = new Maze.MapManager(sceneMap, camera, littleMap, 8);

        UI_statusBars.SetActive(true);
        UI_Skill.SetActive(true);
    }

    private void GameRestart()
    {
        HideTalkBox();
        player = GlobalAsset.animals[GlobalAsset.animals.Count - 1];
        player.Strong(100);
        GlobalAsset.player = player;
        manager.ChangePlain();
        command = Command.None;
    }


    private bool GameReady = false;
    public void GameReadyStart()
    {
        ShowTalkBox("按Enter建開始");
        GameReady = true;
    }

    float timer;
    public void Clock(float deltaTime)
    {
        
        // Clock 檢查.(鋸齒波邊緣觸發)
        timer += deltaTime;
        if (timer < ClockTime) return;
        timer = 0;
        

        if(player != null)
        {
            if (player.isDead)
                ShowTalkBox("你已經死了\n按Enter鍵轉生");
            
            else if (GlobalAsset.RateOfColorOn(player.Color, player.position.Z.value) == 1f)
                ShowTalkBox("我方勝利");

            else
                clockAudio.Play();
        }

        // 將場上的技能效果清空.
        Maze.SkillManager.clear();

        
        MazeClock();

        if(manager != null)
        {
            manager.Clock();
            UpdataUI();
        }
    }

    private void MazeClock()
    {
        // 地圖 行動.
        gameMap.Clock();

        // 所有生物 行動.
        for (int i = 0; i < GlobalAsset.animals.Count; ++i)
        {
            Maze.Animal each = GlobalAsset.animals[i];

            if (each.isDead)
            {
                GlobalAsset.animals.RemoveAt(i);
                --i;
                continue;
            }


            if (each == player)
            {
                if (isAuto)
                    each.Auto();
                else
                    PlayerAction();
            }
            else
                each.Auto();


            each.Clock();
        }

        // 所有生成器 行動.
        for (int i = 0; i < GlobalAsset.creaters.Count; ++i)
        {
            if (GlobalAsset.creaters[i].IsDead)
            {
                GlobalAsset.creaters.RemoveAt(i);
                --i;
                continue;
            }
            GlobalAsset.creaters[i].Clock();
        }

    }

    

    // 目前有6個顏色.
    private Color colorIndex(int index)
    {
        switch (index)
        {
            case 0:
                return Color.white;
            case 1:
                return Color.red;
            case 2:
                return Color.green;
            case 3:
                return Color.blue;
            case 4:
                return Color.cyan;
            case 5:
                return Color.yellow;
            default:
                return Color.magenta;
        }
    }
    
    // 切換 [自動/手動] 模式.
    private void SwitchAuto()
    {
        if (isAuto)
        {
            isAuto = !isAuto;
            command = Command.None;
        }
        else
        {
            isAuto = !isAuto;
        }
    }

    // player 根據 玩家command 行動.
    private void PlayerAction()
    {
        switch (command)
        {
            case Command.Up:
                player.MoveFor(Maze.Vector2D.Up);
                break;

            case Command.Down:
                player.MoveFor(Maze.Vector2D.Down);
                break;

            case Command.Left:
                player.MoveFor(Maze.Vector2D.Left);
                break;

            case Command.Right:
                player.MoveFor(Maze.Vector2D.Right);
                break;

            case Command.Plain:
                player.ChangePlain();
                manager.ChangePlain();
                break;

            case Command.Attack:
                player.Attack();
                break;

            case Command.Straight:
                player.Straight();
                break;

            case Command.Horizon:
                player.Horizon();
                break;

            case Command.Wall:
                player.Build();
                break;
        }
        command = Command.None;
    }

    // UI 顯示玩家狀態.
    private void UpdataUI()
    {
        UI_HP.GetComponent<Slider>().value = GlobalAsset.player.hp.BarRate;
        UI_EP.GetComponent<Slider>().value = GlobalAsset.player.ep.BarRate;
        UI_Hungry.GetComponent<Slider>().value = GlobalAsset.player.hungry.BarRate;
    }

    // UI顯示對話框.
    private void ShowTalkBox(String message)
    {
        talkBox.Show(message);
        UI_Skill.SetActive(false);
    }

    // UI隱藏對話框.
    private void HideTalkBox()
    {
        talkBox.Hide();
    }

    // 玩家可下的指令.
    enum Command
    {
        Up,Down,Left,Right,Plain,Attack,Straight,Horizon,Wall,None
    }
}
