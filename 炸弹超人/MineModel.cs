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
        /// 是否已经起爆
        /// </summary>
        private bool Blasted;
        /// <summary>
        /// 继承自Cell类的构造函数
        /// </summary>
        /// <param name="GameMap"></param>
        /// <param name="tabelLocation"></param>
        public MineModel(Map GameMap, Point tabelLocation):base(GameMap,tabelLocation)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(delegate {
                //炸弹延缓爆炸
                System.Threading.Thread.Sleep(3000);
                if (Blasted) return;
                Blast(this);
                Blasted = true;
            });
        }

        /// <summary>
        /// 被隐藏的构造函数（禁止访问）
        /// </summary>
        /// <param name="location"></param>
        /// <param name="tabelLocation"></param>
        private MineModel(Point location, Point tabelLocation) : base(location,tabelLocation) { }

        /// <summary>
        /// 立即起爆！
        /// </summary>
        /// <returns>是否允许立即起爆</returns>
        public bool BlastNow()
        {
            if (Blasted) return false;
            Blast(this);
            Blasted = true;
            return true;
        }

    }
}
