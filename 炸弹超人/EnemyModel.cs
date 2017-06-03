using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 炸弹超人
{
    class EnemyModel:Cell
    {
        /// <summary>
        /// 继承自Cell类的构造函数
        /// </summary>
        /// <param name="GameMap"></param>
        /// <param name="tabelLocation"></param>
        public EnemyModel(Map GameMap, Point tabelLocation):base(GameMap,tabelLocation){ }
    }
}
