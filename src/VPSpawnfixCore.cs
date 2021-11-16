using System;
using System.Reflection;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

using HarmonyLib;

namespace VanillaPatch.Spawnfix
{
    public class VPSpawnfixCore : ModSystem
    {
        Harmony harmony;

        public override double ExecuteOrder() => 1.0;

        public override void StartServerSide(ICoreServerAPI api)
        {
            if (harmony == null) {
                harmony = new Harmony("vs.vanillapatch.spawnfix");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
        }

        // Don't dispose of Harmony, we only want to patch on first start
        /*public override void Dispose()
        {
            harmony = null;
        }*/
    }
}