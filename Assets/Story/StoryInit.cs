using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Maze;

namespace Story
{
    public class StoryInit
    {
        private static TalkBox TalkBox { get { return GlobalAsset.talkBox; } }
        private static Animal Player { get { return GlobalAsset.player; } }
        private static bool teachMode = true;

        static public void Init1(StoryManager manager)
        {
            var ready = new State();
            var start = new State();
            var gaming = new State();
            var waitForRestart = new State();
            var waitForEnd = new State();
            var end = new State();

            ready.AddEvent(new Event(
                () => GameStatus.ready,
                start,
                new StoryPlayer().AddMessage("按Enter開始遊戲")
                ));

            start.AddEvent(new Event(
                () => TalkBox.end,
                gaming,
                new ActionPlayer(
                    () => {
                        GameStatus.pause = false;
                        GlobalAsset.controller.GameStart();
                    }
                    )
                ));

            gaming
                .AddEvent(new Event(
                    ()=>teachMode,
                    TeachMode(gaming),
                    new ActionPlayer(
                        () => { }
                    )
                ))
                .AddEvent(new Event(
                    () => GlobalAsset.player.IsDead && GlobalAsset.LastestAnimalOnLayerColor(Player.position.Z.value, Player.Color) == null,
                    waitForEnd,
                    new StoryPlayer().AddMessageWithAction(
                        "物種滅絕...你輸了\n按Enter鍵回主選單",
                        ()=> {
                            GameStatus.pause = true;
                            GameStatus.lose = true;
                        }
                    )
                ))
                .AddEvent(new Event(
                    () => GlobalAsset.player.IsDead && !GameStatus.lose,
                    waitForRestart,
                    new ActionPlayer(() => {
                        TalkBox.Show("你已經死了\n按Enter鍵轉生");
                    }
                    )
                ))
                .AddEvent(new Event(
                    () => GlobalAsset.RateOfColorOn(Player.Color,Player.position.Z.value) == 1f ,
                    waitForEnd,
                    new StoryPlayer().AddMessageWithAction(
                        "我方勝利\n按Enter鍵回主選單",
                        () => {
                            GameStatus.pause = true;
                            GameStatus.win = true;
                        }
                    )
                ));

            waitForRestart.AddEvent(new Event(
                () => TalkBox.end,
                gaming,
                new ActionPlayer(
                    () =>{
                        GameStatus.pause = false;
                        GlobalAsset.controller.GameRestart();
                    }
                    )));

            waitForEnd.AddEvent(new Event(
                () => TalkBox.end,
                end,
                new ActionPlayer(
                    () => GlobalAsset.controller.GotoStartScene()
                    )));
            

            manager.currentState = ready;
        }


        static private State TeachMode(State end)
        {
            State start = new State();

            start.AddEvent(new Event(
                () => true,
                Teach_Move(Teach_Food(Teach_Creater(Teach_Skill(end)))),
                new ActionPlayer(
                    () => teachMode = false
                    )
                ));

            return start;
        }

        static private State Teach_Move(State end)
        {
            State start = new State();
            State step0 = new State();
            State step1 = new State();

            start.AddEvent(new Event(
                () => true,
                step0,
                new StoryPlayer().AddMessage("你可以按鍵盤上下左右來移動\n也可移動滑鼠再按住左鍵來移動\n試試看吧")
            ));
            

            step0.AddEvent(new Event(
                () => GameStatus.moved,
                end,
                new StoryPlayer().AddMessage("很棒")
            ));

            return start;
        }

        static private State Teach_Food(State end)
        {
            var start = new State();
            var step0 = new State();

            start.AddEvent(new Event(
                () => TalkBox.end,
                step0,
                new StoryPlayer().AddMessage("地上會冒出一些黃色星星\n吃了它可以恢復能量增加飽食度\n去吃看看吧")
                ));

            step0.AddEvent(new Event(
                () => GameStatus.ate,
                end,
                new StoryPlayer().AddMessage("做得好\n現在你恢復了一些能量\n(左上方就是你的能量狀態)")
                ));
            

            return start;
        }

        static private State Teach_Creater(State end)
        {
            var start = new State();
            var step0 = new State();

            start.AddEvent(new Event(
                () => TalkBox.end,
                end,
                new StoryPlayer()
                    .AddMessage("現在說說關於村莊的事")
                    .AddMessageWithAction("看到那個東西了嗎", () => {
                        GlobalAsset.positionHint.ShowHintAt(Player.Hometown.PositOnScene);
                     })
                    .AddMessage("那是你的村莊中心")
                    .AddMessage("隨著周圍的同伴越多")
                    .AddMessage("會成長得越快")
                    .AddMessage("相反的")
                    .AddMessage("隨著同伴越少")
                    .AddMessage("會荒廢的越快")
                ));

            step0.AddEvent(new Event(
                () => TalkBox.end,
                end,
                new ActionPlayer(() => {
                    GameStatus.pause = false;
                })
                ));


            return start;
        }

        static private State Teach_Skill(State end)
        {
            var start = new State();

            start.AddEvent(new Event(
                () => TalkBox.end,
                end,
                new StoryPlayer()
                    .AddMessageWithAction("下方那一排是你可以使用的技能", () => SkillBar.Main.Flash(1))
                    .AddMessage("你可以按QWERT使用技能")
                    .AddMessage("一旦使用技能，你的體力值就會減少\n(左上方藍色的那條就是體力值)")
                    .AddMessage("滑鼠移到技能圖示上面，會有技能說明")
                    .AddMessage("活用這些技能，讓你的族群強大")
                ));

            return start;
        }
    }
}
