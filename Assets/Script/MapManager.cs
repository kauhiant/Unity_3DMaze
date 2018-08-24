using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public GameObject camera;
    public GameObject playerHintVector;
    public GameObject littleMap;
    public GameObject UI_statusBars;
    public GameObject UI_HP;
    public GameObject UI_EP;
    public GameObject UI_Hungry;
    public GameObject UI_Skill;

    public RateBar2 rateBar;
    public HintBox hintBox;
    public TalkBox talkBox;

    public AudioSource clockAudio;
    public AudioSource punchAudio;
    public AudioSource otherSkillAudio;

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

    public Sprite[] gridSprites;
    public Sprite stoneSprite;
    public Sprite createrSprite;
    public Sprite foodSprite;
    public Sprite wallSprite;
    

    private Maze.Map3D gameMap;
    private Maze.Map2D sceneMap;
    private Maze.Animal player;
    private Maze.MapManager manager;
    
    private bool isWin  = false;



	// Use this for initialization.
	void Start () {

        Maze.Animal.SetShape(new Maze.Shape(animalShapes));
        Maze.Stone.SetSprite(stoneSprite);
        Maze.Creater.SetSprite(createrSprite);
        Maze.Food.SetSprite(foodSprite);
        Maze.Wall.SetSprite(wallSprite);

        GlobalAsset.gridSprites = gridSprites;
        GlobalAsset.attack = attack;
        GlobalAsset.straight = straight;
        GlobalAsset.horizon = horizon;
        GlobalAsset.create = create;

        GlobalAsset.playerMark = playerMark;
        GlobalAsset.createrMark = createrMark;

        GlobalAsset.attackAudio = punchAudio;
        GlobalAsset.otherAttackAudio = otherSkillAudio;

        // 放幾個gridSprite就生成幾層世界.
        gameMap = new Maze.Map3D(32, 32, gridSprites.Length);
        sceneMap = new Maze.Map2D(gameMap);
        Maze.MazeObject.SetMaze(gameMap);

        
        // 在每個平面執行動作.
        for (int layer=0; layer<Maze.MazeObject.World.Layers; ++layer)
        {
            // 創造6個不同顏色的村莊.
            for (int i = 0; i < 6; ++i)
            {
                Maze.Point3D position = gameMap.GetRandomPointOn(layer);
                Maze.Creater creater = new Maze.Creater(position, GlobalAsset.colors[i]);

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
	void Update ()
    {
        if (GameReady && Input.GetKeyDown(KeyCode.Return))
        {
            if (manager == null)
                GameStart();
            else if (player.IsDead)
                GameRestart();
            else if (isWin)
                gotoStartScene();
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
        AssignPlayer(GlobalAsset.lastestAnimal());
        manager = new Maze.MapManager(sceneMap, camera, littleMap, 8);

        UI_statusBars.SetActive(true);
        UI_Skill.SetActive(true);
        rateBar.SetActive(true);
    }

    // 轉生.
    private void GameRestart()
    {
        HideTalkBox();

        Maze.Animal animal = GlobalAsset.lastestAnimalOnLayerColor(player.position.Z.value, player.Color);
        if (animal == null)
            animal = GlobalAsset.lastestAnimalOnLayer(player.position.Z.value);
        if (animal == null)
            animal = GlobalAsset.lastestAnimal();

        AssignPlayer(animal);
        manager.ChangePlayer();
        UI_Skill.SetActive(true);
    }

    // 指定 animal 給玩家操作.
    private void AssignPlayer(Maze.Animal animal)
    {
        player = animal;
        //player.Strong(10000,100);
        GlobalAsset.player = player;
        playerHintVector.SetActive(true);
    }

    private void gotoStartScene()
    {
        GlobalAsset.Reset();
        SceneManager.LoadScene(0);
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

        if(timer > ClockTime/2)
            // 將場上的技能效果清空.
            Maze.SkillManager.clear();

        if (timer < ClockTime) return;
        timer = 0;

        if(player != null)
        {
            if (player.IsDead)
            {
                ShowTalkBox("你已經死了\n按Enter鍵轉生");
                playerHintVector.SetActive(false);
            }
            
            else if (GlobalAsset.RateOfColorOn(player.Color, player.position.Z.value) == 1f)
            {
                ShowTalkBox("我方勝利\n按Enter鍵回主選單");
                isWin = true;
            }
            else if (isWin && GlobalAsset.RateOfColorOn(player.Color, player.position.Z.value) != 1f)
            {
                HideTalkBox();
                isWin = false;
            }

            else
                clockAudio.Play();
        }


        
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

            if (each.IsDead)
            {
                GlobalAsset.animals.RemoveAt(i);
                --i;
                continue;
            }


            if (each != player)
            {
                each.AutoSurvey();
            }
                
            each.Action();
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
    
    // UI 顯示玩家狀態.
    private void UpdataUI()
    {
        if (GlobalAsset.player == null)
            return;

        UI_HP.GetComponent<Slider>().value = GlobalAsset.player.hp.BarRate;
        UI_EP.GetComponent<Slider>().value = GlobalAsset.player.ep.BarRate;
        UI_Hungry.GetComponent<Slider>().value = GlobalAsset.player.hungry.BarRate;

        rateBar.SetBar(GlobalAsset.PowerOfColorOn(player.position.Z.value));
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

    
}
