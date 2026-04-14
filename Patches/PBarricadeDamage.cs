using Dismantle_Plugin__UnturnedStore_Version_.Models;
using HarmonyLib;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dismantle_Plugin__UnturnedStore_Version_.Patches
{
    [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.destroyBarricade), new Type[] { typeof(BarricadeDrop), typeof(byte), typeof(byte), typeof(ushort) })]
    public class PBarricadeDamage
    {
        [HarmonyPrefix]
        public static void BarricadeDestroy(BarricadeDrop barricade, byte x, byte y, ushort plant)
        {
            if (!PluginCore.Instance.Database.ContainerExists(barricade))
                return;

            ContainerModel Data = PluginCore.Instance.Database.GetContainerData(barricade.GetNetId().id);
            Data.DeleteContainer(true);
        }
    }
}
