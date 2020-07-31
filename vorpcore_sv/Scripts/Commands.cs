using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using vorpcore_sv.Utils;

namespace vorpcore_sv.Scripts
{
    class Commands : BaseScript
    {
        public Commands()
        {

            TriggerEvent("chat:addSuggestion", "/setgroup", "set group to user\n Example: /setgroup playerid mod");
            API.RegisterCommand("setgroup", new Action<int, List<object>, string>((source, args, rawCommand) =>
            {
                if (source > 0) // it's a player.
                {
                    Player _source = ApiController.getSource(source);
                    TriggerEvent("vorp:getCharacter", source, new Action<dynamic>((user) =>
                    {
                        if (user.group == "superadmin")
                        {
                            try
                            {
                                int target = int.Parse(args[0].ToString());
                                string newgroup = args[1].ToString();

                                if (String.IsNullOrEmpty(newgroup))
                                {
                                    _source.TriggerEvent("vorp:Tip", "ERROR: Use Correct Sintaxis", 4000);
                                    return;
                                }

                                TriggerEvent("vorp:setGroup", target, newgroup);
                                _source.TriggerEvent("vorp:Tip", $"Target {target} have group {newgroup}", 4000);
                            }
                            catch
                            {
                                _source.TriggerEvent("vorp:Tip", "ERROR: Use Correct Sintaxis", 4000);
                            }
                        }
                        else
                        {
                            _source.TriggerEvent("vorp:Tip", LoadConfig.Langs["NoPermissions"], 4000);
                        }
                    }));
                }
                else
                {
                    Debug.WriteLine("This only can be executed from client side.");
                }
            }), false);

            TriggerEvent("chat:addSuggestion", "/setjob", "set job to user\n Example: /setjob playerid medic");
            API.RegisterCommand("setjob", new Action<int, List<object>, string>((source, args, rawCommand) =>
            {
                if (source > 0) // it's a player.
                {
                    Player _source = ApiController.getSource(source);
                    TriggerEvent("vorp:getCharacter", source, new Action<dynamic>((user) =>
                    {
                        if (user.group == "admin" || user.group == "superadmin")
                        {
                            try
                            {
                                int target = int.Parse(args[0].ToString());
                                string newjob = args[1].ToString();

                                if (String.IsNullOrEmpty(newjob))
                                {
                                    _source.TriggerEvent("vorp:Tip", "ERROR: Use Correct Sintaxis", 4000);
                                    return;
                                }

                                TriggerEvent("vorp:setJob", target, newjob);
                                _source.TriggerEvent("vorp:Tip", $"Target {target} have job {newjob}", 4000);
                            }
                            catch
                            {
                                _source.TriggerEvent("vorp:Tip", "ERROR: Use Correct Sintaxis", 4000);
                            }
                        }
                        else
                        {
                            _source.TriggerEvent("vorp:Tip", LoadConfig.Langs["NoPermissions"], 4000);
                        }
                    }));
                }
                else
                {
                    Debug.WriteLine("This only can be executed from client side.");
                }
            }), false);

            TriggerEvent("chat:addSuggestion", "/addmoney", "add money to user\n Example: /addmoney playerid moneytype quantity");
            API.RegisterCommand("addmoney", new Action<int, List<object>, string>((source, args, rawCommand) =>
            {
                if (source > 0) // it's a player.
                {
                    Player _source = ApiController.getSource(source);

                    TriggerEvent("vorp:getCharacter", source, new Action<dynamic>((user) =>
                    {
                        if (user.group == "admin" || user.group == "superadmin")
                        {
                            try
                            {
                                int target = int.Parse(args[0].ToString());
                                int montype = int.Parse(args[1].ToString());
                                double quantity = double.Parse(args[2].ToString());

                                TriggerEvent("vorp:addMoney", target, montype, quantity);
                                _source.TriggerEvent("vorp:Tip", $"Added {quantity} to {target}", 4000);
                            }
                            catch
                            {
                                _source.TriggerEvent("vorp:Tip", "ERROR: Use Correct Sintaxis", 4000);
                            }
                        }
                        else
                        {
                            _source.TriggerEvent("vorp:Tip", LoadConfig.Langs["NoPermissions"], 4000);
                        }
                    }));
                }
                else
                {
                    Debug.WriteLine("This only can be executed from client side.");
                }
            }), false);

            TriggerEvent("chat:addSuggestion", "/delmoney", "remove money to user\n Example: /delmoney playerid moneytype quantity");
            API.RegisterCommand("delmoney", new Action<int, List<object>, string>((source, args, rawCommand) =>
            {
                if (source > 0) //it's a player.
                {
                    Player _source = ApiController.getSource(source);
                    TriggerEvent("vorp:getCharacter", source, new Action<dynamic>((user) =>
                    {
                        if (user.group == "admin" || user.group == "superadmin")
                        {
                            try
                            {
                                int target = int.Parse(args[0].ToString());
                                int montype = int.Parse(args[1].ToString());
                                double quantity = double.Parse(args[2].ToString());

                                TriggerEvent("vorp:removeMoney", target, montype, quantity);
                                _source.TriggerEvent("vorp:Tip", $"Removed {quantity} from {target}", 4000);
                            }
                            catch
                            {
                                _source.TriggerEvent("vorp:Tip", "ERROR: Use Correct Sintaxis", 4000);
                            }
                        }
                        else
                        {
                            _source.TriggerEvent("vorp:Tip", LoadConfig.Langs["NoPermissions"], 4000);
                        }
                    }));
                }
                else
                {
                    Debug.WriteLine("This only can be executed from client side.");
                }
            }), false);

            TriggerEvent("chat:addSuggestion", "/addwhitelist", "Example: /addwhitelist 11000010c8aa16e");
            API.RegisterCommand("addwhitelist", new Action<int, List<object>, string>((source, args, rawCommand) =>
            {
                if (source > 0) // it's a player.
                {
                    Player _source = ApiController.getSource(source);
                    TriggerEvent("vorp:getCharacter", source, new Action<dynamic>((user) =>
                    {
                        if (user.group == "admin" || user.group == "superadmin")
                        {
                            try
                            {
                                string steamId = args[0].ToString();
                                Exports["ghmattimysql"].execute("SELECT * FROM whitelist WHERE identifier = ?", new[] { steamId }, new Action<dynamic>((result) =>
                                {
                                    if (result.Count == 0)
                                    {
                                        Exports["ghmattimysql"].execute("INSERT INTO whitelist (`identifier`) VALUES (?)", new object[] { steamId });
                                        Whitelist.whitelist.Add(steamId);
                                        _source.TriggerEvent("vorp:Tip", $"Added {steamId} to whitelist", 4000);
                                        Debug.WriteLine($"Added {steamId} to whitelist");
                                    }
                                    else
                                    {
                                        _source.TriggerEvent("vorp:Tip", $"{steamId} Is Whitelisted {steamId}", 4000);
                                        Debug.WriteLine($"{steamId} Is Whitelisted {steamId}");                                    }
                                }));
                            }
                            catch
                            {
                                _source.TriggerEvent("vorp:Tip", "ERROR: Use Correct Sintaxis", 4000);
                                Debug.WriteLine("ERROR: Use Correct Sintaxis");
                            }
                        }
                        else
                        {
                            _source.TriggerEvent("vorp:Tip", LoadConfig.Langs["NoPermissions"], 4000);
                        }
                    }));
                }
                else
                {
                    try
                    {
                        string steamId = args[0].ToString();
                        Exports["ghmattimysql"].execute("SELECT * FROM whitelist WHERE identifier = ?", new[] { steamId }, new Action<dynamic>((result) =>
                        {
                            if (result.Count == 0)
                            {
                                Exports["ghmattimysql"].execute("INSERT INTO whitelist (`identifier`) VALUES (?)", new object[] { steamId });
                                Whitelist.whitelist.Add(steamId);
                                Debug.WriteLine($"Added {steamId} to whitelist", 4000);
                            }
                            else
                            {
                                Debug.WriteLine($"{steamId} Is Whitelisted {steamId}", 4000);
                            }
                        }));
                    }
                    catch
                    {
                        Debug.WriteLine("ERROR: Use Correct Sintaxis", 4000);
                    }
                }
            }), false);

            TriggerEvent("chat:addSuggestion", "/removewhitelist", "Example: /removewhitelist 11000010c8aa16e");
            API.RegisterCommand("removewhitelist", new Action<int, List<object>, string>((source, args, rawCommand) =>
            {
                if (source > 0) // it's a player.
                {
                    Player _source = ApiController.getSource(source);
                    TriggerEvent("vorp:getCharacter", source, new Action<dynamic>((user) =>
                    {
                        if (user.group == "admin" || user.group == "superadmin")
                        {
                            try
                            {
                                string steamId = args[0].ToString();
                                Exports["ghmattimysql"].execute("SELECT * FROM whitelist WHERE identifier = ?", new[] { steamId }, new Action<dynamic>((result) =>
                                {
                                    if (result.Count != 0)
                                    {
                                        Exports["ghmattimysql"].execute($"DELETE FROM whitelist WHERE (`identifier`) = ?", new object[] { steamId });
                                        Whitelist.whitelist.Remove(steamId);
                                        _source.TriggerEvent("vorp:Tip", $"Removed {steamId} to whitelist", 4000);
                                        Debug.WriteLine($"Removed {steamId} from whitelist");
                                    }
                                    else
                                    {
                                        _source.TriggerEvent("vorp:Tip", $"{steamId} Is Whitelisted {steamId}", 4000);
                                        Debug.WriteLine($"{steamId} Is Whitelisted {steamId}");
                                    }
                                }));
                            }
                            catch
                            {
                                _source.TriggerEvent("vorp:Tip", "ERROR: Use Correct Sintaxis", 4000);
                                Debug.WriteLine("ERROR: Use Correct Sintaxis");
                            }
                        }
                        else
                        {
                            _source.TriggerEvent("vorp:Tip", LoadConfig.Langs["NoPermissions"], 4000);
                        }
                    }));
                }
                else
                {
                    try
                    {
                        string steamId = args[0].ToString();
                        Exports["ghmattimysql"].execute("SELECT * FROM whitelist WHERE identifier = ?", new[] { steamId }, new Action<dynamic>((result) =>
                        {
                            if (result.Count != 0)
                            {
                                Exports["ghmattimysql"].execute($"DELETE FROM whitelist WHERE (`identifier`) = ?", new object[] { steamId });
                                Whitelist.whitelist.Remove(steamId);
                                Debug.WriteLine($"Removed {steamId} from whitelist");
                            }
                            else
                            {
                                Debug.WriteLine($"{steamId} Is Not In Whitelist {steamId}", 4000);
                            }
                        }));
                    }
                    catch
                    {
                        Debug.WriteLine("ERROR: Use Correct Sintaxis", 4000);
                    }
                }
            }), false);

        }
    }
}
