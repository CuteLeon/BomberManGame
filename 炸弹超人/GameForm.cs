using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//todo:奖励 通关门
//todo:每次移动后不要drawmines()

namespace 炸弹超人
{
    public partial class GameForm : Form
    {
        /// <summary>
        /// 敌人列表
        /// </summary>
        List<EnemyModel> EnemyList=new List<EnemyModel>();
        /// <summary>
        /// 游戏地图对象
        /// </summary>
        private Map GameMap;
        /// <summary>
        /// 玩家对象
        /// </summary>
        private PlayerModel Player;
        /// <summary>
        /// 已经放置的炸弹列表
        /// </summary>
        private List<MineModel> Mines;
        /// <summary>
        /// 玩家角色正在移动线程中
        /// </summary>
        private bool PlayerMoveing;
        /// <summary>
        /// 拉伸后的玩家图像（节省多次拉伸计算资源）
        /// </summary>
        private Bitmap PlayerCellImage;
        /// <summary>
        /// 拉伸后的炸弹图像（节省多次拉伸计算资源）
        /// </summary>
        private Bitmap MineCellImage;
        /// <summary>
        /// 拉伸后的死亡敌人图像（节省多次拉伸计算资源）
        /// </summary>
        private Bitmap EnemyDeadCellImage;
        /// <summary>
        /// 拉伸后的烟雾图像（节省多次拉伸计算资源）
        /// </summary>
        private Bitmap SmokeCellImage;
        /// <summary>
        /// 拉伸后的破损墙图像（节省多次拉伸计算资源）
        /// </summary>
        private Bitmap WallBrokenCellImage;

        public GameForm()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            
            this.SetBounds(0,0,Screen .PrimaryScreen .Bounds .Width ,Screen .PrimaryScreen .Bounds .Height );
            GameMap = new Map(Screen.PrimaryScreen.Bounds.Size);
            //计算拉伸后的玩家、炸弹、敌人、烟雾、破损墙图像
            PlayerCellImage = new Bitmap(UnityResource.Player, GameMap.CellSize);
            MineCellImage = new Bitmap(UnityResource.Mine, GameMap.CellSize);
            EnemyDeadCellImage= new Bitmap(UnityResource.Enemy_Dead, GameMap.CellSize);
            SmokeCellImage = new Bitmap(UnityResource.Smoke, GameMap.CellSize);
            WallBrokenCellImage = new Bitmap(UnityResource.Wall_Broken, GameMap.CellSize);
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            this.BackgroundImage = GameMap.Ground;
            GC.Collect();
        }

