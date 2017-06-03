using System.Drawing;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 炸弹超人
{
    class Map
    {
        /// <summary>
        /// 地图元素类型
        /// </summary>
        public enum CellType
        {
            Ground=0,//地面
            Stone=1,//石头
            Wall=2//墙
        }
        /// <summary>
        /// 固定的游戏地图元素表格宽度
        /// </summary>
        private const int TabelWidth = 23;
        /// <summary>
        /// 固定的游戏地图元素表格高度
        /// </summary>
        private const int TabelHeight = 13;
        /// <summary>
        /// 地图元素类型二维数组
        /// </summary>
        public CellType[,] MapCells = new CellType[TabelHeight,TabelWidth];
        /// <summary>
        /// 只读属性：在构造对象时根据世界尺寸和地图元素表壳尺寸计算元素尺寸
        /// </summary>
        private readonly Size CellSize;
        /// <summary>
        /// 只读属性：在构造对象时根据世界尺寸和地图元素表壳尺寸计算地图与屏幕间隔
        /// </summary>
        public readonly Size PaddingSize;
        /// <summary>
        /// 私有成员：记录游戏世界的尺寸
        /// </summary>
        private Size WorldSize;
        /// <summary>
        /// 墙元素列表
        /// </summary>
        public List<Cell> Walls;
        /// <summary>
        /// 空格列表
        /// </summary>
        private List<Cell> EmptyCell=new List<Cell>();
        private List<Cell> EmptyCellClone = new List<Cell>();
        /// <summary>
        /// 私有位图成员：地面（避免多次绘制，节省计算资源）
        /// </summary>
        public Bitmap Ground;
        /// <summary>
        /// 私有位图成员：石头（避免多次绘制，节省计算资源）
        /// </summary>
        private Bitmap StonesBitmap;
        /// <summary>
        /// 拉伸后的石头图像（避免多次拉伸，节省计算资源）
        /// </summary>
        private readonly Bitmap StoneCellBitmap;
        /// <summary>
        /// 拉伸后的墙图像（避免多次拉伸，节省计算资源）
        /// </summary>
        private readonly Bitmap WallCellBitmap;
        /// <summary>
        /// 随机数生成器
        /// </summary>
        private Random UnityRandom = new Random();
        /// <summary>
        /// 通关门在Tabel里的坐标
        /// </summary>
        private Cell DoorLocation;
        /// <summary>
        /// 奖励在Tabel里的坐标
        /// </summary>
        private Cell GiftLocation;

        //todo:构造时计算出拉伸后的stone 和wall 防止 多次drawimage时拉伸计算

        /// <summary>
        /// 隐藏的默认构造函数
        /// </summary>
        private Map(){}
        /// <summary>
        /// 公开的构造函数（需要传入游戏世界的尺寸）
        /// </summary>
        /// <param name="WorldSize"></param>
        public Map(Size worldSize)
        {
            WorldSize = worldSize;

            //计算：CellSize;
            int cellSize = Math.Min(WorldSize.Width / TabelWidth, WorldSize.Height / TabelHeight);
            CellSize = new Size(cellSize, cellSize);
            StoneCellBitmap = new Bitmap(UnityResource.Stone, CellSize);
            WallCellBitmap = new Bitmap(UnityResource.Wall, CellSize);
            //Debug.Print(CellSize.ToString());

            //计算：PaddingSize
            PaddingSize = new Size((WorldSize.Width-CellSize.Width * TabelWidth)/2,
                (WorldSize.Height - CellSize.Height * TabelHeight) / 2);
            //Debug.Print(PaddingSize.ToString());

            //绘制默认地面、石头和墙
            Ground = new Bitmap(UnityResource.GameGround ,WorldSize);
            StonesBitmap = CreateStones();
            CreateWalls();
            //绘制石头层
            using (Graphics MapGraphics = Graphics.FromImage(Ground))
                MapGraphics.DrawImage(StonesBitmap, Point.Empty);
        }

        /// <summary>
        /// 创建石头层图像
        /// </summary>
        /// <returns>石头层图像</returns>
        private Bitmap CreateStones()
        {
            Bitmap StonesBitmap = new Bitmap(WorldSize.Width, WorldSize.Height);
            using (Graphics StonesGraphics = Graphics.FromImage(StonesBitmap))
            {
                for (int X = 0; X < TabelWidth; X++)
                {
                    MapCells[0,X]= CellType.Stone;
                    MapCells[TabelHeight-1,X]= CellType.Stone;
                }
                for (int Y = 1; Y < TabelHeight-1; Y++)
                {
                    MapCells[Y,0] = CellType.Stone;
                    MapCells[Y,TabelWidth-1] = CellType.Stone;
                }
                for (int Y = 2; Y < TabelHeight - 1; Y+=2)
                    for (int X = 2; X < TabelWidth - 1; X+=2)
                        MapCells[Y, X] = CellType.Stone;

                Point DrawLocation = new Point(PaddingSize);
                EmptyCell = new List<Cell>();
                for (int Y = 0; Y < TabelHeight; Y++)
                {
                    for (int X = 0; X < TabelWidth; X++)
                    {
                        if (MapCells[Y, X] == CellType.Stone)
                            StonesGraphics.DrawImage(StoneCellBitmap, DrawLocation.X, DrawLocation.Y, CellSize.Width, CellSize.Height);
                        else
                            EmptyCell.Add(new Cell(DrawLocation,new Point(Y,X)));
                        DrawLocation.Offset(CellSize.Width,0);
                    }
                    DrawLocation.Offset(PaddingSize.Width - DrawLocation.X, 0);
                    DrawLocation.Offset(0,CellSize.Height);
                }
            }
            return StonesBitmap;
        }

        /// <summary>
        /// 创建墙层元素
        /// </summary>
        private void CreateWalls()
        {
            //根据空元素数量计算墙元素格式
            int WallsCount = (int)(((TabelWidth - 2) * (TabelHeight - 2) - (TabelWidth - 3) / 2 * (TabelHeight - 3) / 2) * 0.3 - 1);
            int Index;
            EmptyCellClone = new List<Cell>();
            EmptyCellClone.AddRange(EmptyCell);
            //删除左上角三个空格，因为那里是玩家出现的地方，不允许出现墙元素
            EmptyCellClone.RemoveAll(x=> x.TabelLocation == new Point(1, 1) || x.TabelLocation == new Point(2, 1) || x.TabelLocation == new Point(1, 2));

            Walls = new List<Cell>();
            //从空元素列表随机选取作为墙元素
            while (WallsCount > 0)
            {
                Index = UnityRandom.Next(EmptyCellClone.Count);
                Walls.Add(EmptyCellClone[Index]);
                EmptyCellClone.RemoveAt(Index);
                WallsCount--;
            }
            //从墙元素列表随机选取作为通关门和奖励
            List<Cell> WallsClone = new List<Cell>();
            WallsClone.AddRange(Walls);
            Index = UnityRandom.Next(WallsClone.Count);
            DoorLocation = WallsClone[Index];
            WallsClone.RemoveAt(Index);
            Index = UnityRandom.Next(WallsClone.Count);
            GiftLocation = WallsClone[Index];
            WallsClone.Clear();
            WallsClone = null;
        }

        /// <summary>
        /// 绘制墙层图像
        /// </summary>
        /// <returns>墙层图像</returns>
        public Bitmap DrawWalls()
        {
            Bitmap WallsBitmap = new Bitmap(WorldSize.Width, WorldSize.Height);
            using (Graphics WallsGraphics = Graphics.FromImage(WallsBitmap))
            {
                foreach (Cell Wall in Walls)
                {
                    WallsGraphics.DrawImage(WallCellBitmap, Wall.Location.X, Wall.Location.Y, CellSize.Width, CellSize.Height);
                }
            }
            return WallsBitmap;
        }

        /// <summary>
        /// 创建敌人
        /// </summary>
        /// <returns>敌人列表坐标</returns>
        private List<Point> CreateEnemy()
        {
            List<Point> EnemyList = new List<Point>();
            int EnemyCount = 5;
            int Index;
            while (EnemyCount > 0)
            {
                Index = UnityRandom.Next(EmptyCellClone.Count);
                EnemyList.Add(EmptyCellClone[Index].TabelLocation);
                EnemyCount--;
            }
            return EnemyList;
        }

        /// <summary>
        /// 重置游戏地图
        /// </summary>
        public void ResetMap()
        {
            CreateWalls();
            GC.Collect();
        }
    }
}
