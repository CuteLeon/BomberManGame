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
        List<Cell> EnemyList;
        /// <summary>
        /// 游戏地图对象
        /// </summary>
        private Map GameMap;
        /// <summary>
        /// 玩家对象
        /// </summary>
        private Cell Player;
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
            Player = new Cell(new Point( GameMap.PaddingSize.Width + GameMap.CellSize.Width,GameMap.PaddingSize.Height+ GameMap.CellSize.Height),new Point(1,1));
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            this.BackgroundImage = GameMap.Ground;
            GC.Collect();
        }

        private void GameForm_Click(object sender, EventArgs e)
        {
            GameMap.ResetMap();
            UnityGraphics.DrawImage(GameMap.Ground, Point.Empty);
            UnityGraphics.DrawImage(GameMap.DrawWalls(), Point.Empty);

            EnemyList = GameMap.CreateEnemy();
            foreach (Cell Enemy in EnemyList)
                UnityGraphics.DrawImage(UnityResource.Enemy, new Rectangle(Enemy.Location, GameMap.CellSize));

            UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
        }

        private void GameForm_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (PlayerMoveing) return;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    {
                        if (GameMap.MapCells[Player.TabelLocation.Y - 1, Player.TabelLocation.X] == Map.CellType.Ground)
                        {
                            PlayerMoveing = true;
                            Player.TabelLocation.Offset(0, -1);
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate{
                                while (Player.Location.Y > (Player.TabelLocation.Y) * GameMap.CellSize.Height + GameMap.PaddingSize.Height)
                                {
                                    UnityGraphics.DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                    Player.Location.Offset(0, -5);
                                    Thread.Sleep(10);
                                    UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                }
                                UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                PlayerMoveing = false;
                            });
                        }
                        break;
                    }
                case Keys.Down:
                    {
                        if (GameMap.MapCells[Player.TabelLocation.Y + 1, Player.TabelLocation.X] == Map.CellType.Ground)
                        {
                            PlayerMoveing = true;
                            Player.TabelLocation.Offset(0, 1);
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate {
                                while (Player.Location.Y < (Player.TabelLocation.Y) * GameMap.CellSize.Height + GameMap.PaddingSize.Height)
                                {
                                    UnityGraphics.DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                    Player.Location.Offset(0, 5);
                                    Thread.Sleep(10);
                                    UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                }
                                UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                PlayerMoveing = false;
                            });
                        }
                        break;
                    }
                case Keys.Left:
                    {
                        if (GameMap.MapCells[Player.TabelLocation.Y, Player.TabelLocation.X-1] == Map.CellType.Ground)
                        {
                            PlayerMoveing = true;
                            Player.TabelLocation.Offset(-1,0);
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate {
                                while (Player.Location.X > (Player.TabelLocation.X) * GameMap.CellSize.Width + GameMap.PaddingSize.Width)
                                {
                                    UnityGraphics.DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                    Player.Location.Offset(-5,0);
                                    Thread.Sleep(10);
                                    UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                }
                                UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                PlayerMoveing = false;
                            });
                        }
                        break;
                    }
                case Keys.Right:
                    {
                        if (GameMap.MapCells[Player.TabelLocation.Y, Player.TabelLocation.X + 1] == Map.CellType.Ground)
                        {
                            PlayerMoveing = true;
                            Player.TabelLocation.Offset(1, 0);
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate {
                                while (Player.Location.X < (Player.TabelLocation.X) * GameMap.CellSize.Width + GameMap.PaddingSize.Width)
                                {
                                    UnityGraphics.DrawImage(GameMap.Ground.Clone(new Rectangle(Player.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), Player.Location);
                                    Player.Location.Offset(5, 0);
                                    Thread.Sleep(10);
                                    UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                }
                                UnityGraphics.DrawImage(UnityResource.Player, new Rectangle(Player.Location, GameMap.CellSize));
                                PlayerMoveing = false;
                            });
                        }
                        break;
                    }
            }
        }
    }
}
