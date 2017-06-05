using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 炸弹超人
{
    class PlayerModel:Cell
    {
        /// <summary>
        /// 记录玩家所在地图对象
        /// </summary>
        Map GameMap;
        /// <summary>
        /// 默认炸弹总数
        /// </summary>
        public byte DefaultBombCount = 3;
        /// <summary>
        /// 默认坐标
        /// </summary>
        public static Cell DefaultLocation;
        /// <summary>
        /// 爆炸杀伤半径
        /// </summary>
        public byte BlastRadius = 2;
        /// <summary>
        /// 允许同时放置的炸弹总数
        /// </summary>
        public byte BombLeftCount;
        /// <summary>
        /// 游戏地图克隆，防止清除轨迹时资源被抢占
        /// </summary>
        public Bitmap Ground;

        /// <summary>
        /// 被隐藏的构造函数（禁止访问）
        /// </summary>
        /// <param name="location"></param>
        /// <param name="tabelLocation"></param>
        private PlayerModel(Point location, Point tabelLocation) : base(location,tabelLocation) { }

        /// <summary>
        /// 继承自Cell类的构造函数
        /// </summary>
        /// <param name="gameMap"></param>
        /// <param name="tabelLocation"></param>
        public PlayerModel(Map gameMap, Point tabelLocation):base(gameMap,tabelLocation){
            GameMap = gameMap;
            Ground = (Bitmap)gameMap.Ground.Clone();
            DefaultLocation = new Cell(this.Location,tabelLocation);
            BombLeftCount = DefaultBombCount;

            //最大火力作弊！
            BlastRadius =20;
        }

        /// <summary>
        /// 检查是否允许放置炸弹
        /// </summary>
        /// <returns>是否允许放置</returns>
        public bool CanPlaceBomb()
        {
            return (BombLeftCount > 0);
        }

        /// <summary>
        /// 放置炸弹
        /// </summary>
        public void PlaceBomb()
        {
            BombLeftCount--;
        }

        /// <summary>
        /// 初始化玩家
        /// </summary>
        public void ResetPlayer()
        {
            BombLeftCount = DefaultBombCount;
            Location = PlayerModel.DefaultLocation.Location;
            TabelLocation = PlayerModel.DefaultLocation.TabelLocation;
            System.Diagnostics.Debug.Print(DefaultLocation.Location.ToString());
        }
    }
}
