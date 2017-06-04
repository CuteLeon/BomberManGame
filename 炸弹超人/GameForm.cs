﻿using System;
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
        /// <summary>
        /// 拉伸后的玩家图像（节省多次拉伸计算资源）
        /// </summary>
        private Bitmap PlayerCellImage;
        /// <summary>
        /// 拉伸后的炸弹图像（节省多次拉伸计算资源）
        /// </summary>
        private Bitmap MineCellImage;
        /// <summary>
        /// 拉伸后的敌人图像（节省多次拉伸计算资源）
        /// </summary>
        private Bitmap EnemyCellImage;
        /// <summary>
        /// 拉伸后的烟雾图像（节省多次拉伸计算资源）
        /// </summary>
        private Bitmap SmokeCellImage;


        public GameForm()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            
            this.SetBounds(0,0,Screen .PrimaryScreen .Bounds .Width ,Screen .PrimaryScreen .Bounds .Height );
            GameMap = new Map(Screen.PrimaryScreen.Bounds.Size);
            //计算拉伸后的玩家、炸弹、敌人、烟雾图像
            PlayerCellImage = new Bitmap(UnityResource.Player, GameMap.CellSize);
            MineCellImage = new Bitmap(UnityResource.Mine, GameMap.CellSize);
            EnemyCellImage = new Bitmap(UnityResource.Enemy, GameMap.CellSize);
            SmokeCellImage = new Bitmap(UnityResource.Smoke, GameMap.CellSize);
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
                        if (Player.PlaceBomb())
                        {
                            Mines.Add(new MineModel(GameMap, Player.TabelLocation));
                            Mines.Last().Blast += new MineModel.BlastEventHander(BombBlast);
                            using (Graphics UnityGraphics = this.CreateGraphics())
                            {
                                GameMap.MapCellsClone[Player.TabelLocation.Y, Player.TabelLocation.X] = Map.CellType.Mine;
                                UnityGraphics.DrawImageUnscaled(MineCellImage,Mines.Last().Location);
                                UnityGraphics.DrawImageUnscaled(PlayerCellImage, Player.Location);
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
            using (Graphics UnityGraphics = this.CreateGraphics())
            {
                GameMap.ResetMap();
                UnityGraphics.DrawImageUnscaled(GameMap.Ground, Point.Empty);
                UnityGraphics.DrawImageUnscaled(GameMap.DrawWalls(), Point.Empty);

                Player = new PlayerModel(GameMap,new Point(1, 1));
                UnityGraphics.DrawImageUnscaled(PlayerCellImage, Player.Location);

                Mines = new List<MineModel>();

                EnemyList = GameMap.CreateEnemy();
                foreach (Cell Enemy in EnemyList)
                    UnityGraphics.DrawImageUnscaled(EnemyCellImage, Enemy.Location);
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
                            UnityGraphics.DrawImageUnscaled(SmokeCellImage, DrawLocation);
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
                            UnityGraphics.DrawImageUnscaled(SmokeCellImage, DrawLocation);
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
                            UnityGraphics.DrawImageUnscaled(SmokeCellImage, DrawLocation);
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
                            UnityGraphics.DrawImageUnscaled(SmokeCellImage, DrawLocation);
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
                    
                    //多线程允许传入任意多个参数
                    new Thread(delegate () { ClearSmoke(SmokePoints, GameMap.Ground.Clone());}).Start();

                    Mines.Remove(sender);
                }
            }
            catch (Exception ex) { }
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
                    try
                    {
                        new Action(() => {
                            UnityGraphics.DrawImage(GameMap.Ground.Clone(new Rectangle(SmokePoint.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), SmokePoint.Location);
                        }).Invoke();
                    }
                    catch (Exception ex) { }
                }
        }

        /// <summary>
        /// 炸弹爆炸之后，延时消散爆炸烟雾
        /// </summary>
        /// <param name="Smoke">烟雾坐标数组</param>
        /// <param name="MapGround">填充背景用的Ground副本</param>
        private void ClearSmoke(object SmokePoints,object MapGround)
        {
            if (((List<Cell>)SmokePoints).FirstOrDefault(X => X.TabelLocation.Equals(Player.TabelLocation)) != null)
            {
                MessageBox.Show("玩家被炸弹炸伤！游戏结束！");
                ResetGame();
            }

            int EnemyIndex = 0;
            while (EnemyIndex < EnemyList.Count)
            {
                if (((List<Cell>)SmokePoints).FirstOrDefault(X => X.TabelLocation.Equals(EnemyList[EnemyIndex].TabelLocation)) != null)
                {
                    Debug.Print("敌人 {0},{1} 被炸伤，退出战场！剩余敌人总数：{2}", EnemyList[EnemyIndex].TabelLocation.X, EnemyList[EnemyIndex].TabelLocation.Y,EnemyList.Count.ToString());
                    EnemyList.RemoveAt(EnemyIndex);
                }
                else
                    EnemyIndex++;
            }
            
            Thread.Sleep(200);
            using (Graphics UnityGraphics = this.CreateGraphics())
                foreach (Cell SmokePoint in (List<Cell>)SmokePoints)
                {
                    //if ()
                    //烟雾消散之后才认为Wall被炸成了Ground，防止多个炸弹联动爆炸时会穿透
                    GameMap.MapCellsClone[SmokePoint.TabelLocation.Y, SmokePoint.TabelLocation.X] = Map.CellType.Ground;
                    try
                    {
                        new Action(() => {
                            UnityGraphics.DrawImage(((Bitmap)MapGround).Clone(new Rectangle(SmokePoint.Location, GameMap.CellSize), System.Drawing.Imaging.PixelFormat.Format32bppArgb), SmokePoint.Location);
                        }).Invoke();
                    }
                    catch (Exception ex) { }
                }
            GC.Collect();
        }
    }
}
