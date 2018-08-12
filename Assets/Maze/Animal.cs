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


        // for auto action
        enum Command
        {
            Up, Down, Left, Right, Plain, Attack, Straight, Horizon, Wall, None
        }
        private Command command = Command.None;
        private Animal friend = null;
        private int patrolDist = 0;
        private int followDist = 0;
        private float leaveHomeRate = 0.03f;


        private Point2D posit;
        private Vector2D Vect
        { get { return this.Plain.Vector3To2(vector); } }
        public Color Color { get; private set; }

        public Vector3D vector;
        public EnergyBar hp;
        public EnergyBar ep;
        public EnergyBar hungry;
        public int power;

        public bool isDead
        { get { return this.hp.Value == 0; } }

        public Plain Plain
        { get { return posit.Plain; } }

        public Vector2D vectorOnScenen
        { get { return GlobalAsset.player.posit.Plain.Vector3To2(vector); } }


        public Dimention forwardDimen {
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

        public override Color GetColor()
        {
            float a = hp.Value / (hp.Max * 2f) + 0.5f;
            return new Color(Color.r, Color.g, Color.b, a);
        }

        public Creater Hometown{ get; private set; }

        public Animal(Point3D position, Creater hometown) : base(position)
        {
            posit = new Point2D(this.position, Dimention.Z);
            vector = Vector3D.Xp;
            hp = new EnergyBar(200);
            ep = new EnergyBar(100);
            hungry = new EnergyBar(200);
            this.Color = hometown.GetColor();
            this.power = 20;
            this.Hometown = hometown;
            this.patrolDist = hometown.Level + 1;
        }

        public override Sprite GetSprite()
        {
            return Animal.Shape.GetAt(this.vectorOnScenen);
        }

        public void ChangeHomeTown(Creater creater)
        {
            if (creater.GetColor().Equals(this.Color))
            {
                this.Hometown = creater;
                this.patrolDist = creater.Level +1 ;
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            Grid grid = World.GetAt(position); 
            if(hungry.BarRate != 0)
                grid.InsertObj(new Food(this.position, (int)((hp.Max + ep.Max) * hungry.BarRate)));
        }

        

        /// <summary>
        /// use it after Clock()
        /// </summary>
        public void Auto()
        {
            Survey();
            Action();
        }
        
        public void Clock()
        {
            if (isDead) return;

            hungry.Add(-1);
            if (hungry.BarRate < 0.3f)
                hp.Add(-1);
            else
            {
                if (!hp.IsFull)
                {
                    hp.Add(1);
                    hungry.Add(-1);
                }
                if (!ep.IsFull)
                {
                    ep.Add(1);
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


        public void Strong(int value)
        {
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
                TurnTo(vector);
        }
        
        public void ChangePlain()
        {
            ChangePlain(this.forwardDimen);
            RegisterEvent(ObjEvent.plain);
        }

        public void Attack()
        {
            ep.Add(-5);
            if (ep.IsZero)
                return;

            Point3D targetPosition = this.position.Copy();
            SkillManager.showSkill(Skill.attack, this.position, this.vector);

            targetPosition.MoveFor(this.vector, 1);
            Grid targetGrid = World.GetAt(targetPosition);

            if (targetGrid == null) return;
            if (targetGrid.Obj == null) return;

            if (targetGrid.Obj is Animal)
            {
                Animal enemy = (Animal)(targetGrid.Obj);
                if (!enemy.Color.Equals(this.Color))
                    enemy.BeAttack(this);
            }
            else if(targetGrid.Obj is Wall)
            {
                Wall wall = (Wall)targetGrid.Obj;
                wall.BeAttack(this);
            }
        }

        public void Straight()
        {
            ep.Add(-20);
            if (ep.IsZero)
                return;

            Point3D targetPosition = this.position.Copy();
            SkillManager.showSkill(Skill.straight, this.position, this.vector);

            for (int i = 0; i < 3; ++i)
            {
                targetPosition.MoveFor(this.vector, 1);
                Grid targetGrid = World.GetAt(targetPosition);

                if (targetGrid == null) return;
                if (targetGrid.Obj == null) continue;

                if(targetGrid.Obj is Animal){
                    Animal target = (Animal)targetGrid.Obj;
                    if(!target.Color.Equals(this.Color))
                        target.BeAttack(this);
                }
                else if (targetGrid.Obj is Wall)
                {
                    Wall wall = (Wall)targetGrid.Obj;
                    wall.BeAttack(this);
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
                SkillManager.showSkill(Skill.horizon, this.position, this.vector);
            else
                SkillManager.showSkill(Skill.attack, this.position, this.vector);

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

                if (targetGrid.Obj is Animal)
                {
                    Animal target = (Animal)targetGrid.Obj;
                    if (!target.Color.Equals(this.Color))
                        target.BeAttack(this);
                }
                else if (targetGrid.Obj is Wall)
                {
                    Wall wall = (Wall)targetGrid.Obj;
                    wall.BeAttack(this);
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
                targetGrid.InsertObj(new Wall(targetPosition, 100));
            }
        }



        public void BeAttack(Animal enemy)
        {
            this.hp.Add(-enemy.power);

            if (this.hp.IsZero)
            {
                Destroy();
            }
        }
        
        private void TurnTo(Vector2D vector)
        {
            this.vector = this.Plain.Vector2To3(vector);
            RegisterEvent(ObjEvent.shape);
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
            RegisterEvent(ObjEvent.move);
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

            food.Destroy();
        }


        // 偵測周圍 7*7 內有什麼.
        // 並選擇採取什麼模式.
        private void Survey()
        {
            Iterator iter = new Iterator(this.posit, 3);
            Animal animal = null;

            do
            {
                Point2D point = iter.Iter;
                Grid grid = World.GetAt(point.Binded);

                if (grid == null || grid.Obj == null)
                    continue;

                /*if(grid.Obj is Food)
                {
                    FeedOn((Food)grid.Obj);
                    return;
                }*/

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
                    if (this.Hometown == null && grid.Obj.GetColor().Equals(this.Color))
                    {
                        this.ChangeHomeTown((Creater)grid.Obj);
                    }
                }

            } while (iter.MoveToNext());


            if (Hometown != null)
            {
                Patrol(Hometown);

                if (Hometown.IsDead)
                    Hometown = null;
            }
            else if(friend != null)
            {
                Follow();
            }
            else if(animal != null)
            {
                this.friend = animal;
                followDist = 3;
                Follow();
            }
            else
            {
                Wander();
            }
            
        }

        private void Action()
        {
            switch (command)
            {
                case Command.Up:
                    MoveFor(Maze.Vector2D.Up);
                    break;

                case Command.Down:
                    MoveFor(Maze.Vector2D.Down);
                    break;

                case Command.Left:
                    MoveFor(Maze.Vector2D.Left);
                    break;

                case Command.Right:
                    MoveFor(Maze.Vector2D.Right);
                    break;

                case Command.Plain:
                    ChangePlain();
                    break;

                case Command.Attack:
                    Attack();
                    break;

                case Command.Straight:
                    Straight();
                    break;

                case Command.Horizon:
                    Horizon();
                    break;

                case Command.Wall:
                    Build();
                    break;
            }
            command = Command.None;
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
                    command = Convert(target);
                    return;
                }
            }

            command = Command.None;
            patrolDist = this.posit.DistanceTo(home.position);
        }

        // 追擊某個敵人.直到某些狀況才結束.(未完成)
        private void Battle(Animal enemy)
        {
            command = DetermineSkill(enemy.position);

            if(command == Command.None)
                Wander();
        }

        // 跑去吃某個食物.(未完成)
        private void FeedOn(Food food)
        {
            Wander();
        }

        // 跟隨某個同伴.
        private void Follow()
        {
            Point2D temp = this.posit.Copy();
            Vector2D target = RandomVector(10);

            for (int i = 0; i < 4; ++i)
            {
                temp.MoveFor(target, 1);
                if (temp.DistanceTo(friend.position) > followDist)
                {
                    temp.MoveFor(target, -1);
                    target = VectorConvert.Rotate(target);
                }
                else
                {
                    command = Convert(target);
                    return;
                }
            }

            command = Command.None;
            followDist = this.posit.DistanceTo(friend.position);
        }

        // 漫無目的亂走.
        private void Wander()
        {
            Vector2D target = RandomVector(10);
            command = Convert(target);
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

        private Command Convert(Vector2D vector)
        {
            switch (vector)
            {
                case Vector2D.Right:
                    return Command.Right;
                case Vector2D.Down:
                    return Command.Down;
                case Vector2D.Left:
                    return Command.Left;
                case Vector2D.Up:
                    return Command.Up;
                default:
                    return Command.None;
            }
        }

        private Command DetermineSkill(Point3D target)
        {
            Point2D point = this.posit.Copy();
            point.MoveFor(this.Vect,1);

            if (point.Binded.Equals(target))
                return Command.Attack;

            if (target.IsOnRange(new Range2D(point, this.Vect, 3, 1)))
                return Command.Horizon;

            if (target.IsOnRange(new Range2D(point, this.Vect, 1, 3)))
                return Command.Straight;

            return Command.None;
        }
    }
}
