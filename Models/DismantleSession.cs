using Dismantle_Plugin__UnturnedStore_Version_.Models.Configuration;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = System.Random;

namespace Dismantle_Plugin__UnturnedStore_Version_.Models
{
    public class DismantleSession
    {
        public bool IsActive { get { return Effects != null || IsLoadingConstructor; } }
        private bool IsLoadingConstructor { get; set; }
        public uint ContainerID { get; set; }
        public InteractableVehicle Vehicle { get; set; }
        public VehicleGroup VehicleGroup { get; set; }
        public Coroutine Effects { get; set; }
        public float EffectsDuration { get; set; }
        public DismantleSession()
        {
            IsLoadingConstructor = false;
        }
        public DismantleSession(InteractableVehicle vehicle, uint container)
        {
            IsLoadingConstructor = true;
            Vehicle = vehicle;
            ContainerID = container;
            VehicleGroup = PluginCore.Instance.Configuration.Instance.VehicleGroups.OrderByDescending(X => X.Priority).FirstOrDefault(X => X.IncludeVehicle(Vehicle.id));
            EffectsDuration = VehicleGroup.GetDuration();
            StartDismantle();
            IsLoadingConstructor = false;
        }

        public void StartDismantle()
        {
            //Remove Players
            bool ShouldScream = false;
            for (byte PassengerIndex = 0; PassengerIndex < Vehicle.passengers.Length; PassengerIndex++)
            {
                Passenger Passenger = Vehicle.passengers[PassengerIndex];
                if (Passenger.player == null)
                    continue;

                if (PluginCore.Instance.Configuration.Instance.EffectsConfig.KillPassengers)
                {
                    Passenger.player.player.life.askDamage(byte.MaxValue, Vector3.zero, EDeathCause.BONES, ELimb.SKULL, CSteamID.Nil, out EPlayerKill Kill);
                    ShouldScream = true;
                }
                if (!PluginCore.Instance.Configuration.Instance.EffectsConfig.KillPassengers || Passenger.player != null)
                    Vehicle.removePlayer(PassengerIndex, Vehicle.transform.position, (byte)Passenger.player.player.look.yaw, true);
            }
            //Audio
            if (ShouldScream)
            {
                EffectAsset EffectAsset = Assets.find(EAssetType.EFFECT, PluginCore.Instance.Configuration.Instance.EffectsConfig.ScreamAudioID) as EffectAsset;
                if (EffectAsset != null)
                {
                    TriggerEffectParameters EffectParameters = GetParameters(EffectAsset);
                    EffectManager.triggerEffect(EffectParameters);
                }
            }
            //Freeze Vehicle
            if (PluginCore.Instance.Configuration.Instance.EffectsConfig.FreezeVehicle)
                foreach (Collider Collisor in Vehicle.vehicleColliders)
                    if (Collisor != null && Collisor.attachedRigidbody != null)
                        Collisor.attachedRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            //Close Container Door
            BarricadeManager.ServerSetFireLit(PluginCore.Instance.Database.GetContainerData(ContainerID).Drop.interactable as InteractableFire, true);
            //Start Effects
            Effects = PluginCore.Instance.StartCoroutine(EffectsCycle());
        }
        public void EndDismantle()
        {
            if (Effects != null)
            {
                PluginCore.Instance.StopCoroutine(Effects);
                Effects = null;
            }
            BarricadeDrop Drop = PluginCore.Instance.Database.GetContainerData(ContainerID).Drop;
            if (Drop != null)
                BarricadeManager.ServerSetFireLit(Drop.interactable as InteractableFire, false);
            if (Vehicle != null)
            {
                if (PluginCore.Instance.Configuration.Instance.EffectsConfig.FreezeVehicle)
                    foreach (Collider Collisor in Vehicle.vehicleColliders)
                        if (Collisor != null && Collisor.attachedRigidbody != null)
                            Collisor.attachedRigidbody.constraints = RigidbodyConstraints.None;
                VehicleManager.askVehicleDestroy(Vehicle);
            }
        }
        public TriggerEffectParameters GetParameters(EffectAsset Asset = null)
        {
            TriggerEffectParameters EffectParameters = new TriggerEffectParameters(Asset);
            EffectParameters.relevantDistance = 25f;
            EffectParameters.wasInstigatedByPlayer = true;
            EffectParameters.shouldReplicate = true;
            EffectParameters.position = Vehicle.transform.position;
            return EffectParameters;
        }

