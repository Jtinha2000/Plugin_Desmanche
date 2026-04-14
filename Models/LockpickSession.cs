using Dismantle_Plugin__UnturnedStore_Version_.Models.Configuration;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Extensions;
using SDG.Framework.Devkit;
using SDG.Unturned;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dismantle_Plugin__UnturnedStore_Version_.Models
{
    public class LockpickSession
    {
        public Player Owner { get; set; }
        public InteractableVehicle TargetVehicle { get; set; }
        public LockpickConfig Config { get; set; }
        public float RemainingTime { get; set; }
        public Coroutine Checker { get; set; }
        public LockpickSession()
        {

        }
        public LockpickSession(Player owner, InteractableVehicle targetVehicle, LockpickConfig config)
        {
            Owner = owner;
            TargetVehicle = targetVehicle;
            Config = config;
            RemainingTime = Config.GetDuration();
            Checker = PluginCore.Instance.StartCoroutine(CheckerIterator());
        }
        public void RemoveLockpickItem()
        {
            for (int Page = 0; Page <= 1; Page++)
            {
                ItemJar Item = Owner.inventory.items[Page].items.FirstOrDefault(X => X.item.id == Config.ItemID);
                if (Item == null)
                    continue;
                Owner.inventory.items[Page].removeItem(Owner.inventory.items[Page].getIndex(Item.x, Item.y));
                break;
            }
        }
        public void RingAlarm()
        {
            EffectAsset Asset = Assets.find(EAssetType.EFFECT, PluginCore.Instance.Configuration.Instance.AlarmEffectID) as EffectAsset;
            if(Asset != null)
            {
                TriggerEffectParameters parames = new TriggerEffectParameters(Asset);
                parames.position = TargetVehicle.transform.position;
                parames.direction = Vector3.up;
                parames.shouldReplicate = true;
                parames.relevantDistance = PluginCore.Instance.Configuration.Instance.AlarmEffectRange;
                EffectManager.triggerEffect(parames);
            }

            string NextNodeName = (LevelNodes.nodes.OrderBy(X => Vector3.Distance(X.point, TargetVehicle.transform.position)).FirstOrDefault(X => X.type == ENodeType.LOCATION) as LocationNode).name;
            foreach (SteamPlayer Client in Provider.clients)
            {
                if (Client.player != Owner && Client.ToUnturnedPlayer().GetPermissions().Any(X => X.Name == PluginCore.Instance.Configuration.Instance.PolicePermission))
                    ChatManager.serverSendMessage(PluginCore.Instance.Translate("AdviseRoberry", NextNodeName, Owner.channel.owner.playerID.characterName), Color.white, null, Client, EChatMode.SAY, null, true);
            }
        }
        public void Endsession(bool Failed = true)
        {
            PluginCore.Instance.StopCoroutine(Checker);
            PluginCore.Instance.Lockpicks.Remove(this);
        }
        public IEnumerator CheckerIterator()
        {
            while (RemainingTime > 0)
            {
                RemainingTime -= 0.25f;
                if (DamageTool.raycast(new Ray(Owner.look.aim.position, Owner.look.aim.forward), 4f, RayMasks.VEHICLE, Owner).vehicle != TargetVehicle)//FailStopLooking
                {
                    ChatManager.serverSendMessage(PluginCore.Instance.Translate("StopLooking"), Color.white, null, Owner.channel.owner, EChatMode.SAY, null, true);
                    Endsession();
                    RemoveLockpickItem();
                }
                yield return new WaitForSeconds(0.25f);
            }

            System.Random Randomizer = new System.Random();
            bool ShouldRemoveItem = true;
            if (Config.SucessProbability >= Randomizer.Next(0, 100))
            {
                VehicleManager.unlockVehicle(TargetVehicle, Owner);
                if (Config.AlarmProbability >= Randomizer.Next(0, 100))
                    RingAlarm();
                if (Config.ConsumeJustOnFail)
                    ShouldRemoveItem = false;
            }
            else
            {
                ChatManager.serverSendMessage(PluginCore.Instance.Translate("LockpickFailed"), Color.white, null, Owner.channel.owner, EChatMode.SAY, null, true);
                if (Config.AlarmOnFail || Config.AlarmProbability >= Randomizer.Next(0, 100))
                    RingAlarm();
            }
            Endsession(false);
            if (ShouldRemoveItem)
                RemoveLockpickItem();
        }
    }
}
