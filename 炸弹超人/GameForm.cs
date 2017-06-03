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

namespace 炸弹超人
{
    public partial class GameForm : Form
    {
        /// <summary>
        /// 敌人列表
        /// </summary>
        List<EnemyModel> EnemyList;
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

        public GameForm()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

            this.SetBounds(0,0,Screen .PrimaryScreen .Bounds .Width ,Screen .PrimaryScreen .Bounds .Height );
            GameMap = new Map(Screen.PrimaryScreen.Bounds.Size);
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
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate{
                                int Target = (Player.TabelLocation.Y) * GameMap.CellSize.Height + GameMap.PaddingSize.Height;
                                while (Player.Location.Y > Target+5)
                                {
                                    Thread.Sleep(10);
                                    //屏蔽掉会有很有趣的轨迹效果
                                    this.CreateGraphics().DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                    Player.Location.Offset(0, -5);
                                    this.CreateGraphics().DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                    DrawMines();
                                }
                                //屏蔽掉，会有很有趣的痕迹效果
                                this.CreateGraphics().DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                Player.Location = new Point(Player.Location.X,Target);
                                this.CreateGraphics().DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
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
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate {
                                int Target = (Player.TabelLocation.Y) * GameMap.CellSize.Height + GameMap.PaddingSize.Height;
                                while (Player.Location.Y < Target-5)
                                {
                                    Thread.Sleep(10);
                                    //屏蔽掉会有很有趣的轨迹效果
                                    this.CreateGraphics().DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                    Player.Location.Offset(0, 5);
                                    this.CreateGraphics().DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                    //DrawMines();
                                }
                                //屏蔽掉，会有很有趣的痕迹效果
                                this.CreateGraphics().DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                Player.Location = new Point(Player.Location.X, Target);
                                this.CreateGraphics().DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                PlayerMoveing = false;
                                DrawMines();
                            });
                        }
                        break;
                    }
                case Keys.Left:
                    {
                        if (GameMap.MapCellsClone[Player.TabelLocation.Y, Player.TabelLocation.X-1] == Map.CellType.Ground)
                        {
                            PlayerMoveing = true;
                            Player.TabelLocation.Offset(-1,0);
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate {
                                int Target = (Player.TabelLocation.X) * GameMap.CellSize.Width + GameMap.PaddingSize.Width;
                                while (Player.Location.X >Target+5)
                                {
                                    Thread.Sleep(10);
                                    //屏蔽掉会有很有趣的轨迹效果
                                    this.CreateGraphics().DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                    Player.Location.Offset(-5,0);
                                    this.CreateGraphics().DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                    //DrawMines();
                                }
                                //屏蔽掉，会有很有趣的痕迹效果
                                this.CreateGraphics().DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                Player.Location = new Point(Target, Player.Location.Y);
                                this.CreateGraphics().DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
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
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate {
                                int Target = (Player.TabelLocation.X) * GameMap.CellSize.Width + GameMap.PaddingSize.Width;
                                while (Player.Location.X < Target-5)
                                {
                                    Thread.Sleep(10);
                                    //屏蔽掉会有很有趣的轨迹效果
                                    this.CreateGraphics().DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                    Player.Location.Offset(5, 0);
                                    this.CreateGraphics().DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                    //DrawMines();
                                }
                                //屏蔽掉，会有很有趣的痕迹效果
                                this.CreateGraphics().DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                Player.Location = new Point(Target, Player.Location.Y);
                                this.CreateGraphics().DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                PlayerMoveing = false;
                                DrawMines();
                            });
                        }
                        break;
                    }

                case Keys.Space:
                    {
                        //按空格键释放炸弹
                        if (Player.PlaceBomb())
                        {
                            Mines.Add(new MineModel(GameMap,Player.TabelLocation));
                            Mines.Last().Blast +=new MineModel.BlastEventHander(BombBlast);

                            GameMap.MapCellsClone[Player.TabelLocation.Y, Player.TabelLocation.X] = Map.CellType.Mine;
                            this.CreateGraphics().DrawImage(UnityResource.Mine,new Rectangle(Mines.Last().Location,GameMap.CellSize));
                            this.CreateGraphics().DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
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
            GameMap.ResetMap();
            this.CreateGraphics().DrawImageUnscaled(GameMap.Ground, Point.Empty);
            this.CreateGraphics().DrawImageUnscaled(GameMap.DrawWalls(), Point.Empty);

            Player = new PlayerModel(GameMap,new Point(1, 1));
            this.CreateGraphics().DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));

            Mines = new List<MineModel>();

            EnemyList = GameMap.CreateEnemy();
            foreach (Cell Enemy in EnemyList)
                this.CreateGraphics().DrawImage(UnityResource.Enemy, new Rectangle(Enemy.Location, GameMap.CellSize));

            this.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.GameForm_KeyDown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GameForm_KeyDown);
        }

        /// <summary>
        /// 绘制炸弹图层
        /// </summary>
        private void DrawMines()
        {
            foreach (Cell Mine in Mines)
            {
                this.CreateGraphics().DrawImage(UnityResource.Mine, new Rectangle(Mine.Location, GameMap.CellSize));
            }
        }

        /// <summary>
        /// 检查是否触碰到敌人
        /// </summary>
        /// <returns></returns>
        private bool TouchEnemy()
        {
            return (EnemyList.Where(X => new Rectangle(X.Location, GameMap.CellSize).IntersectsWith(new Rectangle(Player.Location, GameMap.CellSize))).Count() > 0);
        }

        /// <summary>
        /// 炸弹爆炸事件
        /// </summary>
        private void BombBlast(MineModel sender)
        {
            try
            {
                Debug.Print("炸弹 {0} , {1} 起爆！", sender.TabelLocation.X, sender.TabelLocation.Y);

                List<Point> SmokePoints = new List<Point>();
                Player.BombCount++;
                SmokePoints.Add(sender.Location);
                GameMap.MapCellsClone[sender.TabelLocation.Y, sender.TabelLocation.X] = Map.CellType.Ground;
                this.CreateGraphics().DrawImage(UnityResource.Smoke, new Rectangle(sender.Location, GameMap.CellSize));
                //this.CreateGraphics().DrawImage(GameMap.Ground.Clone(new Rectangle(sender.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), sender.Location);
                Point DrawLocation;
                for (int Radius = 1; Radius <= Player.BlastRadius; Radius++)
                {
                    DrawLocation = new Point(sender.Location.X, sender.Location.Y - GameMap.CellSize.Height * Radius);
                    if (GameMap.MapCellsClone[sender.TabelLocation.Y - Radius, sender.TabelLocation.X] == Map.CellType.Wall)
                    {
                        SmokePoints.Add(DrawLocation);
                        GameMap.MapCellsClone[sender.TabelLocation.Y - Radius, sender.TabelLocation.X] = Map.CellType.Ground;
                        this.CreateGraphics().DrawImage(UnityResource.Smoke, new Rectangle(DrawLocation, GameMap.CellSize));
                        //this.CreateGraphics().DrawImage(GameMap.Ground.Clone(new Rectangle(DrawLocation, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), DrawLocation);
                        break;
                    }
                    else if (GameMap.MapCellsClone[sender.TabelLocation.Y - Radius, sender.TabelLocation.X] == Map.CellType.Ground)
                    {
                        SmokePoints.Add(DrawLocation);
                        this.CreateGraphics().DrawImage(UnityResource.Smoke, new Rectangle(DrawLocation, GameMap.CellSize));
                    }
                    else if (GameMap.MapCellsClone[sender.TabelLocation.Y - Radius, sender.TabelLocation.X] == Map.CellType.Stone)
                        break;
                    else if (GameMap.MapCellsClone[sender.TabelLocation.Y - Radius, sender.TabelLocation.X] == Map.CellType.Mine)
                    {
                        Mines.Where(X => X.TabelLocation.Equals(new Point(sender.TabelLocation.X, sender.TabelLocation.Y - Radius)))?.First().BlastNow();
                        break;
                    }
                }

                for (int Radius = 1; Radius <= Player.BlastRadius; Radius++)
                {
                    DrawLocation = new Point(sender.Location.X, sender.Location.Y + GameMap.CellSize.Height * Radius);
                    if (GameMap.MapCellsClone[sender.TabelLocation.Y + Radius, sender.TabelLocation.X] == Map.CellType.Wall)
                    {
                        SmokePoints.Add(DrawLocation);
                        GameMap.MapCellsClone[sender.TabelLocation.Y + Radius, sender.TabelLocation.X] = Map.CellType.Ground;
                        this.CreateGraphics().DrawImage(UnityResource.Smoke, new Rectangle(DrawLocation, GameMap.CellSize));
                        //this.CreateGraphics().DrawImage(GameMap.Ground.Clone(new Rectangle(DrawLocation, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), DrawLocation);
                        break;
                    }
                    else if (GameMap.MapCellsClone[sender.TabelLocation.Y + Radius, sender.TabelLocation.X] == Map.CellType.Ground)
                    {
                        SmokePoints.Add(DrawLocation);
                        this.CreateGraphics().DrawImage(UnityResource.Smoke, new Rectangle(DrawLocation, GameMap.CellSize));
                    }
                    else if (GameMap.MapCellsClone[sender.TabelLocation.Y + Radius, sender.TabelLocation.X] == Map.CellType.Stone)
                        break;
                    else if (GameMap.MapCellsClone[sender.TabelLocation.Y + Radius, sender.TabelLocation.X] == Map.CellType.Mine)
                    {
                        Mines.Where(X => X.TabelLocation.Equals(new Point(sender.TabelLocation.X, sender.TabelLocation.Y + Radius)))?.First().BlastNow();
                        break;
                    }
                }

                for (int Radius = 1; Radius <= Player.BlastRadius; Radius++)
                {
                    DrawLocation = new Point(sender.Location.X - GameMap.CellSize.Width * Radius, sender.Location.Y);
                    if (GameMap.MapCellsClone[sender.TabelLocation.Y, sender.TabelLocation.X - Radius] == Map.CellType.Wall)
                    {
                        SmokePoints.Add(DrawLocation);
                        GameMap.MapCellsClone[sender.TabelLocation.Y, sender.TabelLocation.X - Radius] = Map.CellType.Ground;
                        this.CreateGraphics().DrawImage(UnityResource.Smoke, new Rectangle(DrawLocation, GameMap.CellSize));
                        //this.CreateGraphics().DrawImage(GameMap.Ground.Clone(new Rectangle(DrawLocation, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), DrawLocation);
                        break;
                    }
                    else if (GameMap.MapCellsClone[sender.TabelLocation.Y, sender.TabelLocation.X - Radius] == Map.CellType.Ground)
                    {
                        SmokePoints.Add(DrawLocation);
                        this.CreateGraphics().DrawImage(UnityResource.Smoke, new Rectangle(DrawLocation, GameMap.CellSize));
                    }
                    else if (GameMap.MapCellsClone[sender.TabelLocation.Y, sender.TabelLocation.X - Radius] == Map.CellType.Stone)
                        break;
                    else if (GameMap.MapCellsClone[sender.TabelLocation.Y, sender.TabelLocation.X - Radius] == Map.CellType.Mine)
                    {
                        Mines.Where(X => X.TabelLocation.Equals(new Point(sender.TabelLocation.X - Radius, sender.TabelLocation.Y)))?.First().BlastNow();
                        break;
                    }
                }

                for (int Radius = 1; Radius <= Player.BlastRadius; Radius++)
                {
                    DrawLocation = new Point(sender.Location.X + GameMap.CellSize.Width * Radius, sender.Location.Y);
                    if (GameMap.MapCellsClone[sender.TabelLocation.Y, sender.TabelLocation.X + Radius] == Map.CellType.Wall)
                    {
                        SmokePoints.Add(DrawLocation);
                        GameMap.MapCellsClone[sender.TabelLocation.Y, sender.TabelLocation.X + Radius] = Map.CellType.Ground;
                        this.CreateGraphics().DrawImage(UnityResource.Smoke, new Rectangle(DrawLocation, GameMap.CellSize));
                        //this.CreateGraphics().DrawImage(GameMap.Ground.Clone(new Rectangle(DrawLocation, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), DrawLocation);
                        break;
                    }
                    else if (GameMap.MapCellsClone[sender.TabelLocation.Y, sender.TabelLocation.X + Radius] == Map.CellType.Ground)
                    {
                        SmokePoints.Add(DrawLocation);
                        this.CreateGraphics().DrawImage(UnityResource.Smoke, new Rectangle(DrawLocation, GameMap.CellSize));
                    }
                    else if (GameMap.MapCellsClone[sender.TabelLocation.Y, sender.TabelLocation.X + Radius] == Map.CellType.Stone)
                        break;
                    else if (GameMap.MapCellsClone[sender.TabelLocation.Y, sender.TabelLocation.X + Radius] == Map.CellType.Mine)
                    {
                        Mines.Where(X => X.TabelLocation.Equals(new Point(sender.TabelLocation.X + Radius, sender.TabelLocation.Y)))?.First().BlastNow();
                        break;
                    }
                }
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClearSmoke), SmokePoints);

                Mines.Remove(sender);
            }
            catch (Exception ex) { }
        }


        /// <summary>
        /// 炸弹爆炸之后，延时消散爆炸烟雾
        /// </summary>
        /// <param name="Smoke">烟雾坐标数组</param>
        private void ClearSmoke(object SmokePoints)
        {
            Thread.Sleep(300);
            foreach (Point SmokePoint in (List < Point >) SmokePoints)
            {
                try
                {
                    this.CreateGraphics().DrawImage(GameMap.Ground.Clone(new Rectangle(SmokePoint, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), SmokePoint);
                }
                catch (Exception ex) { }
            }
        }
    }
}
