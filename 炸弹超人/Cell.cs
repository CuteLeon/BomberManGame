using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 炸弹超人
{
    class Cell
    {
        /// <summary>
        /// 像素坐标
        /// </summary>
        public Point Location;
        /// <summary>
        /// 元素在元素表Tabel里的坐标
        /// </summary>
        public Point TabelLocation;
        /// <summary>
        /// 隐藏默认构造函数
        /// </summary>
        private Cell() { }

        /// <summary>
        /// 公开的元素构造函数
        /// </summary>
        /// <param name="location">像素坐标</param>
        /// <param name="tabelLocation">在元素表里的坐标</param>
        public Cell(Point location,Point tabelLocation)
        {
            Location = location;
            TabelLocation = tabelLocation;
        }

        /// <summary>
        /// 公开的元素构造函数
        /// </summary>
        /// <param name="GameMap">对象Map</param>
        /// <param name="tabelLocation">在元素表里的坐标</param>
        public Cell(Map GameMap, Point tabelLocation)
        {
            TabelLocation = tabelLocation;
            Location = new Point(GameMap.PaddingSize.Width + tabelLocation.X* GameMap.CellSize.Width, GameMap.PaddingSize.Height + tabelLocation.Y* GameMap.CellSize.Height);
        }
    }
}
