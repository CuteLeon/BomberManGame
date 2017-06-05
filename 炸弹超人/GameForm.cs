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
//todo:爆炸后敌人在爆炸范围外的图像无法擦除

namespace 炸弹超人
{
    public partial class GameForm : Form
    {
        /// <summary>
        /// 奖励物品坐标
        /// </summary>
        private Cell GiftLocation;
        private Bitmap GiftCellImage;
        /// <summary>
        /// 通关门坐标
        /// </summary>
        private Cell DoorLocation;
        private Bitmap DoorCellImage;
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
        private List<MineModel> MineList = new List<MineModel>();
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
            Player = new PlayerModel(GameMap, new Point(1, 1));

            //计算拉伸后的图像
            PlayerCellImage = new Bitmap(UnityResource.Player, GameMap.CellSize);
            MineCellImage = new Bitmap(UnityResource.Mine, GameMap.CellSize);
            EnemyDeadCellImage= new Bitmap(UnityResource.Enemy_Dead, GameMap.CellSize);
            SmokeCellImage = new Bitmap(UnityResource.Smoke, GameMap.CellSize);
            WallBrokenCellImage = new Bitmap(UnityResource.Wall_Broken, GameMap.CellSize);
            GiftCellImage = new Bitmap(UnityResource.Gift, GameMap.CellSize);
            DoorCellImage = new Bitmap(UnityResource.Door,GameMap.CellSize);
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            this.Icon = UnityResource.BomberMan;
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
                                DrawMinesAndGiftAndDoor();
                                CheckPlayerArriveGiftOrDoor();
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
                                DrawMinesAndGiftAndDoor();
                                CheckPlayerArriveGiftOrDoor();
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
                                DrawMinesAndGiftAndDoor();
                                CheckPlayerArriveGiftOrDoor();
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
                                DrawMinesAndGiftAndDoor();
                                CheckPlayerArriveGiftOrDoor();
                            });
                        }
                        break;
                    }
                case Keys.Enter:
                case Keys.Space:
                    {
                        //按空格键释放炸弹
                        if (Player.CanPlaceBomb())
                        {
                            if (MineList.FirstOrDefault(X => X.TabelLocation.Equals(Player.TabelLocation)) == null)
                            {
                                GameMap.MapCellsClone[Player.TabelLocation.Y, Player.TabelLocation.X] = Map.CellType.Mine;
                                Player.PlaceBomb();
                                MineList.Add(new MineModel(GameMap, Player.TabelLocation));
                                MineList.Last().Blast += new MineModel.BlastEventHander(BombBlast);
                                using (Graphics UnityGraphics = this.CreateGraphics())
                                {
                                    UnityGraphics.DrawImageUnscaled(MineCellImage,MineList.Last().Location);
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
            GiftLocation = null;
            DoorLocation = null;

            foreach (EnemyModel Enemy in EnemyList)
            {
                Enemy.Dispose();
                Enemy.Patrol -=new EnemyModel.PatrolEventHander ( EnemyPatrol);
            }

            foreach (MineModel Mine in MineList)
            {
                Mine.Dispose();
                Mine.Blast -= new MineModel.BlastEventHander(BombBlast);
            }

            using (Graphics UnityGraphics = this.CreateGraphics())
            {
                Player.ResetPlayer();

                GameMap.ResetMap();
                UnityGraphics.DrawImageUnscaled(GameMap.Ground, Point.Empty);
                UnityGraphics.DrawImageUnscaled(GameMap.DrawWalls(), Point.Empty);

                UnityGraphics.DrawImageUnscaled(PlayerCellImage, Player.Location);

                MineList = new List<MineModel>();

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
        private void DrawMinesAndGiftAndDoor()
        {
            using (Graphics UnityGraphics = this.CreateGraphics())
            {
                //绘制炸弹
                foreach (Cell Mine in MineList)
                    UnityGraphics.DrawImageUnscaled(MineCellImage, Mine.Location);

                //绘制奖励和通关门
                if (GiftLocation!=null) UnityGraphics.DrawImageUnscaled(GiftCellImage,GiftLocation.Location);
                if(DoorLocation!=null) UnityGraphics.DrawImageUnscaled(DoorCellImage, DoorLocation.Location);
            }

            GC.Collect();
        }

        /// <summary>
        /// 检查玩家到达奖励或者通关门
        /// </summary>
        private void CheckPlayerArriveGiftOrDoor()
        {
            if (GiftLocation != null) if (Player.TabelLocation.Equals(GiftLocation.TabelLocation))
                {
                    //获得奖励！
                    Player.BlastRadius ++;
                    Player.BombLeftCount ++;
                    Player.DefaultBombCount++;
                    GiftLocation = null;
                }
            if (DoorLocation != null) if (Player.TabelLocation.Equals(DoorLocation.TabelLocation))
                {
                    //到达通关门
                    if (EnemyList.Count == 0)
                    {
                        //游戏胜利！！！
                        MessageBox.Show(this,"胜利！游戏结束！");
                        ResetGame();
                    }
                }
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
                Rectangle TempRectangle;
                TempRectangle = new Rectangle(Sender.Location, GameMap.CellSize);
                TempRectangle.Intersect(new Rectangle (Player.Location,GameMap.CellSize));
                if ((double)(TempRectangle.Width * TempRectangle.Height) / (double)(GameMap.CellSize.Width * GameMap.CellSize.Width) > 0.3)
                {
                    //必须回到this.Invoke()线程，才可以正常重置游戏！
                    this.Invoke(new Action(() =>
                    {
                        StopGame();

                        MessageBox.Show(this, "玩家被敌人撞伤！游戏结束！");
                        ResetGame();
                    }));
                    return;
                }
                //if (new Rectangle(Sender.Location, GameMap.CellSize).IntersectsWith(new Rectangle(Player.Location, GameMap.CellSize)))
                //{
                //    //必须回到this.Invoke()线程，才可以正常重置游戏！
                //    this.Invoke(new Action(()=> {
                //        MessageBox.Show(this,"玩家被敌人撞伤！游戏结束！");
                //        ResetGame();
                //    }));
                //    return;
                //}
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
                Player.BombLeftCount++;
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
                        CheckGiftAndDoorUnderWall(LocationInTabel);

                        SmokePoints.Add(new Cell(DrawLocation,LocationInTabel));
                        UnityGraphics.DrawImageUnscaled(WallBrokenCellImage, DrawLocation);
                        //伤害穿透作弊！
                        //break;
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
                        MineList.Where(X => X.TabelLocation.Equals(LocationInTabel))?.First().BlastNow();
                        //伤害穿透作弊！
                        //break;
                    }
                }

                for (int Radius = 1; Radius <= Player.BlastRadius; Radius++)
                {
                    DrawLocation = new Point(sender.Location.X, sender.Location.Y + GameMap.CellSize.Height * Radius);
                    LocationInTabel = new Point(sender.TabelLocation.X,sender.TabelLocation.Y + Radius);
                    if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Wall)
                    {
                        CheckGiftAndDoorUnderWall(LocationInTabel);

                        SmokePoints.Add(new Cell(DrawLocation, LocationInTabel));
                        UnityGraphics.DrawImageUnscaled(WallBrokenCellImage, DrawLocation);
                        //伤害穿透作弊！
                        //break;
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
                        MineList.Where(X => X.TabelLocation.Equals(LocationInTabel))?.First().BlastNow();
                        //伤害穿透作弊！
                        //break;
                    }
                }

                for (int Radius = 1; Radius <= Player.BlastRadius; Radius++)
                {
                    DrawLocation = new Point(sender.Location.X - GameMap.CellSize.Width * Radius, sender.Location.Y);
                    LocationInTabel = new Point(sender.TabelLocation.X - Radius, sender.TabelLocation.Y);
                    if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Wall)
                    {
                        CheckGiftAndDoorUnderWall(LocationInTabel);

                        SmokePoints.Add(new Cell(DrawLocation, LocationInTabel));
                        UnityGraphics.DrawImageUnscaled(WallBrokenCellImage, DrawLocation);
                        //伤害穿透作弊！
                        //break;
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
                        MineList.Where(X => X.TabelLocation.Equals(LocationInTabel))?.First().BlastNow();
                        //伤害穿透作弊！
                        //break;
                    }
                }
                for (int Radius = 1; Radius <= Player.BlastRadius; Radius++)
                {
                    DrawLocation = new Point(sender.Location.X + GameMap.CellSize.Width * Radius, sender.Location.Y);
                    LocationInTabel = new Point(sender.TabelLocation.X + Radius, sender.TabelLocation.Y);
                    if (GameMap.MapCellsClone[LocationInTabel.Y, LocationInTabel.X] == Map.CellType.Wall)
                    {
                        CheckGiftAndDoorUnderWall(LocationInTabel);

                        SmokePoints.Add(new Cell(DrawLocation, LocationInTabel));
                        UnityGraphics.DrawImageUnscaled(WallBrokenCellImage, DrawLocation);
                        //伤害穿透作弊！
                        //break;
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
                        MineList.Where(X => X.TabelLocation.Equals(LocationInTabel))?.First().BlastNow();
                        //伤害穿透作弊！
                        //break;
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

                MineList.Remove(sender);
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
        /// 检查墙下面是否是奖励或通关门
        /// </summary>
        private void CheckGiftAndDoorUnderWall(Point LocationInTabel)
        {
            if (GiftLocation == null)
                if (LocationInTabel.Equals(GameMap.GiftLocation.TabelLocation))
                    GiftLocation = GameMap.GiftLocation;

            if (DoorLocation == null)
                if (LocationInTabel.Equals(GameMap.DoorLocation.TabelLocation))
                    DoorLocation = GameMap.DoorLocation;
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
            //DrawMinesAndGiftAndDoor();
            GC.Collect();
        }

        /// <summary>
        /// 检查炸弹爆炸时的伤害
        /// </summary>
        /// <returns>是否炸到玩家</returns>
        private bool CheckBlast(List<Cell> SmokePoints, Graphics UnityGraphics)
        {
            int EnemyIndex = 0;
            Rectangle TempRectangle;
            while (EnemyIndex < EnemyList.Count)
            {
                foreach (Cell SmokePoint in SmokePoints)
                {
                    TempRectangle = new Rectangle(EnemyList[EnemyIndex].Location, GameMap.CellSize);
                    TempRectangle.Intersect(new Rectangle(SmokePoint.Location, GameMap.CellSize));
                    if ((double)(TempRectangle.Width * TempRectangle.Height) / (double)(GameMap.CellSize.Width * GameMap.CellSize.Width) > 0.3)
                    {
                        lock (EnemyDeadCellImage)
                            UnityGraphics.DrawImageUnscaled(EnemyDeadCellImage, EnemyList[EnemyIndex].Location);
                        EnemyList[EnemyIndex].Dispose();//结束敌人的巡逻线程，否则不会释放内存
                        EnemyList[EnemyIndex].Patrol -= new EnemyModel.PatrolEventHander(EnemyPatrol);
                        EnemyList.RemoveAt(EnemyIndex);
                        EnemyIndex--;
                        break;
                    }
                }
                EnemyIndex++;

                //if (((List<Cell>)SmokePoints).FirstOrDefault(X => new Rectangle(X.Location,GameMap.CellSize).IntersectsWith(new Rectangle(EnemyList[EnemyIndex].Location,GameMap.CellSize))) != null)
                //{
                //    //Debug.Print("敌人 {0} : {1},{2} 被炸伤，退出战场！剩余敌人总数：{3}", EnemyIndex, EnemyList[EnemyIndex].TabelLocation.X, EnemyList[EnemyIndex].TabelLocation.Y, EnemyList.Count - 1);
                //    lock(EnemyDeadCellImage)
                //        UnityGraphics.DrawImageUnscaled(EnemyDeadCellImage, EnemyList[EnemyIndex].Location);
                //    EnemyList[EnemyIndex].Dispose();//结束敌人的巡逻线程，否则不会释放内存
                //    EnemyList[EnemyIndex].Patrol -= new EnemyModel.PatrolEventHander(EnemyPatrol);
                //    EnemyList.RemoveAt(EnemyIndex);
                //}
                //else
                //    EnemyIndex++;
            }
            GC.Collect();

            foreach (Cell SmokePoint in SmokePoints)
            {
                TempRectangle = new Rectangle(SmokePoint.Location, GameMap.CellSize);
                TempRectangle.Intersect(new Rectangle(Player.Location, GameMap.CellSize));
                if ((double)(TempRectangle.Width * TempRectangle.Height) / (double)(GameMap.CellSize.Width * GameMap.CellSize.Width) > 0.3)
                {
                    //Debug.Print("玩家被炸弹炸伤，重新开始游戏！");
                    UnityGraphics.DrawImage(UnityResource.Player_Lose, new Rectangle(Player.Location, GameMap.CellSize));
                    StopGame();
                    MessageBox.Show(this,"玩家被炸弹炸伤！游戏结束！");
                    ResetGame();
                    return true;
                }
            }
            return false;

            //if (((List<Cell>)SmokePoints).FirstOrDefault(X => new Rectangle(X.Location, GameMap.CellSize).IntersectsWith(new Rectangle(Player.Location, GameMap.CellSize))) != null)
            //{
            //    //Debug.Print("玩家被炸弹炸伤，重新开始游戏！");
            //    UnityGraphics.DrawImage(UnityResource.Player_Lose, new Rectangle(Player.Location, GameMap.CellSize));
            //    StopGame();
            //    MessageBox.Show("玩家被炸弹炸伤！游戏结束！");
            //    ResetGame();
            //    return true;
            //}
            //else
            //    return false;
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (EnemyModel Enemy in EnemyList)
            {
                Enemy.Dispose();
                Enemy.Patrol -= new EnemyModel.PatrolEventHander(EnemyPatrol);
            }
            EnemyList.Clear();
            foreach (MineModel Mine in MineList)
            {
                Mine.Dispose();
                Mine.Blast -= new MineModel.BlastEventHander(BombBlast);
            }
            MineList.Clear();
        }

        /// <summary>
        /// 停止游戏 (拆除炸弹 静止敌人)
        /// </summary>
        private void StopGame()
        {
            foreach (EnemyModel Enemy_In in EnemyList)
            {
                Enemy_In.Dispose();
                Enemy_In.Patrol -= new EnemyModel.PatrolEventHander(EnemyPatrol);
            }

            foreach (MineModel Mine in MineList)
            {
                Mine.Dispose();
                Mine.Blast -= new MineModel.BlastEventHander(BombBlast);
            }
        }
    }
}
