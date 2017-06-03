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
        /// 全局Graphics
        /// </summary>
        Graphics UnityGraphics;
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
        private List<Cell> Mines;
        /// <summary>
        /// 玩家角色正在移动线程中
        /// </summary>
        private bool PlayerMoveing;

        public GameForm()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

            this.SetBounds(0,0,Screen .PrimaryScreen .Bounds .Width ,Screen .PrimaryScreen .Bounds .Height );
            UnityGraphics = this.CreateGraphics();
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
                                    //屏蔽掉会有很有趣的轨迹效果
                                    //UnityGraphics.DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                    Player.Location.Offset(0, -5);
                                    Thread.Sleep(10);
                                    UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                    DrawMines();
                                }
                                //屏蔽掉，会有很有趣的痕迹效果
                                //UnityGraphics.DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                Player.Location = new Point(Player.Location.X,Target);
                                UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                PlayerMoveing = false;
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
                                    //屏蔽掉会有很有趣的轨迹效果
                                    //UnityGraphics.DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                    Player.Location.Offset(0, 5);
                                    Thread.Sleep(10);
                                    UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                    DrawMines();
                                }
                                //屏蔽掉，会有很有趣的痕迹效果
                                //UnityGraphics.DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                Player.Location = new Point(Player.Location.X, Target);
                                UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                PlayerMoveing = false;
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
                                    //屏蔽掉会有很有趣的轨迹效果
                                    //UnityGraphics.DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                    Player.Location.Offset(-5,0);
                                    Thread.Sleep(10);
                                    UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                    DrawMines();
                                }
                                //屏蔽掉，会有很有趣的痕迹效果
                                //UnityGraphics.DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                Player.Location = new Point(Target, Player.Location.Y);
                                UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                PlayerMoveing = false;
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
                                    //屏蔽掉会有很有趣的轨迹效果
                                    //UnityGraphics.DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                    Player.Location.Offset(5, 0);
                                    Thread.Sleep(10);
                                    UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                    DrawMines();
                                }
                                //屏蔽掉，会有很有趣的痕迹效果
                                //UnityGraphics.DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                Player.Location = new Point(Target, Player.Location.Y);
                                UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                PlayerMoveing = false;
                            });
                        }
                        break;
                    }

                case Keys.Space:
                    {
                        //按空格键释放炸弹
                        if (Player.PlaceBomb())
                        {
                            Mines.Add(new Cell(GameMap,Player.TabelLocation));
                            GameMap.MapCellsClone[Player.TabelLocation.Y, Player.TabelLocation.X] = Map.CellType.Mine;
                            UnityGraphics.DrawImage(UnityResource.Mine,new Rectangle(Mines.Last().Location,GameMap.CellSize));
                        }
                        break;
                    }
            }

            //刷新炸弹层
            DrawMines();
        }

        /// <summary>
        /// 初始化/重置游戏
        /// </summary>
        private void ResetGame()
        {
            GameMap.ResetMap();
            UnityGraphics.DrawImageUnscaled(GameMap.Ground, Point.Empty);
            UnityGraphics.DrawImageUnscaled(GameMap.DrawWalls(), Point.Empty);

            Player = new PlayerModel(GameMap,new Point(1, 1));
            UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));

            Mines = new List<Cell>();

            EnemyList = GameMap.CreateEnemy();
            foreach (Cell Enemy in EnemyList)
                UnityGraphics.DrawImage(UnityResource.Enemy, new Rectangle(Enemy.Location, GameMap.CellSize));

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
                UnityGraphics.DrawImage(UnityResource.Mine, new Rectangle(Mine.Location, GameMap.CellSize));
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
    }
}
