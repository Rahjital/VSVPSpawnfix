using System;
using System.Reflection;
using Vintagestory.Server;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

using HarmonyLib;

using Vintagestory.API.Server;

namespace VanillaPatch.Spawnfix
{
    static class ServerEventAPIPatch
    {
        static bool OnTrySpawnEntityAddPrePostfix()
        {
            ServerEventManagerPatch.dirty = true;

            return true;
        }
    }

    [HarmonyPatch(typeof(ServerEventManager))]
    static class ServerEventManagerPatch
    {
        public static bool dirty = true;
        static MulticastDelegate OnTrySpawnEntityDelegate;

        [HarmonyPrefix]
        [HarmonyPatch("TriggerTrySpawnEntity")]
        static bool TriggerTrySpawnEntityPrefix(ref EntityProperties properties, Vec3d position, long herdId, ServerEventManager __instance, ref bool __result)
        {
            __result = true;

            if (__instance.GetType() == typeof(CoreServerEventManager))
            {
                return true;
            }

            if (dirty)
            {
                FieldInfo fieldInfo = typeof(ServerEventManager).GetField("OnTrySpawnEntity", BindingFlags.Instance | BindingFlags.NonPublic);
                OnTrySpawnEntityDelegate = fieldInfo.GetValue(__instance) as MulticastDelegate;

                dirty = false;
            }

            if (OnTrySpawnEntityDelegate == null)
			{
				return true;
			}

			Delegate[] invocationList = OnTrySpawnEntityDelegate.GetInvocationList();

			for (int i = 0; i < invocationList.Length; i++)
			{
				TrySpawnEntityDelegate TrySpawnEntityDelegate = (TrySpawnEntityDelegate)invocationList[i];
				try
				{
					__result = __result && TrySpawnEntityDelegate(ref properties, position, herdId);
				}
				catch (Exception ex)
				{
					ServerMain.Logger.Error("Exception thrown during handling event OnTrySpawnEntity: {0}", ex);
					return false;
				}
			}

            return false;
        }

        public static void Dispose()
        {
            OnTrySpawnEntityDelegate = null;
            dirty = true;
        }
    }
}