        private void GameForm_Click(object sender, EventArgs e)
        {
            ResetGame();
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (PlayerMoveing) return;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    {
                        if (GameMap.MapCellsClone[Player.TabelLocation.Y - 1, Player.TabelLocation.X] == Map.CellType.Ground)
                        {
                            PlayerMoveing = true;
                            Player.TabelLocation.Offset(0, -1);
                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                using (Graphics UnityGraphics = this.CreateGraphics())
                                {
                                    int Target = (Player.TabelLocation.Y) * GameMap.CellSize.Height + GameMap.PaddingSize.Height;
                                    while (Player.Location.Y > Target + 5)
                                    {
                                        Thread.Sleep(10);
                                        //屏蔽掉会有很有趣的轨迹效果
                                        UnityGraphics.DrawImage(Player.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                        Player.Location.Offset(0, -5);
                                        UnityGraphics.DrawImageUnscaled(PlayerCellImage, Player.Location);
                                    }
                                    //屏蔽掉，会有很有趣的痕迹效果
                                    UnityGraphics.DrawImage(Player.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                    Player.Location = new Point(Player.Location.X, Target);
                                    UnityGraphics.DrawImageUnscaled(PlayerCellImage, Player.Location);
                                }
                                PlayerMoveing = false;
                                DrawMines();
                            });
                        }
                        break;
                    }
                case Keys.Down:
                    {
                        if (GameMap.MapCellsClone[Player.TabelLocation.Y + 1, Player.TabelLocation.X] == Map.CellType.Ground)
                        {
                            PlayerMoveing = true;
                            Player.TabelLocation.Offset(0, 1);
                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                using (Graphics UnityGraphics = this.CreateGraphics())
                                {
                                    int Target = (Player.TabelLocation.Y) * GameMap.CellSize.Height + GameMap.PaddingSize.Height;
                                    while (Player.Location.Y < Target - 5)
                                    {
                                        Thread.Sleep(10);
                                        //屏蔽掉会有很有趣的轨迹效果
                                        UnityGraphics.DrawImage(Player.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                        Player.Location.Offset(0, 5);
                                        UnityGraphics.DrawImageUnscaled(PlayerCellImage, Player.Location);
                                    }
                                    //屏蔽掉，会有很有趣的痕迹效果
                                    UnityGraphics.DrawImage(Player.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                    Player.Location = new Point(Player.Location.X, Target);
                                    UnityGraphics.DrawImageUnscaled(PlayerCellImage, Player.Location);
                                }
                                PlayerMoveing = false;
                                DrawMines();
                            });
                        }
                        break;
                    }
                case Keys.Left:
                    {
                        if (GameMap.MapCellsClone[Player.TabelLocation.Y, Player.TabelLocation.X - 1] == Map.CellType.Ground)
                        {
                            PlayerMoveing = true;
                            Player.TabelLocation.Offset(-1, 0);
                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                using (Graphics UnityGraphics = this.CreateGraphics())
                                {
                                    int Target = (Player.TabelLocation.X) * GameMap.CellSize.Width + GameMap.PaddingSize.Width;
                                    while (Player.Location.X > Target + 5)
                                    {
                                        Thread.Sleep(10);
                                        //屏蔽掉会有很有趣的轨迹效果
                                        UnityGraphics.DrawImage(Player.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                        Player.Location.Offset(-5, 0);
                                        UnityGraphics.DrawImageUnscaled(PlayerCellImage, Player.Location);
                                    }
                                    //屏蔽掉，会有很有趣的痕迹效果
                                    UnityGraphics.DrawImage(Player.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                    Player.Location = new Point(Target, Player.Location.Y);
                                    UnityGraphics.DrawImageUnscaled(PlayerCellImage, Player.Location);
                                }
                                PlayerMoveing = false;
                                DrawMines();
                            });
                        }
                        break;
                    }
                case Keys.Right:
                    {
                        if (GameMap.MapCellsClone[Player.TabelLocation.Y, Player.TabelLocation.X + 1] == Map.CellType.Ground)
                        {
                            PlayerMoveing = true;
                            Player.TabelLocation.Offset(1, 0);
                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                using (Graphics UnityGraphics = this.CreateGraphics())
                                {
                                    int Target = (Player.TabelLocation.X) * GameMap.CellSize.Width + GameMap.PaddingSize.Width;
                                    while (Player.Location.X < Target - 5)
                                    {
                                        Thread.Sleep(10);
                                        //屏蔽掉会有很有趣的轨迹效果
                                        UnityGraphics.DrawImage(Player.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                        Player.Location.Offset(5, 0);
                                        UnityGraphics.DrawImageUnscaled(PlayerCellImage, Player.Location);
                                    }
                                    //屏蔽掉，会有很有趣的痕迹效果
                                    UnityGraphics.DrawImage(Player.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                    Player.Location = new Point(Target, Player.Location.Y);
                                    UnityGraphics.DrawImageUnscaled(PlayerCellImage, Player.Location);
                                }
                                PlayerMoveing = false;
                                DrawMines();
                            });
                        }
                        break;
                    }

                case Keys.Space:
                    {
                        //按空格键释放炸弹
                        if (Player.CanPlaceBomb())
                        {
                            if (Mines.FirstOrDefault(X => X.TabelLocation.Equals(Player.TabelLocation)) == null)
                            {
                                GameMap.MapCellsClone[Player.TabelLocation.Y, Player.TabelLocation.X] = Map.CellType.Mine;
                                Player.PlaceBomb();
                                Mines.Add(new MineModel(GameMap, Player.TabelLocation));
                                Mines.Last().Blast += new MineModel.BlastEventHander(BombBlast);
                                using (Graphics UnityGraphics = this.CreateGraphics())
                                {
                                    UnityGraphics.DrawImageUnscaled(MineCellImage,Mines.Last().Location);
                                    UnityGraphics.DrawImageUnscaled(PlayerCellImage, Player.Location);
                                }
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 初始化/重置游戏
        /// </summary>
        private void ResetGame()
        {
            foreach (EnemyModel Enemy in EnemyList)
            {
                Enemy.Dispose();
                //Enemy.Patrol -=new EnemyModel.PatrolEventHander ( EnemyPatrol);
            }
            
            using (Graphics UnityGraphics = this.CreateGraphics())
            {
                GameMap.ResetMap();
                UnityGraphics.DrawImageUnscaled(GameMap.Ground, Point.Empty);
                UnityGraphics.DrawImageUnscaled(GameMap.DrawWalls(), Point.Empty);

                Player = new PlayerModel(GameMap,new Point(1, 1));
                UnityGraphics.DrawImageUnscaled(PlayerCellImage, Player.Location);

                Mines = new List<MineModel>();

                EnemyList = GameMap.CreateEnemy(5);
                foreach (EnemyModel Enemy in EnemyList)
                {
                    UnityGraphics.DrawImageUnscaled(Enemy.EnemyCellImage, Enemy.Location);
                    Enemy.Patrol += new EnemyModel.PatrolEventHander(EnemyPatrol);
                }
            }

            this.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.GameForm_KeyDown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GameForm_KeyDown);
        }

        /// <summary>
        /// 绘制炸弹图层
        /// </summary>
        private void DrawMines()
        {
            using (Graphics UnityGraphics = this.CreateGraphics())
                foreach (Cell Mine in Mines)
                    UnityGraphics.DrawImageUnscaled(MineCellImage, Mine.Location);
            GC.Collect();
        }

        /// <summary>
        /// 敌人触发巡逻事件
        /// </summary>
        private void EnemyPatrol(EnemyModel Sender,Point EnemyLocation)
        {
            using (Graphics UnityGraphics = this.CreateGraphics())
            {
                //Debug.Print("敌人 {0},{1} 触发巡逻事件！" ,Sender.TabelLocation.X,Sender.TabelLocation.Y);
                UnityGraphics.DrawImage(Sender.MapGround.Clone(new Rectangle(EnemyLocation, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), EnemyLocation);
                UnityGraphics.DrawImageUnscaled(Sender.EnemyCellImage, Sender.Location);
                
                //玩家与敌人碰撞，受伤
                if (new Rectangle(Sender.Location, GameMap.CellSize).IntersectsWith(new Rectangle(Player.Location, GameMap.CellSize)))
                {
                    //必须回到this.Invoke()线程，才可以正常重置游戏！
                    this.Invoke(new Action(()=> {
                        MessageBox.Show(this,"玩家被敌人撞伤！游戏结束！");
                        ResetGame();
                    }));
                    return;
                }
            }
            GC.Collect();
        }

        /// <summary>
        /// 炸弹爆炸事件
        /// </summary>
        private void BombBlast(MineModel sender)
        {
            using (Graphics UnityGraphics = this.CreateGraphics())
            {
                List<Cell> SmokePoints = new List<Cell>();
                Player.BombCount++;
                SmokePoints.Add(new Cell(sender.Location,sender.TabelLocation));
                GameMap.MapCellsClone[sender.TabelLocation.Y, sender.TabelLocation.X] = Map.CellType.Ground;
                UnityGraphics.DrawImageUnscaled(SmokeCellImage, sender.Location);
                Point DrawLocation,LocationInTabel;

                for (int Radius = 1; Radius <= Player.BlastRadius; Radius++)
                {
                    DrawLocation = new Point(sender.Location.X, sender.Location.Y - GameMap.CellSize.Height * Radius);
                    LocationInTabel = new Point(sender.TabelLocation.X, sender.TabelLocation.Y - Radius);
                    if (GameMap.MapCellsClone[LocationInTabel.Y,LocationInTabel.X] == Map.CellType.Wall)
                    {
                        SmokePoints.Add(new Cell(DrawLocation,LocationInTabel));
                        UnityGraphics.DrawImageUnscaled(WallBrokenCellImage, DrawLocation);
                        break;
                    }
                    else if (GameMap.MapCellsClone[LocationInTabel.Y,LocationInTabel.X] == Map.CellType.Ground)
                    {
                        SmokePoints.Add(new Cell(DrawLocation, LocationInTabel));
                        UnityGraphics.DrawImageUnscaled(SmokeCellImage, DrawLocation);
                    }
                    else if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Stone)
                        break;
                    else if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Mine)
                    {
                        Mines.Where(X => X.TabelLocation.Equals(LocationInTabel))?.First().BlastNow();
                        break;
                    }
                }

                for (int Radius = 1; Radius <= Player.BlastRadius; Radius++)
                {
                    DrawLocation = new Point(sender.Location.X, sender.Location.Y + GameMap.CellSize.Height * Radius);
                    LocationInTabel = new Point(sender.TabelLocation.X,sender.TabelLocation.Y + Radius);
                    if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Wall)
                    {
                        SmokePoints.Add(new Cell(DrawLocation, LocationInTabel));
                        UnityGraphics.DrawImageUnscaled(WallBrokenCellImage, DrawLocation);
                        break;
                    }
                    else if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Ground)
                    {
                        SmokePoints.Add(new Cell(DrawLocation, LocationInTabel));
                        UnityGraphics.DrawImageUnscaled(SmokeCellImage, DrawLocation);
                    }
                    else if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Stone)
                        break;
                    else if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Mine)
                    {
                        Mines.Where(X => X.TabelLocation.Equals(LocationInTabel))?.First().BlastNow();
                        break;
                    }
                }

