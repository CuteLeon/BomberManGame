using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 炸弹超人
{
    class EnemyModel:Cell,IDisposable
    {
        /// <summary>
        /// 拉伸后的敌人图像（节省多次拉伸计算资源）
        /// </summary>
        public Bitmap EnemyCellImage;
        /// <summary>
        /// 巡逻线程
        /// </summary>
        Thread PatrolThread;
        /// <summary>
        /// 巡逻事件句柄
        /// </summary>
        /// <param name="sender"></param>
        public delegate void PatrolEventHander(EnemyModel sender, Point EnemyLocation);
        /// <summary>
        /// 巡逻事件
        /// </summary>
        public event PatrolEventHander Patrol;
        /// <summary>
        /// 随机数发生器，用于巡逻定向
        /// </summary>
        private Random UnityRandom = new Random();
        /// <summary>
        /// 枚举巡逻方向
        /// </summary>
        private enum PatrolDirection
        {
            None=0,
            Up=1,
            Down=2,
            Left=3,
            Right=4
        }
        /// <summary>
        /// 上次巡逻方向
        /// </summary>
        private PatrolDirection LastDirection=PatrolDirection.None;
        /// <summary>
        /// 敌人所在游戏地图
        /// </summary>
        public Map GameMap;
        /// <summary>
        /// 地图背景副本
        /// </summary>
        public Bitmap MapGround;

        /// <summary>
        /// 被隐藏的构造函数（禁止访问）
        /// </summary>
        /// <param name="location"></param>
        /// <param name="tabelLocation"></param>
        private EnemyModel(Point location, Point tabelLocation) : base(location,tabelLocation) { }

        /// <summary>
        /// 继承自Cell类的构造函数
        /// </summary>
        /// <param name="GameMap"></param>
        /// <param name="tabelLocation"></param>
        public EnemyModel(Map gameMap, Point tabelLocation):base(gameMap,tabelLocation){
            GameMap = gameMap;
            EnemyCellImage = new Bitmap(UnityResource.Enemy, GameMap.CellSize);
            PatrolThread = new Thread(delegate (){ StartPatrol(); });
            PatrolThread.Start();
            MapGround = (Bitmap)GameMap.Ground.Clone();
        }

        /// <summary>
        /// 敌人角色自动巡逻
        /// </summary>
        private void StartPatrol()
        {
            //记录移动之前的坐标，作为事件参数
            //Debug.Print("敌人开始巡逻！");
            Point LastLocation;
            while (true)
            {
                LastLocation = this.Location;

                //Debug.Print(DateTime.Now.ToString() + " 巡逻方向：" + LastDirection.ToString());
                switch (LastDirection)
                {
                    case PatrolDirection.None:
                        {
                            //四周都是墙壁，无法巡逻时等待
                            System.Threading.Thread.Sleep(UnityRandom.Next(500));
                            if (CanGo(PatrolDirection.Up)) { LastDirection = PatrolDirection.Up; break; }
                            if (CanGo(PatrolDirection.Down)) { LastDirection = PatrolDirection.Down; break; }
                            if (CanGo(PatrolDirection.Left)) { LastDirection = PatrolDirection.Left; break; }
                            if (CanGo(PatrolDirection.Right)) { LastDirection = PatrolDirection.Right; break; }
                            break;
                        }
                    case PatrolDirection.Up:
                        {
                            if (CanGo(PatrolDirection.Up))
                            {
                                this.TabelLocation.Offset(0, -1);
                                int Target = this.TabelLocation.Y * GameMap.CellSize.Height + GameMap.PaddingSize.Height;
                                while (this.Location.Y > Target + 5)
                                {
                                    Thread.Sleep(100);
                                    this.Location.Offset(0, -5);
                                    Patrol(this, LastLocation);
                                }
                                this.Location = new Point(this.Location.X, Target);
                                Patrol(this, LastLocation);

                                //正常移动过程中有很小概率转向
                                if (UnityRandom.NextDouble() > 0.9)
                                {
                                    //Debug.Print(DateTime.Now.ToString() + " 巡逻时随机转向!");
                                    LastDirection = GetRandomDirection();
                                }
                            }
                            else
                            {
                                //移动撞墙时转向
                                //Debug.Print(DateTime.Now.ToString() + " 撞墙，强制转向！");
                                LastDirection = GetRandomDirection();
                            }
                            break;
                        }
                    case PatrolDirection.Down:
                        {
                            if (CanGo(PatrolDirection.Down))
                            {
                                this.TabelLocation.Offset(0, 1);
                                int Target = this.TabelLocation.Y * GameMap.CellSize.Height + GameMap.PaddingSize.Height;
                                while (this.Location.Y < Target - 5)
                                {
                                    Thread.Sleep(100);
                                    this.Location.Offset(0, +5);
                                    Patrol(this, LastLocation);
                                }
                                this.Location = new Point(this.Location.X, Target);
                                Patrol(this, LastLocation);

                                //正常移动过程中有很小概率转向
                                if (UnityRandom.NextDouble() > 0.9)
                                {
                                    //Debug.Print(DateTime.Now.ToString() + " 巡逻时随机转向!");
                                    LastDirection = GetRandomDirection();
                                }
                            }
                            else
                            {
                                //移动撞墙时转向
                                //Debug.Print(DateTime.Now.ToString() + " 撞墙，强制转向！");
                                LastDirection = GetRandomDirection();
                            }
                            break;
                        }
                    case PatrolDirection.Left:
                        {
                            if (CanGo(PatrolDirection.Left))
                            {
                                this.TabelLocation.Offset(-1,0);
                                int Target = this.TabelLocation.X * GameMap.CellSize.Width + GameMap.PaddingSize.Width;
                                while (this.Location.X > Target + 5)
                                {
                                    Thread.Sleep(100);
                                    this.Location.Offset(-5,0);
                                    Patrol(this, LastLocation);
                                }
                                this.Location = new Point(Target, this.Location.Y);
                                Patrol(this, LastLocation);

                                //正常移动过程中有很小概率转向
                                if (UnityRandom.NextDouble() > 0.9)
                                {
                                    //Debug.Print(DateTime.Now.ToString() + " 巡逻时随机转向!");
                                    LastDirection = GetRandomDirection();
                                }
                            }
                            else
                            {
                                //移动撞墙时转向
                                //Debug.Print(DateTime.Now.ToString() + " 撞墙，强制转向！");
                                LastDirection = GetRandomDirection();
                            }
                            break;
                        }
                    case PatrolDirection.Right:
                        {
                            if (CanGo(PatrolDirection.Right))
                            {
                                this.TabelLocation.Offset(1, 0);
                                int Target = this.TabelLocation.X * GameMap.CellSize.Width + GameMap.PaddingSize.Width;
                                while (this.Location.X < Target - 5)
                                {
                                    Thread.Sleep(100);
                                    this.Location.Offset(+5, 0);
                                    Patrol(this, LastLocation);
                                }
                                this.Location = new Point(Target, this.Location.Y);
                                Patrol(this, LastLocation);

                                //正常移动过程中有很小概率转向
                                if (UnityRandom.NextDouble() > 0.9)
                                {
                                    //Debug.Print(DateTime.Now.ToString() + " 巡逻时随机转向!");
                                    LastDirection = GetRandomDirection();
                                }
                            }
                            else
                            {
                                //移动撞墙时转向
                                //Debug.Print(DateTime.Now.ToString() + " 撞墙，强制转向！");
                                LastDirection = GetRandomDirection();
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 判断是否转到指定方向
        /// </summary>
        /// <returns>是否可以转向</returns>
        private bool CanGo(PatrolDirection Direction)
        {
            switch (Direction)
            {
                case PatrolDirection.None:
                    throw new Exception("试图转向：None");
                case PatrolDirection.Up:
                        return (GameMap.MapCellsClone[this.TabelLocation.Y - 1, this.TabelLocation.X] == Map.CellType.Ground);
                case PatrolDirection.Down:
                        return (GameMap.MapCellsClone[this.TabelLocation.Y + 1, this.TabelLocation.X] == Map.CellType.Ground);
                case PatrolDirection.Left:
                        return (GameMap.MapCellsClone[this.TabelLocation.Y , this.TabelLocation.X-1] == Map.CellType.Ground);
                case PatrolDirection.Right:
                        return (GameMap.MapCellsClone[this.TabelLocation.Y, this.TabelLocation.X+1] == Map.CellType.Ground);
            }
            return false;
        }

        /// <summary>
        /// 获取一个随机方向
        /// </summary>
        /// <returns>随机方向</returns>
        private PatrolDirection GetRandomDirection()
        {
            //Debug.Print("开始随机转向...");
            List<PatrolDirection> DirectionEnableList = new List<PatrolDirection>();
            for (int Index = 1; Index < 5; Index++)
            {
                if (CanGo((PatrolDirection)Index))
                    DirectionEnableList.Add((PatrolDirection)Index);
            }

            if (DirectionEnableList.Count == 0) return PatrolDirection.None;
            return DirectionEnableList[UnityRandom.Next(DirectionEnableList.Count)];
        }

        /// <summary>
        /// 清理线程
        /// </summary>
        public void Dispose()
        {
            PatrolThread.Abort();
            PatrolThread = null;
        }
    }
}
