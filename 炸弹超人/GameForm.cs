using System;
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
        /// 游戏地图对象
        /// </summary>
        private Map GameMap;
        public GameForm()
        {
            InitializeComponent();
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            this.SetBounds(0,0,Screen .PrimaryScreen .Bounds .Width ,Screen .PrimaryScreen .Bounds .Height );
            GameMap = new Map(Screen.PrimaryScreen.Bounds.Size);
            this.BackgroundImage = GameMap.Ground;
            this.Refresh();
            GC.Collect();
        }

        private void GameForm_Click(object sender, EventArgs e)
        {
            GameMap.ResetMap();
            this.CreateGraphics().DrawImage(GameMap.Ground, Point.Empty);
            this.CreateGraphics().DrawImage(GameMap.DrawWalls(), Point.Empty);
        }
    }
}
