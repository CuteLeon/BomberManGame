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
        /// 爆炸杀伤半径（默认为 1）
        /// </summary>
        public byte BlastRadius = 3;
        /// <summary>
        /// 允许同时放置的炸弹总数（默认为 1）
        /// </summary>
        public Byte BombCount=3;
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
        /// <param name="GameMap"></param>
        /// <param name="tabelLocation"></param>
        public PlayerModel(Map GameMap, Point tabelLocation):base(GameMap,tabelLocation){
            Ground = (Bitmap)GameMap.Ground.Clone();
        }

        /// <summary>
        /// 角色移动
        /// </summary>
        public void Move()
        {

        }

        /// <summary>
        /// 放置炸弹
        /// </summary>
        /// <returns>是否允许放置</returns>
        public bool PlaceBomb()
        {
            if (BombCount > 0)
            {
                BombCount--;
                return true;
            }
            else
                return false;
        }
    }
}
