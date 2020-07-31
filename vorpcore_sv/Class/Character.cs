using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vorpcore_sv.Class
{
    public class Character : BaseScript
    {
        private string identifier;

        private string group;
        private string job;
        private string jobgrade;
        private string firstname;
        private string lastname;
        private string inventory;
        private string status;
        private string coords;

        private double money;
        private double gold;
        private double rol;

        private int xp;

        private bool isdead;

        public string Identifier { get => identifier; }
        public string Group { get => group; }
        public string Job { get => job; }
        public string Jobgrade { get => jobgrade; }
        public string Firstname { get => firstname; set => firstname = value; }
        public string Lastname { get => lastname; set => lastname = value; }
        public string Inventory { get => inventory; set => inventory = value; }
        public string Status { get => status; set => status = value; }
        public string Coords { get => coords; set => coords = value; }
        public double Money { get => money; }
        public double Gold { get => gold; }
        public double Rol { get => rol; }
        public int Xp { get => xp; }
        public bool IsDead { get => isdead; }

        public Character(string identifier)
        {
            this.identifier = identifier;
            this.job = "unemployed";
            this.group = "user";
            this.inventory = "{}";
        }

        public Character(string identifier, string group, string job, string jobgrade, string firstname, string lastname, string inventory, string status, string coords, double money, double gold, double rol, int xp, bool isdead)
        {
            this.identifier = identifier;
            this.group = group;
            this.job = job;
            this.jobgrade = jobgrade;
            this.firstname = firstname;
            this.lastname = lastname;
            this.inventory = inventory;
            this.status = status;
            this.coords = coords;
            this.money = money;
            this.gold = gold;
            this.rol = rol;
            this.xp = xp;
            this.isdead = isdead;
        }

        public Dictionary<string, object> getCharacter()
        {
            Dictionary<string, object> userData = new Dictionary<string, object>();
            userData.Add("identifier", identifier);
            userData.Add("group", group);
            userData.Add("job", job);
            userData.Add("money", money);
            userData.Add("gold", gold);
            userData.Add("rol", rol);
            userData.Add("xp", xp);
            userData.Add("firstname", firstname);
            userData.Add("lastname", lastname);
            userData.Add("inventory", inventory);
            userData.Add("status", status);
            userData.Add("coords", coords);
            userData.Add("isdead", isdead);

            return userData;
        }

        public void addCurrency(int currency, double quantity)
        {
            switch (currency)
            {
                case 0:
                    money += quantity;
                    Exports["ghmattimysql"].execute($"UPDATE characters SET money=money + ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
                case 1:
                    gold += quantity;
                    Exports["ghmattimysql"].execute($"UPDATE characters SET gold=gold + ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
                case 2:
                    rol += quantity;
                    Exports["ghmattimysql"].execute($"UPDATE characters SET rol=rol + ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
                default:
                    break;
            }
        }

        public void removeCurrency(int currency, double quantity)
        {
            switch (currency)
            {
                case 0:
                    money -= quantity;
                    Exports["ghmattimysql"].execute($"UPDATE characters SET money=money - ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
                case 1:
                    gold -= quantity;
                    Exports["ghmattimysql"].execute($"UPDATE characters SET gold=gold - ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
                case 2:
                    rol -= quantity;
                    Exports["ghmattimysql"].execute($"UPDATE characters SET rol=rol - ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
                default:
                    break;
            }
        }

        public void addXp(int quantity)
        {
            xp += quantity;
            Exports["ghmattimysql"].execute($"UPDATE characters SET xp=xp + ? WHERE identifier=?", new object[] { quantity, identifier });
        }

        public void removeXp(int quantity)
        {
            xp -= quantity;
            Exports["ghmattimysql"].execute($"UPDATE characters SET xp=xp - ? WHERE identifier=?", new object[] { quantity, identifier });
        }

        public void setJob(string newjob)
        {
            job = newjob;
            Exports["ghmattimysql"].execute($"UPDATE characters SET job=? WHERE identifier=?", new string[] { newjob, identifier });
        }

        public void setGroup(string newgroup)
        {
            group = newgroup;
            Exports["ghmattimysql"].execute($"UPDATE characters SET `group`=? WHERE identifier=?", new string[] { newgroup.ToString(), identifier });
        }

        public void setDead(bool dead)
        {
            isdead = dead;
            int intdead = dead ? 1 : 0;
            Exports["ghmattimysql"].execute("UPDATE characters SET isdead=? WHERE identifier=?", new object[] { intdead, identifier });
        }

    }
}