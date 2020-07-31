﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Threading.Tasks;
using vorpcore_cl.Utils;

namespace vorpcore_cl.Scripts
{
    public class SpawnPlayer : BaseScript
    {
        public static bool firstSpawn = true;
        public static bool iSPvpOn = false;

        public SpawnPlayer()
        {
            //Iniciamos un Tick para guardar cada 10 segundos las coords del jugador en el servidor
            Tick += saveLastCoordsTick;

            Tick += manageOnMount;

            Tick += disableHud;

            EventHandlers["vorp:initPlayer"] += new Action</*string, string, string, int, int, string,*/ Vector3, float, bool>(InitPlayer);
            Function.Call(Hash.SET_MINIMAP_HIDE_FOW, true);

            EventHandlers["playerSpawned"] += new Action<object>(InitTpPlayer);
        }

        static bool active = false;

        [Tick]
        public async Task disableHud()
        {
            await Delay(1);
            API.DisableControlAction(0, 0x580C4473, true); // Disable hud
            API.DisableControlAction(0, 0xCF8A4ECA, true); // Disable hud
        }

        [Tick]
        private async Task manageOnMount()
        {
            await Delay(1);
            int pped = API.PlayerPedId();

            int count = 0;
            uint playerHash = (uint)API.GetHashKey("PLAYER");

            if (API.IsControlPressed(0, (uint)0xCEFD9220))
            {
                Function.Call((Hash)0xBF25EB89375A37AD, 1, playerHash, playerHash);
                active = true;
                await Delay(4000);
            }
            if(!API.IsPedOnMount(pped) && !API.IsPedInAnyVehicle(pped, false) && active == true)
            {
                Function.Call((Hash)0xBF25EB89375A37AD, 5, playerHash, playerHash);
                active = false;

            }else if (active == true && (API.IsPedOnMount(pped) || API.IsPedInAnyVehicle(pped, false)))
            {
                if (API.IsPedInAnyVehicle(pped, false))
                {

                }
                else if (API.GetPedInVehicleSeat(API.GetMount(pped), -1) == pped)
                {
                    Function.Call((Hash)0xBF25EB89375A37AD, 5, playerHash, playerHash);
                    active = false;
                }
            }
        }

        private async void InitTpPlayer(object spawnInfo)
        {
            if (firstSpawn)
            {
                Debug.WriteLine("VORP INIT_PLAYER");
                await Delay(4000);
                TriggerServerEvent("vorp:playerSpawn");
                firstSpawn = false;
            }
        }

        private void InitPlayer(Vector3 coords, float heading, bool isdead)
        {
            Function.Call(Hash.SET_MINIMAP_HIDE_FOW, true);
            PlayerActions.TeleportToCoords(coords.X, coords.Y, coords.Z, heading);

            if (GetConfig.Config["ActiveEagleEye"].ToObject<bool>())
            {
                Function.Call((Hash)0xA63FCAD3A6FEC6D2, API.PlayerId(), true);
            }

            if (GetConfig.Config["ActiveDeadEye"].ToObject<bool>())
            {
                Function.Call((Hash)0x95EE1DEE1DCD9070, API.PlayerId(), true);
            }

            setPVP();

            if (isdead)
            {
                TriggerServerEvent("vorp:PlayerForceRespawn");
                TriggerEvent("vorp:PlayerForceRespawn");
                RespawnSystem.respawnPlayer();
            }
        }

        public static async Task setPVP()
        {
            uint playerHash = (uint)API.GetHashKey("PLAYER");
            Function.Call((Hash)0xF808475FA571D823, true);
            Function.Call((Hash)0xBF25EB89375A37AD, 5, playerHash, playerHash);

        }

        [Tick]
        private async Task saveLastCoordsTick()
        {
            await Delay(3000);

            if (!firstSpawn)
            {
                int playerPedId = API.PlayerPedId();
                Vector3 playerCoords = API.GetEntityCoords(playerPedId, true, true);
                float playerHeading = API.GetEntityHeading(playerPedId);

                TriggerServerEvent("vorp:saveLastCoords", playerCoords, playerHeading);
            }
        }
    }
}
