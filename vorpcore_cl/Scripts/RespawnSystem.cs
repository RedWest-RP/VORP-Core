using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vorpcore_cl.Scripts
{
    //Respawn System similar to GTA V Death Screen https://imgur.com/a/YnEz9Yd | https://gyazo.com/24cfd684129ee9771f67b3470d351021
    class RespawnSystem : BaseScript
    {
        //Hum this wheel runs a lot, they are made of aluminium
        public RespawnSystem()
        {
            Tick += OnPlayerDead;
            Tick += InfoOnDead;
            EventHandlers["vorp:resurrectPlayer"] += new Action(ResurrectPlayer);
        }

        private async void ResurrectPlayer()
        {
            await resurrectPlayer();
        }

        static bool setDead = false;
        static int TimeToRespawn = 1;

        [Tick]
        public async Task OnPlayerDead()
        {
            await Delay(0);
            if (Function.Call<bool>((Hash)0x2E9C3FCB6798F397, API.PlayerId()))
            {

                if (!setDead)
                {
                    TriggerServerEvent("vorp:ImDead", true);
                    TriggerEvent("vorp:ImDead", true);
                    setDead = true;
                }
                API.NetworkSetInSpectatorMode(true, API.PlayerPedId());
                API.AnimpostfxPlay("DeathFailMP01");                 
                Function.Call((Hash)0xD63FE3AF9FB3D53F, false);     
                Function.Call((Hash)0x1B3DA717B9AFF828, false);
                TimeToRespawn = Utils.GetConfig.Config["RespawnTime"].ToObject<int>();

                while (TimeToRespawn >= 0 && setDead)
                {
                    await Delay(1000);
                    TimeToRespawn -= 1;
                    Exports["spawnmanager"].setAutoSpawn(false); //Is this a copy ? wtf I need to create a new spawnmanager? 
                }
                string keyPress = Utils.GetConfig.Config["RespawnKey"].ToString();
                int KeyInt = Convert.ToInt32(keyPress, 16);
                bool pressKey = false; //sorry the word pressed has copyright
                while (!pressKey && setDead)
                {
                    await Delay(0);
                    if (!Function.Call<bool>((Hash)0xC841153DED2CA89A, API.PlayerPedId()))
                    {
                        API.NetworkSetInSpectatorMode(true, API.PlayerPedId());
                        await Utils.Miscellanea.DrawText(Utils.GetConfig.Langs["SubTitlePressKey"], Utils.GetConfig.Config["RespawnSubTitleFont"].ToObject<int>(), 0.50f, 0.50f, 1.0f, 1.0f, 255, 255, 255, 255, true, true);
                        if (Function.Call<bool>((Hash)0x580417101DDB492F, 0, KeyInt))
                        {
                            TriggerServerEvent("vorp:PlayerForceRespawn");
                            TriggerEvent("vorp:PlayerForceRespawn");
                            API.DoScreenFadeOut(3000);
                            await Delay(3000);
                            await respawnPlayer();
                            pressKey = true;
                            await Delay(1000);
                        }
                    }
                }
            }
        }

        public static async Task InfoOnDead()
        {
            if (Function.Call<bool>((Hash)0xC841153DED2CA89A, API.PlayerPedId()) && setDead)
            {
                int carrier = Function.Call<int>((Hash)0x09B83E68DE004CD4, API.PlayerPedId());
                API.NetworkSetInSpectatorMode(true, carrier);
                await Utils.Miscellanea.DrawText(Utils.GetConfig.Langs["YouAreCarried"], 4, 0.50f, 0.30f, 1.0f, 1.0f, 255, 255, 255, 255, true, true);
            }
            //else if (Function.Call<bool>((Hash)0x9682F850056C9ADE, API.PlayerPedId()) && setDead)
            //{
            //    API.NetworkSetInSpectatorMode(true, API.PlayerPedId());
            //    await Utils.Miscellanea.DrawText(Utils.GetConfig.Langs["YouAreInLasso"], 4, 0.50f, 0.30f, 1.0f, 1.0f, 255, 255, 255, 255, true, true);
            //}
            else if (TimeToRespawn >= 0 && setDead)
            {
                API.NetworkSetInSpectatorMode(false, 0);
                await Utils.Miscellanea.DrawText(Utils.GetConfig.Langs["TitleOnDead"], Utils.GetConfig.Config["RespawnTitleFont"].ToObject<int>(), 0.50F, 0.50F, 1.2F, 1.2F, 171, 3, 0, 255, true, true);
                await Utils.Miscellanea.DrawText(string.Format(Utils.GetConfig.Langs["SubTitleOnDead"], TimeToRespawn.ToString()), Utils.GetConfig.Config["RespawnSubTitleFont"].ToObject<int>(), 0.50f, 0.60f, 0.5f, 0.5f, 255, 255, 255, 255, true, true);
            }
        }

        public static async Task respawnPlayer()
        {
            Function.Call((Hash)0x71BC8E838B9C6035, API.PlayerPedId()); //This is from kaners? https://vespura.com/doc/natives/#_0x71BC8E838B9C6035 are u sure? lol amazing
            API.AnimpostfxStop("DeathFailMP01");
            Vector3 curPos = API.GetEntityCoords(API.PlayerPedId(), false, false);
            float heading = API.GetEntityHeading(API.PlayerPedId());
            Function.Call((Hash)0x203BEFFDBE12E96A, API.PlayerPedId(), curPos.X, curPos.Y, curPos.Z, heading, false, false, false);
            await Delay(100);
            TriggerServerEvent("vorpcharacter:getPlayerSkin");
            API.DoScreenFadeIn(1000);
            TriggerServerEvent("vorp:ImDead", false); //This is new or copy can u send me a dm?
            TriggerEvent("vorp:ImDead", false); //This is new or copy can u send me a dm?
            setDead = false;
            API.NetworkSetInSpectatorMode(false, API.PlayerPedId());
            Function.Call((Hash)0xD63FE3AF9FB3D53F, true);
            Function.Call((Hash)0x1B3DA717B9AFF828, true);
            SpawnPlayer.setPVP();
        }

        public async Task resurrectPlayer()
        {
            Function.Call((Hash)0x71BC8E838B9C6035, API.PlayerPedId()); //This is from kaners? https://vespura.com/doc/natives/#_0x71BC8E838B9C6035 are u sure? lol amazing
            API.AnimpostfxStop("DeathFailMP01");
            API.DoScreenFadeIn(1000);
            TriggerServerEvent("vorp:ImDead", false);
            TriggerEvent("vorp:ImDead", false);
            setDead = false;
            await Delay(100);
            API.NetworkSetInSpectatorMode(false, API.PlayerPedId());
            Function.Call((Hash)0xD63FE3AF9FB3D53F, true);
            Function.Call((Hash)0x1B3DA717B9AFF828, true);
            SpawnPlayer.setPVP();
        }
    }
}
