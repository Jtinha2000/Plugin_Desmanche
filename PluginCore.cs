using Dismantle_Plugin__UnturnedStore_Version_.Database;
using Dismantle_Plugin__UnturnedStore_Version_.Models;
using Dismantle_Plugin__UnturnedStore_Version_.Models.Interfaces;
using HarmonyLib;
using JetBrains.Annotations;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace Dismantle_Plugin__UnturnedStore_Version_
{
    public class PluginCore : RocketPlugin<Configuration>
    {
        public const ushort ContainerBarricadeID = 35822;

        internal static PluginCore Instance { get; set; }
        public List<LockpickSession> Lockpicks { get; set; }
        internal Harmony HarmonyInstance { get; set; }
        internal IDatabase Database { get; set; }
        protected override void Load()
        {
            Lockpicks = new List<LockpickSession>();
            Instance = this;
            HarmonyInstance = new Harmony("CachorroComunista.Dismantle");
            HarmonyInstance.PatchAll();
            Database = new JsonDatabase();
            Database.Load();

            PlayerEquipment.OnUseableChanged_Global += PlayerEquipment_OnUseableChanged_Global;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerDead += UnturnedPlayerEvents_OnPlayerDead;
            EffectManager.onEffectButtonClicked += EffectManager_OnButtonClicked;
            EffectManager.onEffectTextCommitted += EffectManager_OnInputFieldSubmitted;
            PlayerEquipment.OnPunch_Global += OnPlayerPunched;
            InteractableVehicle.OnHealthChanged_Global += InteractableVehicle_OnHealthChanged_Global;
            VehicleManager.onEnterVehicleRequested += VehicleManager_onEnterVehicleRequested;
            BarricadeManager.onBarricadeSpawned += OnBarricadeSpawned;
            if (Level.isLoaded)
                Level_OnLevelLoaded(0);
            else
                Level.onLevelLoaded += Level_OnLevelLoaded;
        }
        protected override void Unload()
        {
            HarmonyInstance.UnpatchAll("CachorroComunista.Dismantle");
            Database.Save();
            Database.GetDataToList().ForEach(X => X.DeleteContainer());
            while (Lockpicks.Count > 0)
                Lockpicks[0].Endsession(false);

            PlayerEquipment.OnUseableChanged_Global -= PlayerEquipment_OnUseableChanged_Global;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerDead -= UnturnedPlayerEvents_OnPlayerDead;
            EffectManager.onEffectButtonClicked -= EffectManager_OnButtonClicked;
            EffectManager.onEffectTextCommitted -= EffectManager_OnInputFieldSubmitted;
            PlayerEquipment.OnPunch_Global -= OnPlayerPunched;
            InteractableVehicle.OnHealthChanged_Global -= InteractableVehicle_OnHealthChanged_Global;
            VehicleManager.onEnterVehicleRequested -= VehicleManager_onEnterVehicleRequested;
            BarricadeManager.onBarricadeSpawned -= OnBarricadeSpawned;
            Level.onLevelLoaded -= Level_OnLevelLoaded;
        }
        public override TranslationList DefaultTranslations => new TranslationList
        {
            { "OpenUiError1", "<color=red>[Dismantle System]</color> Another player is already using the container, please wait..." },
            { "StartedDismantle", "<color=blue>[Dismantle System]</color> The dismantling of {0} has started successfully!" },
            { "NoLockpickItem", "<color=red>[Dismantle System]</color> You need to hold a valid object with your hands!" },
            { "MinOfficerAmount", "<color=red>[Dismantle System]</color> At least {0} police officers online required!" },
            { "LockpickStarted", "<color=blue>[Dismantle System]</color> You are trying to steal the vehicle... The lock should break in {0} seconds..." },
            { "BlacklistedVehicle", "<color=red>[Dismantle System]</color> This vehicle has a very advanced lock..." },
            { "NeedsBeLookin", "<color=red>[Dismantle System]</color> You need to be looking at a vehicle..." },
            { "DeadVehicle", "<color=red>[Dismantle System]</color> The vehicle is destroyed..." },
            { "AdviseRoberry", "<color=blue>[Dismantle System]</color> {1} failed lockpicking a vehicle next to <color=red>{0}</color>!..."  },
            { "StopLooking", "<color=red>[Dismantle System]</color> The lockpick failed. Never look away!" },
            { "LockpickFailed", "<color=red>[Dismantle System]</color> The lockpick failed!" },
            { "UseableDrop", "<color=red>[Dismantle System]</color> lockpick failed. Never weaken your hands!" }
        };

        //Ui
        public void OnPlayerPunched(PlayerEquipment Player, EPlayerPunch PunchType)
        {
            RaycastInfo Info = DamageTool.raycast(new UnityEngine.Ray(Player.player.look.transform.position, Player.player.look.transform.forward), 3.5f, RayMasks.BARRICADE_INTERACT, Player.player);
            BarricadeDrop Target = BarricadeManager.FindBarricadeByRootTransform(Info.transform);
            if (Target == null || !Database.ContainerExists(Target) || (Configuration.Instance.NeedsBeOnGroup && Target.GetServersideData().owner != Player.player.channel.owner.playerID.steamID.m_SteamID && (Target.GetServersideData().group == 0 || Target.GetServersideData().group != Player.player.quests.groupID.m_SteamID)))
                return;

            ContainerModel ContainerData = Database.GetContainerData(Target);
            if (ContainerData.ActiveUI != null && ContainerData.ActiveUI.IsActive)
            {
                ChatManager.serverSendMessage(Translate("OpenUiError1"), Color.white, null, Player.player.channel.owner, EChatMode.SAY, null, true);
                return;
            }

            if (ContainerData.ActiveDismantle != null && ContainerData.ActiveDismantle.IsActive)
                return;
            else
                ContainerData.ActiveUI = new ManagingUiEffect(Player.player, ContainerData);
        }
        public void EffectManager_OnButtonClicked(Player Player, string ButtonName)
        {
            if (!Database.ContainerExists(Player))
                return;
            ContainerModel Container = Database.GetContainerData(Player);

            Container.ActiveUI.EffectManager_OnButtonClicked(ButtonName);
        }
        public void EffectManager_OnInputFieldSubmitted(Player Player, string FieldName, string InputText)
        {
            if (!Database.ContainerExists(Player))
                return;
            ContainerModel Container = Database.GetContainerData(Player);

            Container.ActiveUI.EffectManager_OnInputFieldSubmitted(FieldName, InputText);
        }
        private void UnturnedPlayerEvents_OnPlayerDead(Rocket.Unturned.Player.UnturnedPlayer player, Vector3 position)
            => Events_OnPlayerDisconnected(player);
        private void Events_OnPlayerDisconnected(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            if (Database.ContainerExists(player.Player))
                Database.GetContainerData(player.Player).ActiveUI.EndSession();
            if (Lockpicks.Any(X => player.Player == X.Owner))
                Lockpicks.First(X => player.Player == X.Owner).Endsession();
        }

        //Sync Barricades
        private void Level_OnLevelLoaded(int __)
        {
            List<ContainerModel> RemainingContainers = new List<ContainerModel>(Database.GetDataToList());
            foreach (BarricadeRegion Region in BarricadeManager.BarricadeRegions)
                if (Region != null)
                    foreach (BarricadeDrop Drop in Region.drops)
                    {
                        if (Drop.asset.id == ContainerBarricadeID)
                        {
                            ContainerModel Finded = RemainingContainers.FirstOrDefault(X => X.IsBarricadeDrop(Drop));
                            if (Finded != null)
                            {
                                Finded.InitCollider();
                                RemainingContainers.Remove(Finded);
                                if (RemainingContainers.Count == 0)
                                    return;
                            }
                            else
                                Database.AddContainer(Drop);
                        }
                    }
            RemainingContainers.ForEach(X => Database.RemoveContainer(X));
        }
        public void OnBarricadeSpawned(BarricadeRegion region, BarricadeDrop drop)
        {
            if (drop.asset.id == ContainerBarricadeID)
                Database.AddContainer(drop);
        }

        //Sync Vehicles
        private void InteractableVehicle_OnHealthChanged_Global(InteractableVehicle Vehicle)
        {
            if (Vehicle.health > 0)
                return;

            if (Database.SessionExists(Vehicle))
                Database.GetDismantleData(Vehicle).EndDismantle();
            if (Lockpicks.Any(X => X.TargetVehicle == Vehicle))
                Lockpicks.First(X => X.TargetVehicle == Vehicle).Endsession();
        }
        private void VehicleManager_onEnterVehicleRequested(Player player, InteractableVehicle Vehicle, ref bool shouldAllow)
        {
            if (Database.SessionExists(Vehicle))
                shouldAllow = false;
        }

        //Sync Hand Lockpicking
        private void PlayerEquipment_OnUseableChanged_Global(PlayerEquipment PlayerEq)
        {
            LockpickSession Session = Lockpicks.FirstOrDefault(X => X.Owner == PlayerEq.player);
            if (Session == null)
                return;

            ChatManager.serverSendMessage(Translate("UseableDrop"), Color.white, null, PlayerEq.player.channel.owner, EChatMode.SAY, null, true);
            Session.Endsession();
            Session.RemoveLockpickItem();
        }
    }
}
