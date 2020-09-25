using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rougelike
{
    static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form1 = new Form1();
            Application.Run(form1);
        }
    }


    public class Player : Panel
    {
        public int x, y, hp, maxHp, str, dex,xp,level=1, levelUpXP;

        public Player(int x, int y,int hp, int str, int dex, int maxHP, int xp, int level, int levelUpXP)
        {
            this.x = x; this.y = y; this.hp = hp; 
            this.str = str; this.dex = dex; 
            this.maxHp = maxHP; this.xp = xp; 
            this.level = level; this.levelUpXP = levelUpXP;
        }

        public void activate()
        {
            Parent.BackColor = Color.Green;
        }
        public void deactivate()
        {
            Parent.BackColor = Color.DarkGray;
        }
    }

    public class Tile : Panel
    {
        public bool visited = false;
        public int z = 2;

        public Tile(bool visited, int z)
        {
            this.visited = visited;
            this.z = z;
        }

        public Tile(bool visited)
        {
            this.visited = visited;
        }
    }
}