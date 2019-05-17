using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    public class Animal : MazeObject, Attackable
    {
        static private Shape Shape;
        static public void SetShape(Shape shape)
        {
            Animal.Shape = shape;
        }

        //(ADD)
        private static Damage_Text_Controller damage_Text_Controller = GameObject.FindGameObjectWithTag("All_Damage_Text").GetComponent<Damage_Text_Controller>();

        // for auto action
        public enum SkillCommand
        {
            ChangePlain, Attack, Straight, Horizon, Wall, Together,
            None
        }
        
        public enum MoveCommand
        {
            MoveUp, MoveDown, MoveLeft, MoveRight,
            TurnUp, TurnDown, TurnLeft, TurnRight,
            None
        }

        public SkillCommand skillCommand = SkillCommand.None;
        public MoveCommand moveCommamd = MoveCommand.None;

        public Creater Hometown { get; private set; }
        private Animal friend = null;
        private float leaveHomeRate = 0.03f;
        private int patrolDist = 0;
        private int followDist = 0;
        private Stack<Vector2D> route;
        private int surveyExtra = 3;


        private Point2D posit;
        private Vector2D Vect
        { get { return this.Plain.Vector3To2(vector); } }

        public Vector3D vector;
        public EnergyBar hp;
        public EnergyBar ep;
        public EnergyBar hungry;
        public int Power
        {
            get;
            private set;
        }
        public int Armor
        {
            get;
            private set;
        }
        private float ReduceHurtRate
        {
            get
            {
                float ret = 1.0f - Armor / 50f;
                if (ret < 0.1f)
                    ret = 0.1f;
                return ret;
            }
        }

        public Plain Plain
        { get { return posit.Plain; } }
        public Vector2D VectorOnScenen
        { get { return GlobalAsset.player.posit.Plain.Vector3To2(vector); } }
        public Dimention ForwardDimen {
            get {
                switch (vector) {
                    case Vector3D.Xn:
                    case Vector3D.Xp:
                        return Dimention.X;

                    case Vector3D.Yn:
                    case Vector3D.Yp:
                        return Dimention.Y;

                    case Vector3D.Zn:
                    case Vector3D.Zp:
                        return Dimention.Z;

                    default:
                        return Dimention.Null;
                }
            }
        }

        public bool IsDead
        {
            get
            {
                return this.hp.Value == 0;
            }
        }
        public Color Color { get; private set; }

        public override Color GetColor()
        {
            float a = hp.Value / (hp.Max * 2f) + 0.5f;
            return new Color(Color.r, Color.g, Color.b, a);
        }
        public override Sprite GetSprite()
        {
            return Animal.Shape.GetAt(this.VectorOnScenen);
        }
        public override void Destroy()
        {
            base.Destroy();
            Grid grid = World.GetAt(position); 
            if(hungry.BarRate != 0)
                grid.InsertObj(new Food(this.position, (int)((hp.Max + ep.Max) * hungry.BarRate)));
        }

        public override string Name()
        {
            return "生物";
        }

        public override string Info()
        {
            if (this == GlobalAsset.player)
                return "你";

            bool isFriend = this.Color.Equals(GlobalAsset.player.Color);

            if (isFriend)
                return string.Format("相同顏色是朋友\n 生命 {0} / {1}",hp.Value,hp.Max);
            else
                return "不同顏色是敵人\n打死他";
        }

        public Animal(Point3D position, Creater hometown) : base(position)
        {
            posit = new Point2D(this.position, Dimention.Z);
            vector = Vector3D.Xp;

            hp = new EnergyBar(200);
            ep = new EnergyBar(100);
            hungry = new EnergyBar(200);
            Power = 20;
            Armor = 0;

            Color = hometown.Color;
            ChangeHomeTown(hometown);
        }

        
        public void ChangeHomeTown(Creater creater)
        {
            if (creater.Color.Equals(this.Color))
            {
                this.Hometown = creater;
                this.patrolDist = creater.Range ;
            }
        }

        public void FollowFriend(Animal animal)
        {
            if (animal.Color.Equals(this.Color))
            {
                friend = animal;
                followDist = 3;
            }
        }
        

        /// <summary>
        /// use it after Clock()
        /// </summary>
        public void AutoSurvey()
        {
            Survey();
        }

        public void Action()
        {
            switch (moveCommamd)
            {
                case MoveCommand.MoveUp:
                    MoveFor(Vector2D.Up);
                    break;

                case MoveCommand.MoveDown:
                    MoveFor(Vector2D.Down);
                    break;

                case MoveCommand.MoveLeft:
                    MoveFor(Vector2D.Left);
                    break;

                case MoveCommand.MoveRight:
                    MoveFor(Vector2D.Right);
                    break;

                case MoveCommand.TurnUp:
                    TurnTo(Vector2D.Up);
                    break;

                case MoveCommand.TurnDown:
                    TurnTo(Vector2D.Down);
                    break;

                case MoveCommand.TurnLeft:
                    TurnTo(Vector2D.Left);
                    break;

                case MoveCommand.TurnRight:
                    TurnTo(Vector2D.Right);
                    break;

            }

            switch (skillCommand)
            {
                case SkillCommand.ChangePlain:
                    ChangePlain();
                    break;

                case SkillCommand.Attack:
                    Attack();
                    break;

                case SkillCommand.Straight:
                    Straight();
                    break;

                case SkillCommand.Horizon:
                    Horizon();
                    break;

                case SkillCommand.Wall:
                    Build();
                    break;

                case SkillCommand.Together:
                    Together();
                    break;
            }

            moveCommamd = MoveCommand.None;
            skillCommand = SkillCommand.None;
        }

        public void Clock()
        {
            if (IsDead) return;

            hungry.Add(-1);
            if (hungry.BarRate < 0.3f)
                hp.Add(-1);
            else
            {
                if (!hp.IsFull)
                {
                    hp.Add(5);
                    hungry.Add(-1);
                }
                if (!ep.IsFull)
                {
                    ep.Add(5);
                    hungry.Add(-1);
                }
            }

            if (this.hp.IsZero)
            {
                Destroy();
            }

            if(Hometown != null)
            {
                if (UnityEngine.Random.value < leaveHomeRate)
                    ++patrolDist;

                if (!Hometown.position.IsOnPlain(this.Plain) || this.posit.DistanceTo(Hometown.position) > patrolDist)
                    Hometown = null;

                if (patrolDist > 12)
                    Hometown = null;
            }

            FindNewHome();
        }


        public void Strong(int value, int power = 0,int armor=0)
        {
            this.Power += power;
            this.Armor += armor;

            this.hp.MaxExpand(value);
            this.hp.Add(value);
            this.ep.MaxExpand(value);
            this.ep.Add(value);
            this.hungry.MaxExpand(value);
            this.hungry.Add(value);
        }


        public void MoveFor(Vector2D vector)
        {
            if (this.Vect == vector)
                Move();
            else
            {
                TurnTo(vector);
                Move();
            }
        }
        
        public void ChangePlain()
        {
            ChangePlain(this.ForwardDimen);
            RegisterEvent(ObjEvent.plain);
        }

        public void Attack()
        {
            // 攻擊不耗魔.

            Point3D targetPosition = this.position.Copy();
            SkillManager.showSkill(Skill.attack, this);

            targetPosition.MoveFor(this.vector, 1);
            Grid targetGrid = World.GetAt(targetPosition);

            if (targetGrid == null) return;
            if (targetGrid.Obj == null) return;

            if (targetGrid.Obj is Attackable)
            {
                Attackable enemy = (Attackable)(targetGrid.Obj);
                enemy.BeAttack(this);
            }
        }

        public void Straight()
        {
            ep.Add(-20);
            if (ep.IsZero)
                return;

            Point3D targetPosition = this.position.Copy();
            SkillManager.showSkill(Skill.straight, this);

            for (int i = 0; i < 3; ++i)
            {
                targetPosition.MoveFor(this.vector, 1);
                Grid targetGrid = World.GetAt(targetPosition);

                if (targetGrid == null) return;
                if (targetGrid.Obj == null) continue;

                if(targetGrid.Obj is Attackable){
                    Attackable target = (Attackable)targetGrid.Obj;
                    target.BeAttack(this);
                }
            }
        }

        public void Horizon()
        {
            ep.Add(-20);
            if (ep.IsZero)
                return;

            Point2D targetPosition = this.posit.Copy();
            Vector2D targetVector = this.Vect;

            if(GlobalAsset.player != null && this.Plain.Dimention == GlobalAsset.player.Plain.Dimention)
                SkillManager.showSkill(Skill.horizon, this);
            else
                SkillManager.showSkill(Skill.attack, this);

            targetPosition.MoveFor(targetVector, 1);
            targetVector = VectorConvert.Rotate(targetVector);
            targetPosition.MoveFor(targetVector, 2);
            targetVector = VectorConvert.Invert(targetVector);
            
            for(int i=0; i<3; ++i)
            {
                targetPosition.MoveFor(targetVector, 1);
                Grid targetGrid = World.GetAt(targetPosition.Binded);

                if (targetGrid == null) continue;
                if (targetGrid.Obj == null) continue;

                if (targetGrid.Obj is Attackable)
                {
                    Attackable target = (Attackable)targetGrid.Obj;
                    target.BeAttack(this);
                }
            }

        }

        public void Build()
        {
            ep.Add(-10);
            if (ep.IsZero)
                return;

            Point3D targetPosition = this.position.Copy();

            targetPosition.MoveFor(this.vector, 1);
            Grid targetGrid = World.GetAt(targetPosition);
            if (targetGrid == null) return;

            if (targetGrid.IsEmpty())
            {
                targetGrid.InsertObj(new Wall(targetPosition, Power * 5));
            }
        }

        public void Together()
        {
            Iterator iter = new Iterator(this.PositOnScene, 3);
            do
            {
                Point2D point = iter.Iter;
                Grid grid = World.GetAt(point.Binded);

                if (grid == null || grid.Obj == null)
                    continue;

                if(grid.Obj is Animal)
                {
                    Animal animal = (Animal)grid.Obj;
                    if (animal.Color.Equals(this.Color))
                        animal.FollowFriend(this);
                }

            } while (iter.MoveToNext());

        }



        void Attackable.BeAttack(Animal enemy)
        {
            if (enemy.Color.Equals(this.Color)) return;

            int hurt = enemy.Power;
            hurt = (int)(hurt * ReduceHurtRate);
            this.hp.Add(-hurt);

            RegisterEvent(ObjEvent.damage);

            //(ADD)
            if(GlobalAsset.player != null && this.position.IsOnPlain(GlobalAsset.player.Plain))
            {
                Vector3 world_Point = Camera.main.WorldToScreenPoint(new Vector2(position.X.value, position.Y.value));

                if (world_Point.x > 0 &&
                    world_Point.y > 0 &&
                    world_Point.x < Screen.width &&
                    world_Point.y < Screen.height)
                    damage_Text_Controller.Creat_Animator(world_Point, hurt);
            }

            if (this.hp.IsZero)
            { // 收頭變強.
                Destroy();
                enemy.Strong(10, 1, 1);
            }
        }
        
        private void TurnTo(Vector2D vector)
        {
            this.vector = this.Plain.Vector2To3(vector);
            RegisterEvent(ObjEvent.sprite);
        }

        private void Move()
        {
            Point3D temp = this.position.Copy();
            temp.MoveFor(vector, 1);
            Grid targetGrid = World.GetAt(temp);

            if (targetGrid == null)
                return;

            if (targetGrid.Obj != null)
            {
                if (targetGrid.Obj is Food)
                {
                    EatFood((Food)targetGrid.Obj);
                }
                else
                    return;
            }

            World.Swap(position, temp);
            RegisterEvent(ObjEvent.posit);

            // 當照著路線走時，會把走過的標記移除.
           if(route != null && route.Count > 0)
            {
                if (route.Peek() == this.Vect) // 照著路線走.
                {
                    route.Pop();
                }
                else // 偏離路線，原本的路線已經沒用了.
                {
                    route = null;
                }
                    
            }
        }

        private void ChangePlain(Dimention dimen)
        {
            this.posit.ChangePlain(dimen);
        }

        private void EatFood(Food food)
        {
            this.hp.Add(food.Nutrient);
            this.ep.Add(food.Nutrient);
            this.hungry.Add(food.Nutrient);

            if (this == GlobalAsset.player)
                GameStatus.ate = true;

            food.Destroy();
        }


        // 偵測周圍 7*7 內有什麼.
        // 並選擇採取什麼模式.
        private void Survey()
        {
            Iterator iter = new Iterator(this.posit, surveyExtra);
            Animal animal = null;
            
            do
            {
                Point2D point = iter.Iter;
                Grid grid = World.GetAt(point.Binded);

                if (grid == null || grid.Obj == null)
                    continue;

                if(grid.Obj is Food && this.hungry.BarRate < 0.5f)
                {
                    FeedOn((Food)grid.Obj);
                    return;
                }

                if (grid.Obj is Animal)
                {
                    if (((Animal)(grid.Obj)).Color.Equals(this.Color))
                        animal = (Animal)grid.Obj;

                    else if(ep.BarRate > 0.3f)
                    {
                        Battle((Animal)grid.Obj);
                        return;
                    }
                }

                if(grid.Obj is Creater)
                {
                    Creater creater = (Creater)grid.Obj;
                    if (this.Hometown == null && creater.Color.Equals(this.Color))
                    {
                        this.ChangeHomeTown((Creater)grid.Obj);
                    }
                }

            } while (iter.MoveToNext());


            if(friend != null)
            {
                Follow();
            }
            else if (Hometown != null)
            {
                Patrol(Hometown);

                if (Hometown.IsDead)
                    Hometown = null;
            }
            else
            {
                Wander();
            }
            
        }

        

        // 招集附近的流浪夥伴一起蓋新家.
        private void FindNewHome()
        {
            if (this.Hometown != null)
                return;

            Point3D position = this.position.Copy();
            position.MoveFor(this.vector, 1);

            Grid targetGrid = World.GetAt(position);
            if (targetGrid == null || targetGrid.Obj != null) return;


            Point2D positOnPlain = new Point2D(position, Dimention.Z);
            int stoneCount = 0;
            int wallCount = 0;
            int animalCount = 0;
            bool sameColorOfAllAnimal = true;

            Iterator iter = new Iterator(positOnPlain, 2);

            do
            {
                Point2D point = iter.Iter;
                Grid grid = World.GetAt(point.Binded);

                if (grid == null || grid.Obj == null)
                    continue;

                else if (grid.Obj is Stone)
                    ++stoneCount;

                else if (grid.Obj is Wall)
                    ++wallCount;

                else if (grid.Obj is Animal && grid.Obj != this)
                {
                    Animal animal = (Animal)grid.Obj;

                    if (animal.Color.Equals(this.Color))
                    {
                        if(animal.Hometown == null)
                            ++animalCount;
                    }
                    else
                    {
                        sameColorOfAllAnimal = false;
                        break;
                    }
                }

                else if (grid.Obj is Creater)
                {
                    Creater creater = (Creater)grid.Obj;
                    this.ChangeHomeTown(creater);
                    return;
                }
                
            } while (iter.MoveToNext());

            if(sameColorOfAllAnimal && animalCount > 1)
            {
                Creater newHome = new Creater(position, this.Color);
                targetGrid.InsertObj(newHome);
                this.Hometown = newHome;
                GlobalAsset.creaters.Add(newHome);
            }
        }
        
        // 在家附近巡邏.不會離太遠.
        private void Patrol(Creater home)
        {
            Point2D temp = this.posit.Copy();
            Vector2D target = RandomVector(10);

            for(int i=0; i<4; ++i)
            {
                temp.MoveFor(target, 1);
                if (temp.DistanceTo(home.position) > patrolDist)
                {
                    temp.MoveFor(target, -1);
                    target = VectorConvert.Rotate(target);
                }
                else
                {
                    moveCommamd = Convert(target);
                    return;
                }
            }

            skillCommand = SkillCommand.None;
            patrolDist = this.posit.DistanceTo(home.position);
        }

        // 追擊某個敵人.直到某些狀況才結束.(未完成)
        private void Battle(Animal enemy)
        {
            skillCommand = DetermineSkill(enemy.position);

            if(skillCommand == SkillCommand.None)
                Wander();
        }

        // 跑去吃某個食物.
        private void FeedOn(Food food)
        {
            if (route != null)
                FollowRoute();

            route = new BFS_Map(this.posit, new Point2D(food.position, Dimention.Z), surveyExtra).FindRoute();
            if (route == null)
                Wander();
            else
                FollowRoute();

          //  Wander();
        }

        // 跟隨某個同伴.
        private void Follow()
        {
            if (friend.IsDead)
            {
                friend = null;
                Wander();
                return;
            }

            if (this.posit.DistanceTo(friend.position) < followDist)
                Wander();
            else
            {
                route = new BFS_Map(this.posit, new Point2D(friend.position, Dimention.Z), followDist + 3).FindRoute();
                FollowRoute();
            }
            
        }

        // 漫無目的亂走.
        private void Wander()
        {
            Vector2D targetVector = RandomVector(10);

            for(int i = 0; i < 4; ++i)
            {
                Point2D target = this.posit.Copy();
                target.MoveFor(targetVector, 1);
                Grid targetGrid = World.GetAt(target.Binded);

                if (targetGrid != null && ( targetGrid.IsEmpty() || targetGrid.Obj is Food) )
                    break;
                else
                    targetVector = VectorConvert.Rotate(targetVector);

            }
            

            moveCommamd = Convert(targetVector);
        }

        // 照著路線走.
        private void FollowRoute()
        {
            if (route == null || route.Count == 0)
            {
                Wander();
                return;
            }

            Vector2D targetVector = route.Peek();
            moveCommamd = Convert(targetVector);

            // 如果成功移動的話，route.Pop().
            // 會在Move()判斷.
                
        }



        private Vector2D RandomVector(int arg)
        {
            switch (UnityEngine.Random.Range(0, arg))
            {
                case 0:
                    return Vector2D.Right;
                case 1:
                    return Vector2D.Down;
                case 2:
                    return Vector2D.Left;
                case 3:
                    return Vector2D.Up;
                default:
                    return this.Vect;
            }
        }

        private MoveCommand Convert(Vector2D vector)
        {
            switch (vector)
            {
                case Vector2D.Right:
                    return MoveCommand.MoveRight;
                case Vector2D.Down:
                    return MoveCommand.MoveDown;
                case Vector2D.Left:
                    return MoveCommand.MoveLeft;
                case Vector2D.Up:
                    return MoveCommand.MoveUp;
                default:
                    return MoveCommand.None;
            }
        }

        private SkillCommand DetermineSkill(Point3D target)
        {
            Point2D point = this.posit.Copy();
            point.MoveFor(this.Vect,1);

            if (point.Binded.Equals(target))
                return SkillCommand.Attack;

            if (target.IsOnRange(new Range2D(point, this.Vect, 3, 1)))
                return SkillCommand.Horizon;

            if (target.IsOnRange(new Range2D(point, this.Vect, 1, 3)))
                return SkillCommand.Straight;

            return SkillCommand.None;
        }
        


    }
}
