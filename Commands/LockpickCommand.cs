using Dismantle_Plugin__UnturnedStore_Version_.Models;
using Dismantle_Plugin__UnturnedStore_Version_.Models.Configuration;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Extensions;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dismantle_Plugin__UnturnedStore_Version_.Commands
{
    public class LockpickCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "Lockpick";

        public string Help => "";

        public string Syntax => "/Lockpick";

        public List<string> Aliases => new List<string> { "Roubar", "Picklock", "Steal" };

        public List<string> Permissions => new List<string> { "Lockpick.Command" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Player Caller = PlayerTool.getPlayer(new Steamworks.CSteamID(ulong.Parse(caller.Id)));
            if (PluginCore.Instance.Lockpicks.Any(X => X.Owner == Caller))
                return;
            if(Provider.clients.Count(X => X.ToUnturnedPlayer().GetPermissions().Any(Z => Z.Name == PluginCore.Instance.Configuration.Instance.PolicePermission)) < PluginCore.Instance.Configuration.Instance.MinOfficerLockpickNeeded)
            {
                ChatManager.serverSendMessage(PluginCore.Instance.Translate("MinOfficerAmount", PluginCore.Instance.Configuration.Instance.MinOfficerLockpickNeeded), Color.white, null, Caller.channel.owner, EChatMode.SAY, null, true);
                return;
            }

            InteractableVehicle TargetVehicle = DamageTool.raycast(new Ray(Caller.look.aim.position, Caller.look.aim.forward), 4f, RayMasks.VEHICLE, Caller).vehicle;
            if(TargetVehicle == null)
            {
                ChatManager.serverSendMessage(PluginCore.Instance.Translate("NeedsBeLookin"), Color.white, null, Caller.channel.owner, EChatMode.SAY, null, true);
                return;
            }
            if (TargetVehicle.isDead)
            {
                ChatManager.serverSendMessage(PluginCore.Instance.Translate("DeadVehicle"), Color.white, null, Caller.channel.owner, EChatMode.SAY, null, true);
                return;
            }
            if (!TargetVehicle.isLocked || PluginCore.Instance.Database.SessionExists(TargetVehicle))
                return;

            if (PluginCore.Instance.Configuration.Instance.BlacklistedLockpickVehicles.Contains(TargetVehicle.id))
            {
                ChatManager.serverSendMessage(PluginCore.Instance.Translate("BlacklistedVehicle"), Color.white, null, Caller.channel.owner, EChatMode.SAY, null, true);
                return;
            }

            foreach (LockpickConfig Config in PluginCore.Instance.Configuration.Instance.Lockpicks)
            {
                if (Caller.equipment.itemID != Config.ItemID || !Config.IncludeVehicle(TargetVehicle.id) || !Caller.channel.owner.ToUnturnedPlayer().HasPermission(Config.NeededPermission))
                    continue;

                PluginCore.Instance.Lockpicks.Add(new LockpickSession(Caller, TargetVehicle, Config));
                ChatManager.serverSendMessage(PluginCore.Instance.Translate("LockpickStarted", Config.MaxTime), Color.white, null, Caller.channel.owner, EChatMode.SAY, null, true);
                return;
            }
            ChatManager.serverSendMessage(PluginCore.Instance.Translate("NoLockpickItem"), Color.white, null, Caller.channel.owner, EChatMode.SAY, null, true);
        }
    }
}
