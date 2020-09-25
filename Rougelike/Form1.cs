using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Rougelike
{
    public partial class Form1 : Form
    {
        private int tileSize;
        private const int gridSize = 4;
        public Tile[,] mapTiles;
        Player player = null;
        ListViewItem lvi;
        Random rand = new Random();
        private int playerstr=3, playerdex=1, playerhp=55, playerMaxHp=55, level = 1, xp = 0;
        private int tiles, iFight, floor=1;

        public Form1()
        {
            InitializeComponent();
            Enemy1.addMonstersToList();
            Items.addItemsToList();
            newGame();

            
        }

        private void initMap()
        {
            
                    mapTiles = new Tile[gridSize, gridSize];

            for (var n = 0; n < gridSize; n++)
            {
                for (var m = 0; m < gridSize; m++)
                {
                    var newPanel = new Tile(false)
                    {
                        Size = new Size(tileSize, tileSize),
                        Location = new Point(tileSize * m, tileSize * (gridSize - n - 1)),
                        Text = "",
                    };

                    // add to Form's Controls so that they show up
                    Controls.Add(newPanel);
                    newPanel.Parent = this.panel1;
                    
                    // add to our 2d array of panels for future use
                    mapTiles[n, m] = newPanel;

                    // color the backgrounds
                        newPanel.BackgroundImage = Image.FromFile("images/notVisited.png");
                        newPanel.BackgroundImageLayout = ImageLayout.Stretch;

                }
            }
        }


        private void initStats()
        {
            dexInt.Text = playerdex.ToString();
            strInt.Text = playerstr.ToString();
            xpInt.Text = xp.ToString() + " / " + player.levelUpXP;
            hpInt.Text = playerhp.ToString()+" / "+ playerMaxHp;
            levelInt.Text = level.ToString();

        }

        private void newGame()
        {

            tiles = gridSize * gridSize;
            tileSize = Math.Min(panel1.Width / gridSize, panel1.Height / gridSize);
            initMap();

            player =createPlayer(rand.Next(0,gridSize-1), rand.Next(0, gridSize - 1));
            newTile(player.x,player.y);
            initStats();

        }


        private void refreshMap()
        {
            tileSize = Math.Min(panel1.Width / gridSize, panel1.Height / gridSize);
            for (var n = 0; n < gridSize; n++)
            {
                for (var m = 0; m < gridSize; m++)
                {
                    mapTiles[n, m].Size = new Size(tileSize, tileSize);
                    mapTiles[n, m].Location = new Point(tileSize * m, tileSize * (gridSize - n - 1));
                }
            }
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            refreshMap();
        }

        private Player createPlayer(int x, int y)
        {

            Player player = new Player(x, y,playerhp,playerstr,playerdex, playerMaxHp,level,xp,50*level);
            player.Parent = mapTiles[y, x];
            player.Size = new Size(tileSize, tileSize);
            resetPlayerGraphics(player);
            player.BackgroundImageLayout = ImageLayout.Stretch;
            return player;
        }

        private void resetPlayerGraphics(Player player)
        {
            player.BackgroundImage = Image.FromFile("images/active.png");

        }

        private void newTile(int x, int y)
        {
            int a = rand.Next(0, 100);
            
            
            if (mapTiles[y, x].visited == false)
            {
                tiles -= 1;
                if(tiles==0)
                {
                    mapTiles[y, x].Text = "Exit";
                    mapTiles[y, x].BackgroundImage = Image.FromFile("images/ladder.png");
                    Ladder.Visible = true;
                }
                else if(tiles == gridSize*gridSize-1)
                {
                    mapTiles[y, x].Text = "Entrance";
                    mapTiles[y, x].BackgroundImage = Image.FromFile("images/start.png");
                } 
                //Unvisited Room
                else if (a < 30)
                {
                    mapTiles[y, x].Text = "Monster";
                    result.Text = mapTiles[y, x].Text;
                    mapTiles[y, x].visited = true;
                    mapTiles[y, x].BackgroundImage = Image.FromFile("images/monster.png");
                    enemyMet();
                }
                else if (a < 40)
                {
                    mapTiles[y, x].Text = "Elite";
                    mapTiles[y, x].BackgroundImage = Image.FromFile("images/elite.png");
                    eliteMet();
                } 
                else if (a < 75)
                {
                    mapTiles[y, x].Text = "Empty Room";
                    mapTiles[y, x].BackgroundImage = Image.FromFile("images/emptyRoom.png");
                    enemyClear();
                }
                else if (a < 85)
                {
                    mapTiles[y, x].Text = "Healing Fountain remaining uses : 1";
                    mapTiles[y, x].BackgroundImage = Image.FromFile("images/fountain.png");
                    Heal(y, x);
                    enemyClear();
                }
                else
                {

                    mapTiles[y, x].Text = "Chest";
                    int i = rand.Next(0,Items.itemsList.Count);
                    lvi = new ListViewItem(Items.itemsList[i].name);
                    lvi.SubItems.Add(Items.itemsList[i].effect);
                    listView1.Items.Add(lvi);
                    switch (Items.itemsList[i].type)
                    {
                        case 1:
                            playerstr += Items.itemsList[i].value;
                            initStats();
                            break;
                        case 2:
                            playerdex += Items.itemsList[i].value;
                            initStats();
                            break;
                        case 3:
                            playerMaxHp += Items.itemsList[i].value;
                            playerhp = playerMaxHp;
                            initStats();
                            break;
                    }
                    mapTiles[y, x].BackgroundImage = Image.FromFile("images/chest.png");
                    enemyClear();
                }
                initStats();
                result.Text = mapTiles[y, x].Text;
                mapTiles[y, x].visited = true;
            }
            else
            {
                //Visited Room
                if (mapTiles[y, x].Text == "Healing Fountain remaining uses : 1")
                {
                    if(mapTiles[y, x].z >0)
                    {
                        Heal(y, x);
                        mapTiles[y, x].Text = "Healing Fountain is empty";
                    }
                    
                }
                initStats();
                enemyClear();
                result.Text = mapTiles[y, x].Text;
            }

        }

        void Heal(int y, int x)
        {
            double heal = player.maxHp * 0.3;
            if ((Convert.ToInt32(heal) + playerhp) > player.maxHp)
            {
                playerhp = player.maxHp;
            }
            else
            {
                playerhp += Convert.ToInt32(heal);
            }
            hpInt.Text = playerhp.ToString() + " / " + playerMaxHp;
            mapTiles[y, x].z -= 1;

        }

        void LevelUP()
        {
            playerMaxHp += 5;
            playerhp = playerMaxHp;
            playerstr += 1;
            playerdex += 1;
            level += 1;
            xp = 0;
            player.Dispose();
            player = createPlayer(player.x, player.y);
            initStats();
        }

        void enemyClear()
        {
            nameEnem.Text = "";
            dexIntEnem.Text = "";
            strIntEnem.Text = "";
            hpIntEnem.Text = "";
            
        }
        

        private void enemyMet()
        {
            iFight = rand.Next(0, Enemy1.monsterList.Count);
            nameEnem.Text = Enemy1.monsterList[iFight].name;
            dexIntEnem.Text = Enemy1.monsterList[iFight].dex.ToString();
            strIntEnem.Text = Enemy1.monsterList[iFight].str.ToString();
            hpIntEnem.Text = Enemy1.monsterList[iFight].hp.ToString();
            this.panel2.Visible = true;
            this.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form1_PreviewKeyDown);
        }
        private void eliteMet()
        {
            iFight = rand.Next(0, Enemy1.eliteList.Count);
            nameEnem.Text = Enemy1.eliteList[iFight].name;
            dexIntEnem.Text = Enemy1.eliteList[iFight].dex.ToString();
            strIntEnem.Text = Enemy1.eliteList[iFight].str.ToString();
            hpIntEnem.Text = Enemy1.eliteList[iFight].hp.ToString();
            this.panel2.Visible = true;
            this.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form1_PreviewKeyDown);
        }



        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;

            switch (e.KeyValue)
            {
                case 37:
                case 65:
                    //Left
                    if (player.x>0) 
                    {
                        player.Dispose();
                        player = createPlayer((player.x) - 1, player.y);
                        newTile(player.x, player.y);

                    }
                    break;
                case 38:
                case 87:
                    //Up
                    if (player.y< gridSize - 1)
                    {
                        player.Dispose();
                        player = createPlayer(player.x, (player.y) + 1);
                        newTile(player.x, player.y);
                    }
                    break;
                case 39:
                case 68:
                    //Right
                    if (player.x< gridSize - 1)
                    {
                        player.Dispose();
                        player = createPlayer((player.x) + 1, player.y);
                        newTile(player.x, player.y);
                    }
                    break;
                case 40:
                case 83:
                    //Down
                    if (player.y> 0)
                    {
                        player.Dispose();
                        player = createPlayer(player.x, (player.y) - 1);
                        newTile(player.x, player.y);
                    }
                    break;
            }

        }

        private void tabControl1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            tabControl1.Enabled = false;
            tabControl1.Enabled = true;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            int hpEnem = Int32.Parse(hpIntEnem.Text);

            int dmgPlayer = rand.Next(1,player.str);

            int dmgEnem = rand.Next(1, Int32.Parse(strIntEnem.Text));

            int dexPlayer = player.dex;
            int dexEnem = Int32.Parse(dexIntEnem.Text);
            
            
            if (dexPlayer > 40)
            {
                dexPlayer = 40;
            }
            if (dexEnem > 40)
            {
                dexEnem = 40;
            }
            int dodgePlayer = rand.Next(dexPlayer, 100);
            int dodgeEnem = rand.Next(dexEnem, 100);
            

            if (dodgePlayer > 85)
                textEnem.Text = "Enemy missed";
            else
            {
                textEnem.Text = "The enemy hit you for " + dmgEnem;
                playerhp -= dmgEnem;
                hpInt.Text = playerhp.ToString() + " / " + playerMaxHp;
            }

            if (dodgeEnem > 85)
                textPlayer.Text = "You missed";
            else
            {
                textPlayer.Text = "You hit the enemy for " + dmgPlayer;
                hpEnem -= dmgPlayer;
                hpIntEnem.Text = hpEnem.ToString();
            }

            if (playerhp <= 0)
            {
                button1.Visible = false;
                textPlayer.Visible = false;
                textEnem.Visible = false;
                Death.Visible = true;
                NGame.Visible = true;
                
            }
                else if (hpEnem <= 0)
                
            {
                hpEnem = 0;
                hpIntEnem.Text = hpEnem.ToString();
                this.panel2.Visible = false;
                this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form1_PreviewKeyDown);
                button1.Enabled = false;
                button1.Enabled = true;
                if (mapTiles[player.y, player.x].Text=="Monster")
                {
                    xp += Enemy1.monsterList[iFight].xp;
                    xpInt.Text = xp.ToString() + " / " + player.levelUpXP;
                    if (xp >= player.levelUpXP)
                        LevelUP();
                }
                else
                {
                    xp += Enemy1.eliteList[iFight].xp;
                    xpInt.Text = xp.ToString() + " / " + player.levelUpXP;
                    if (xp >= player.levelUpXP)
                        LevelUP();
                }
                
            }
        }
        private void NGame_Click(object sender, EventArgs e)
        {
            NGame.Enabled = false;
            NGame.Enabled = true;
            NGame.Visible = false;
            Death.Visible = false;
            button1.Visible = true;
            textPlayer.Visible = true;
            textEnem.Visible = true;
            for (var n = 0; n < gridSize; n++)
            {
                for (var m = 0; m < gridSize; m++)
                {
                    mapTiles[n, m].Dispose();
                }
            }
            floor = 1;

            playerstr = 3;
            playerdex = 1;
            playerhp = 55;
            playerMaxHp = 55;
            level = 1;
            xp = 0;
            Enemy1.monsterList.Clear();
            Enemy1.eliteList.Clear();
            listView1.Items.Clear();
            Enemy1.addMonstersToList();
            newGame();

            this.panel2.Visible = false;
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form1_PreviewKeyDown);
            button1.Enabled = false;
            button1.Enabled = true;
        }

        private void Ladder_Click(object sender, EventArgs e)
        {
            Ladder.Enabled = false;
            Ladder.Enabled = true;
            Ladder.Visible = false;
            for (var n = 0; n < gridSize; n++)
            {
                for (var m = 0; m < gridSize; m++)
                {
                    mapTiles[n, m].Dispose();
                }
            }
            floor += 1;
            Enemy1.LevelUp();
            newGame();

        }
    }
}