        public IEnumerator EffectsCycle()
        {
            Random Randomizer = new Random();
            ushort VehicleInitialLife = Vehicle.health;
            EffectAsset AssetEletricy1 = Assets.find(EAssetType.EFFECT, 61) as EffectAsset;
            EffectAsset AssetMetal1 = Assets.find(EAssetType.EFFECT, 18) as EffectAsset;
            EffectAsset AssetMetal2 = Assets.find(EAssetType.EFFECT, 12) as EffectAsset;
            TriggerEffectParameters EffectParameters = GetParameters();
            while (EffectsDuration > 0)
            {
                if (PluginCore.Instance.Configuration.Instance.EffectsConfig.BatteryDecreaseChance >= Randomizer.Next(1, 100) && Vehicle.batteryCharge > 0)
                {
                    //Remove Battery Charge
                    Vehicle.batteryCharge = (ushort)(Vehicle.batteryCharge < 1000 ? 0 : Vehicle.batteryCharge - 1000);
                    Vehicle.sendBatteryChargeUpdate();

                    //Battery Effect
                    EffectParameters.asset = AssetEletricy1;
                    EffectManager.triggerEffect(EffectParameters);
                }
                else if (PluginCore.Instance.Configuration.Instance.EffectsConfig.BreakTireChance >= Randomizer.Next(1, 100) && Vehicle.tires.Any(X => X != null && X.isAlive))
                {
                    //Damage Tire
                    for (int SimulatedIndex = Vehicle.tires.Length - 1; SimulatedIndex >= 0; SimulatedIndex--)
                        if (Vehicle.tires[SimulatedIndex] != null && Vehicle.tires[SimulatedIndex].isAlive)
                        {
                            VehicleManager.damageTire(Vehicle, SimulatedIndex);
                            break;
                        }
                }
                else if (PluginCore.Instance.Configuration.Instance.EffectsConfig.HealthDecreaseChance >= Randomizer.Next(1, 100) && Vehicle.health > 5)
                {
                    //Damage Vehicle
                    VehicleManager.sendVehicleHealth(Vehicle, (ushort)Math.Max(5, Vehicle.health - (VehicleInitialLife / 10)));

                    //Health Effect
                    if (50 > Randomizer.Next(1, 100))
                        EffectParameters.asset = AssetMetal1;
                    else
                        EffectParameters.asset = AssetMetal2;
                    EffectManager.triggerEffect(EffectParameters);
                }
                yield return new WaitForSeconds(Math.Min(EffectsDuration, PluginCore.Instance.Configuration.Instance.EffectsConfig.TickLenght));
                EffectsDuration -= PluginCore.Instance.Configuration.Instance.EffectsConfig.TickLenght;
                Randomizer = new Random();
            }
            //EndEffects
            EffectParameters.asset = Assets.find(EAssetType.EFFECT, 53) as EffectAsset;
            EffectManager.triggerEffect(EffectParameters);

            //AddItems
            ContainerModel Data = PluginCore.Instance.Database.GetContainerData(ContainerID);
            if (VehicleGroup != null)
                foreach (ConfigItem Drop in VehicleGroup.Drops)
                    if (Drop.Probability >= Randomizer.Next(0, 100))
                    {
                        int TargetAmount = Randomizer.Next(Drop.MinAmount, Drop.MaxAmount);
                        for (int ActualAmount = 0; ActualAmount < TargetAmount; ActualAmount++)
                            Data.Inventory.TryAddItem(Drop.ConvertToItem());
                    }

            //Register End
            EndDismantle();
        }
    }
}
