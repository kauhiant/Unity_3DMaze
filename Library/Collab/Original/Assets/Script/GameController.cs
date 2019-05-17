using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Maze;

public class GameController : MonoBehaviour
{
    public GameObject mainCamera;

    public GameObject playerHintVector;
    public PositionHint positionHint;
    public Ability ability;

    public GameObject littleMap;
    
    public StatusBar statusBar;
    public GameObject UI_Skill;

    public RateBar2 rateBar;
    public HintBox hintBox;
    public TalkBox talkBox;

    public AudioSource clockAudio;
    public AudioSource punchAudio;
    public AudioSource otherSkillAudio;
    public AudioSource buildAudio;

    public float ClockTime
    {
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


    private float timer; // for clock.
    private Maze.Map3D gameMap;
    private Maze.Map2D sceneMap;
    private Maze.MapManager manager;
    private Story.StoryManager storyManager;

    private Maze.Animal Player
    {
        get { return GlobalAsset.player; }
        set { GlobalAsset.player = value; }
    }
    
    private bool GamePause
    {
        get { return Maze.GameStatus.pause; }
        set { Maze.GameStatus.pause = value; }
    }

    private bool Lose
    {
        get { return GameStatus.lose; }
        set { GameStatus.lose = value; }
    }

    private bool Win
    {
        get { return GameStatus.win; }
        set { GameStatus.win = value; }
    }


	// Use this for initialization.
	void Start () {

        // [遊戲素材設定]
        GlobalAsset.controller = this;

        // 物件造型.
        Maze.Animal.SetShape(new Maze.Shape(animalShapes));
        Maze.Stone.SetSprite(stoneSprite);
        Maze.Creater.SetSprite(createrSprite);
        Maze.Food.SetSprite(foodSprite);
        Maze.Wall.SetSprite(wallSprite);

        // 技能音效.
        Maze.SkillManager.attack = punchAudio;
        Maze.SkillManager.specialAttack = otherSkillAudio;
        Maze.SkillManager.build = buildAudio;

        // 技能造型.
        GlobalAsset.gridSprites = gridSprites;
        GlobalAsset.attack = attack;
        GlobalAsset.straight = straight;
        GlobalAsset.horizon = horizon;
        GlobalAsset.create = create;

        // 小地圖標示.
        GlobalAsset.playerMark = playerMark;
        GlobalAsset.createrMark = createrMark;

        // UI提示框.
        GlobalAsset.positionHint = positionHint;
        GlobalAsset.hintBox = hintBox;
        GlobalAsset.talkBox = talkBox;


        // [遊戲地圖設定]
        // 放幾個gridSprite就生成幾層世界.
        gameMap = new Maze.Map3D(64, 64, gridSprites.Length);
        sceneMap = new Maze.Map2D(gameMap);
        Maze.MazeObject.SetMaze(gameMap);

        // [劇情]
        storyManager = new Story.StoryManager();


        // 在每個平面執行動作.
        for (int layer=0; layer<Maze.MazeObject.World.Layers; ++layer)
        {
            // 創造6個不同顏色的村莊.
            for (int i = 0; i < 6; ++i)
            {
                Maze.Point3D position = gameMap.GetRandomPointOn(layer);
                Maze.Creater creater = new Maze.Creater(position, GlobalAsset.colors[i]);

                if (gameMap.GetAt(position).InsertObj(creater))
                {
                    GlobalAsset.creaters.Add(creater);
                    creater.FullEnergy();
                    // 在每個村莊生成3隻村民.
                    creater.createAnimals(3);
                }

                else // 這個位置已經有東西了.
                    --i;
            }
        }
        
        // clock 歸零.
        timer = 0;

    }
    

    // Update is called once per frame
	void FixedUpdate ()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GamePause = !GamePause;
        }


        if (manager != null)
            manager.Update(Time.deltaTime);

        if(!Lose)
            Clock(Time.deltaTime);
    }
    

    // 開頭故事結束，遊戲準備開始.
    public void GameReadyStart()
    {
        ShowTalkBox("按Enter建開始");
        Maze.GameStatus.ready = true;

        // 劇本.
        Story.StoryInit.Init1(storyManager);
    }

    // 開始遊戲，將現存的生物中的其中一隻變成玩家.
    // 並創造 MapManager 開始畫圖.
    public void GameStart()
    {
        AssignPlayer(GlobalAsset.LastestAnimal());
        manager = new Maze.MapManager(sceneMap, mainCamera, littleMap, GlobalAsset.seenRange);
        positionHint.ShowHintAt(manager.PlayerBind.binded.transform.position);

        statusBar.SetActive(true);
        UI_Skill.SetActive(true);
        rateBar.SetActive(true);
    }

    // 轉生.
    public void GameRestart()
    {
        Maze.Animal animal = GlobalAsset.LastestAnimalOnLayerColor(Player.position.Z.value, Player.Color);
        AssignPlayer(animal);
        manager.RebindPlayer();
        positionHint.ShowHintAt(manager.PlayerBind.binded.transform.position);
        UI_Skill.SetActive(true);
    }

    // 指定 animal 給玩家操作.
    private void AssignPlayer(Maze.Animal animal)
    {
        Player = animal;

        // for test 主角強化.
        Player.Strong(200, 10, 10);

        playerHintVector.SetActive(true);
    }

    // 跳轉道開始畫面.
    public void GotoStartScene()
    {
        GlobalAsset.Reset();
        GameStatus.Reset();
        SceneManager.LoadScene(0);
    }

    
    // FixedUpdate 觸發.
    private void Clock(float deltaTime)
    {
        
        // Clock 檢查.(鋸齒波邊緣觸發)
        timer += deltaTime;

        if(timer > ClockTime/2)
            // 將場上的技能效果清空.
            Maze.SkillManager.clear();

        if (timer < ClockTime) return;
        timer = 0;


        GameStatus.Clock();
        storyManager.Clock();

        if (GamePause)
            return;
        
        MazeClock();

        if(manager != null)
        {
            manager.Clock();
            UpdataUI();
        }
    }

    // MazeObjects 的 Clock.
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


            if (each != Player)
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

        statusBar.UpdateFor(GlobalAsset.player);
        ability.UpdateByAnimal(GlobalAsset.player);
        rateBar.SetBar(GlobalAsset.PowerOfColorOn(Player.position.Z.value));
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
