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
        /// 允许同时放置的炸弹总数（默认为 1）
        /// </summary>
        private Byte BombCount=1;
        /// <summary>
        /// 继承自Cell类的构造函数
        /// </summary>
        /// <param name="GameMap"></param>
        /// <param name="tabelLocation"></param>
        public PlayerModel(Map GameMap, Point tabelLocation):base(GameMap,tabelLocation){}

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
