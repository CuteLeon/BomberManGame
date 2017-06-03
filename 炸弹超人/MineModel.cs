using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 炸弹超人
{
    class MineModel:Cell
    {
        /// <summary>
        /// 爆炸事件委托
        /// </summary>
        public delegate void BlastEventHander(MineModel sender);
        /// <summary>
        /// 爆炸事件
        /// </summary>
        public event BlastEventHander Blast;

        /// <summary>
        /// 继承自Cell类的构造函数
        /// </summary>
        /// <param name="GameMap"></param>
        /// <param name="tabelLocation"></param>
        public MineModel(Map GameMap, Point tabelLocation):base(GameMap,tabelLocation)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(delegate {
                //炸弹延缓5秒爆炸
                System.Threading.Thread.Sleep(3000);
                Blast(this);
            });
        }

    }
}