                for (int Radius = 1; Radius <= Player.BlastRadius; Radius++)
                {
                    DrawLocation = new Point(sender.Location.X - GameMap.CellSize.Width * Radius, sender.Location.Y);
                    LocationInTabel = new Point(sender.TabelLocation.X - Radius, sender.TabelLocation.Y);
                    if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Wall)
                    {
                        SmokePoints.Add(new Cell(DrawLocation, LocationInTabel));
                        UnityGraphics.DrawImageUnscaled(WallBrokenCellImage, DrawLocation);
                        break;
                    }
                    else if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Ground)
                    {
                        SmokePoints.Add(new Cell(DrawLocation, LocationInTabel));
                        UnityGraphics.DrawImageUnscaled(SmokeCellImage, DrawLocation);
                    }
                    else if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Stone)
                        break;
                    else if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Mine)
                    {
                        Mines.Where(X => X.TabelLocation.Equals(LocationInTabel))?.First().BlastNow();
                        break;
                    }
                }
                for (int Radius = 1; Radius <= Player.BlastRadius; Radius++)
                {
                    DrawLocation = new Point(sender.Location.X + GameMap.CellSize.Width * Radius, sender.Location.Y);
                    LocationInTabel = new Point(sender.TabelLocation.X + Radius, sender.TabelLocation.Y);
                    if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Wall)
                    {
                        SmokePoints.Add(new Cell(DrawLocation, LocationInTabel));
                        UnityGraphics.DrawImageUnscaled(WallBrokenCellImage, DrawLocation);
                        break;
                    }
                    else if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Ground)
                    {
                        SmokePoints.Add(new Cell(DrawLocation, LocationInTabel));
                        UnityGraphics.DrawImageUnscaled(SmokeCellImage, DrawLocation);
                    }
                    else if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Stone)
                        break;
                    else if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Mine)
                    {
                        Mines.Where(X => X.TabelLocation.Equals(LocationInTabel))?.First().BlastNow();
                        break;
                    }
                }

                //ThreadPool.QueueUserWorkItem(new WaitCallback(ClearSmoke), SmokePoints);

                //new Action(() => {
                    //使用 lock 块锁定共享资源，可以避免资源抢占！相见恨晚！！！
                    lock (GameMap.Ground)
                    {
                        new Thread(delegate () { ClearSmoke(SmokePoints, GameMap.Ground.Clone());}).Start();
                    }
                //}).Invoke();
                //多线程允许传入任意多个参数

                Mines.Remove(sender);
            }
        }

        //此标识表示方法被弃用
        [Obsolete]
        /// <summary>
        /// 炸弹爆炸之后，延时消散爆炸烟雾（会发生资源抢占异常！）
        /// </summary>
        /// <param name="Smoke">烟雾坐标数组</param>
        private  void ClearSmoke(object SmokePoints)
        {
            Thread.Sleep(300);
            using (Graphics UnityGraphics = this.CreateGraphics())
                foreach (Cell SmokePoint in (List<Cell>) SmokePoints)
                {
                    GameMap.MapCellsClone[SmokePoint.TabelLocation.Y, SmokePoint.TabelLocation.X] = Map.CellType.Ground;
                    new Action(() => {
                        UnityGraphics.DrawImage(GameMap.Ground.Clone(new Rectangle(SmokePoint.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), SmokePoint.Location);
                    }).Invoke();
                }
        }

        /// <summary>
        /// 炸弹爆炸之后，延时消散爆炸烟雾
        /// </summary>
        /// <param name="Smoke">烟雾坐标数组</param>
        /// <param name="MapGround">填充背景用的Ground副本</param>
        private void ClearSmoke(object SmokePoints,object MapGround)
        {
            using (Graphics UnityGraphics = this.CreateGraphics())
            {
                //起爆时检查伤害
                if (CheckBlast((List<Cell>)SmokePoints, UnityGraphics)) return;
            
                Thread.Sleep(300);

                //硝烟散去时检查伤害
                if (CheckBlast((List<Cell>)SmokePoints, UnityGraphics)) return;

                foreach (Cell SmokePoint in (List<Cell>)SmokePoints)
                {
                    //烟雾消散之后才认为Wall被炸成了Ground，防止多个炸弹联动爆炸时会穿透
                    GameMap.MapCellsClone[SmokePoint.TabelLocation.Y, SmokePoint.TabelLocation.X] = Map.CellType.Ground;
                    new Action(() =>
                    {
                        UnityGraphics.DrawImage(((Bitmap)MapGround).Clone(new Rectangle(SmokePoint.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), SmokePoint.Location);
                    }).Invoke();
                }
            }
            GC.Collect();
        }

        /// <summary>
        /// 检查炸弹爆炸时的伤害
        /// </summary>
        /// <returns>是否炸到玩家</returns>
        private bool CheckBlast(List<Cell> SmokePoints,Graphics UnityGraphics)
        {
            int EnemyIndex = 0;
            while (EnemyIndex < EnemyList.Count)
            {
                if (((List<Cell>)SmokePoints).FirstOrDefault(X => new Rectangle(X.Location,GameMap.CellSize).IntersectsWith(new Rectangle(EnemyList[EnemyIndex].Location,GameMap.CellSize))) != null)
                {
                    Debug.Print("敌人 {0} : {1},{2} 被炸伤，退出战场！剩余敌人总数：{3}", EnemyIndex, EnemyList[EnemyIndex].TabelLocation.X, EnemyList[EnemyIndex].TabelLocation.Y, EnemyList.Count - 1);
                    lock(EnemyDeadCellImage)
                        UnityGraphics.DrawImageUnscaled(EnemyDeadCellImage, EnemyList[EnemyIndex].Location);
                    EnemyList[EnemyIndex].Dispose();//结束敌人的巡逻线程，否则不会释放内存
                    EnemyList[EnemyIndex].Patrol -= new EnemyModel.PatrolEventHander(EnemyPatrol);
                    EnemyList.RemoveAt(EnemyIndex);
                }
                else
                    EnemyIndex++;
            }
            GC.Collect();

            if (((List<Cell>)SmokePoints).FirstOrDefault(X => new Rectangle(X.Location, GameMap.CellSize).IntersectsWith(new Rectangle(Player.Location, GameMap.CellSize))) != null)
            {
                Debug.Print("玩家被炸弹炸伤，重新开始游戏！");
                UnityGraphics.DrawImage(UnityResource.Player_Lose, new Rectangle(Player.Location, GameMap.CellSize));
                MessageBox.Show("玩家被炸弹炸伤！游戏结束！");
                ResetGame();
                return true;
            }
            else
                return false;
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //需要卸载敌人线程，否则会出错
            foreach (EnemyModel Enemy in EnemyList)
            {
                Enemy.Dispose();
                Enemy.Patrol -= new EnemyModel.PatrolEventHander(EnemyPatrol);
            }
            EnemyList.Clear();
        }
    }
}
