using CitizenFX.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using vorpcore_sv.Scripts;

namespace vorpcore_sv.Utils
{
    class ApiController : BaseScript
    {
        public ApiController()
        {
            EventHandlers["vorp:getCharacter"] += new Action<int, dynamic>(getCharacter);
            EventHandlers["vorp:addMoney"] += new Action<int, int, double>(addMoney);
            EventHandlers["vorp:removeMoney"] += new Action<int, int, double>(removeMoney);

            EventHandlers["vorp:addXp"] += new Action<int, int>(addXp);
            EventHandlers["vorp:removeXp"] += new Action<int, int>(removeXp);

            EventHandlers["vorp:setJob"] += new Action<int, string>(setJob);
            EventHandlers["vorp:setGroup"] += new Action<int, string>(setGroup);

        }


        public static Player getSource(int handle)
        {
            PlayerList pl = new PlayerList();
            Player p = pl[handle];
            return p;
        }

        private void removeMoney(int handle, int typeCash, double quantity)
        {
            Player player = getSource(handle);

            string sid = ("steam:" + player.Identifiers["steam"]);

            if (LoadCharacter.characters.ContainsKey(sid))
            {
                Debug.WriteLine(string.Format("{0} {1} {2}", sid, typeCash, quantity));
                LoadCharacter.characters[sid].removeCurrency(typeCash, quantity);

                JObject nuipost = new JObject();
                nuipost.Add("type", "ui");
                nuipost.Add("action", "update");
                nuipost.Add("moneyquanty", LoadCharacter.characters[sid].Money);
                nuipost.Add("goldquanty", LoadCharacter.characters[sid].Gold);
                nuipost.Add("rolquanty", LoadCharacter.characters[sid].Rol);
                nuipost.Add("xp", LoadCharacter.characters[sid].Xp);
                nuipost.Add("serverId", handle);

                player.TriggerEvent("vorp:updateUi", nuipost.ToString());
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning removeMoney: User not found!");
                Console.ForegroundColor = ConsoleColor.White;
            }

           

        }

        private void addMoney(int handle, int typeCash, double quantity)
        {
          
            Player player = getSource(handle);

            string sid = ("steam:" + player.Identifiers["steam"]);

            if (LoadCharacter.characters.ContainsKey(sid))
            {
                LoadCharacter.characters[sid].addCurrency(typeCash, quantity);

                JObject nuipost = new JObject();
                nuipost.Add("type", "ui");
                nuipost.Add("action", "update");
                nuipost.Add("moneyquanty", LoadCharacter.characters[sid].Money);
                nuipost.Add("goldquanty", LoadCharacter.characters[sid].Gold);
                nuipost.Add("rolquanty", LoadCharacter.characters[sid].Rol);
                nuipost.Add("xp", LoadCharacter.characters[sid].Xp);
                nuipost.Add("serverId", handle);

                player.TriggerEvent("vorp:updateUi", nuipost.ToString());
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning addMoney: User not found!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void addXp(int handle, int quantity)
        {
          
            Player player = getSource(handle);

            string sid = ("steam:" + player.Identifiers["steam"]);

            if (LoadCharacter.characters.ContainsKey(sid))
            {
                LoadCharacter.characters[sid].addXp(quantity);

                JObject nuipost = new JObject();
                nuipost.Add("type", "ui");
                nuipost.Add("action", "setxp");
                nuipost.Add("xp", LoadCharacter.characters[sid].Xp);

                player.TriggerEvent("vorp:updateUi", nuipost.ToString());
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning addXp: User not found!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void removeXp(int handle, int quantity)
        {
            
            Player player = getSource(handle);

            string sid = ("steam:" + player.Identifiers["steam"]);

            if (LoadCharacter.characters.ContainsKey(sid))
            {
                LoadCharacter.characters[sid].removeXp(quantity);

                JObject nuipost = new JObject();
                nuipost.Add("type", "ui");
                nuipost.Add("action", "setxp");
                nuipost.Add("xp", LoadCharacter.characters[sid].Xp);

                player.TriggerEvent("vorp:updateUi", nuipost.ToString());
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning removeXp: User not found!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void setJob(int handle, string job)
        {

            Player player = getSource(handle);

            string sid = ("steam:" + player.Identifiers["steam"]);

            if (LoadCharacter.characters.ContainsKey(sid))
            {
                LoadCharacter.characters[sid].setJob(job);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning setJob: User not found!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void setGroup(int handle, string group)
        {
            Player player = getSource(handle);

            string sid = ("steam:" + player.Identifiers["steam"]);

            if (LoadCharacter.characters.ContainsKey(sid))
            {
                LoadCharacter.characters[sid].setGroup(group);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning setGroup: User not found!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void getCharacter(int handle, dynamic cb)
        {
            Player player = getSource(handle);

            string sid = ("steam:" + player.Identifiers["steam"]);

            if (!LoadCharacter.characters.ContainsKey(sid))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning: User not found!");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                cb(LoadCharacter.characters[sid].getCharacter());
            }
        }
    }
}
