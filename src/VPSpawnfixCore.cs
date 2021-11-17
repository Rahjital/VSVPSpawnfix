using System;
using System.Reflection;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.Server;

using HarmonyLib;

namespace VanillaPatch.Spawnfix
{
    public class VPSpawnfixCore : ModSystem
    {
        Harmony harmony;

        public override void StartServerSide(ICoreServerAPI api)
        {
            if (harmony == null) {
                harmony = new Harmony("vs.vanillapatch.spawnfix");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                Type serverEventAPIType = typeof(ServerMain).Assembly.GetType("Vintagestory.Server.ServerEventAPI");

                harmony.Patch(serverEventAPIType.GetMethod("add_OnTrySpawnEntity"),
                    new HarmonyMethod(typeof(ServerEventAPIPatch).GetMethod("OnTrySpawnEntityAddPrePostfix", BindingFlags.Static | BindingFlags.NonPublic)));
                harmony.Patch(serverEventAPIType.GetMethod("remove_OnTrySpawnEntity"),
                    new HarmonyMethod(typeof(ServerEventAPIPatch).GetMethod("OnTrySpawnEntityAddPrePostfix", BindingFlags.Static | BindingFlags.NonPublic)));
            }
        }

        public override void Dispose()
        {
            if (harmony != null)
            {
                ServerEventManagerPatch.Dispose();
                harmony.UnpatchAll(harmony.Id);
                harmony = null;
            }
        }
    }
}