using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rougelike
{
    class Initialize
    {
        public string name;
        public int hp, str, dex, xp;

        public Initialize(string name)
        {
            this.name = name;
        }
    }

    class Enemy1 : Initialize
    {
        public static List<Enemy1> monsterList = new List<Enemy1>();
        public static List<Enemy1> eliteList = new List<Enemy1>();

        //int a = rand.Next(0, 100);
        Random rand = new Random();
        public Enemy1(string name, int hp, int str, int dex, int xp) : base(name)
        {
            this.hp = hp+rand.Next(-2,2);
            this.str = str + rand.Next(-1, 1);
            this.dex = dex + rand.Next(-1, 1);
            this.xp = xp;
        }
        public static void addMonstersToList()
        {
            monsterList.Add(new Enemy1("Enemy1", 5, 2, 10, 10));
            monsterList.Add(new Enemy1("Enemy2", 5, 4, 2, 10));

            eliteList.Add(new Enemy1("Elite1", 25, 4, 7, 20));
            eliteList.Add(new Enemy1("Elite2", 17, 3, 15, 20));
        }
        public static void LevelUp()
        {
            foreach(Enemy1 monster in monsterList)
            {
                monster.hp = monster.hp + 5;
                monster.str = monster.str + 2;
                monster.dex = monster.dex + 2;
                monster.xp = monster.xp + 5;

            }

            foreach (Enemy1 elite in eliteList)
            {
                elite.hp = elite.hp + 5;
                elite.str = elite.str + 2;
                elite.dex = elite.dex + 2;
                elite.xp = elite.xp + 5;
            }
        }


    }

    class Items
    {
        public static List<Items> itemsList = new List<Items>();
        public string name, effect;
        public int value, type;
        public Items(string name, string effect, int value, int type)
        {
            this.name = name; this.effect = effect; this.value = value; this.type = type;
        }

        

        public static void addItemsToList()
        {
            itemsList.Add(new Items("Strength relic","+2 strength", 2, 1));
            itemsList.Add(new Items("Dexterity relic", "+2 dexterity", 2, 2));
            itemsList.Add(new Items("Health relic", "+2 health", 2, 3));
        }
    }
}
