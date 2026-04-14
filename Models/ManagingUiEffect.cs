using Dismantle_Plugin__UnturnedStore_Version_.Models.Configuration;
using Dismantle_Plugin__UnturnedStore_Version_.Models.Interfaces;
using Dismantle_Plugin__UnturnedStore_Version_.Models.Sub_Models;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dismantle_Plugin__UnturnedStore_Version_.Models
{
    public class ManagingUiEffect : IUiSession
    {
        public bool IsActive { get { return Owner != null || IsLoadingConstructor; } }

        public Player Owner { get; set; }
        public ContainerModel Data { get; set; }

        public byte CurrentRow { get; set; }
        public bool IsLoadingConstructor { get; set; }
        public ManagingUiEffect()
        {
            IsLoadingConstructor = false;
        }
        public ManagingUiEffect(Player owner, ContainerModel data)
        {
            IsLoadingConstructor = true;
            Owner = owner;
            Data = data;
            CurrentRow = 0;
            Instantiate();
            IsLoadingConstructor = false;
        }

        public void Instantiate()
        {
            Owner.enablePluginWidgetFlag(EPluginWidgetFlags.Modal);
            EffectManager.sendUIEffect(17321, 17321, Owner.channel.owner.transportConnection, true);
            UpdateButtons();
            UpdateLoadBar();
            UpdateItems();
        }
        public void EndSession()
        {
            Owner.disablePluginWidgetFlag(EPluginWidgetFlags.Modal);
            EffectManager.askEffectClearByID(17321, Owner.channel.owner.transportConnection);
            Owner = null;
        }
        public void EffectManager_OnButtonClicked(string ButtonName)
        {
            if (ButtonName == "Close")
                EndSession();
            else if (ButtonName == "All")
            {
                for (byte indexX = 0; indexX < Data.Inventory.SizeX; indexX++)
                {
                    SerializableItem TargetItem = Data.Inventory._items[CurrentRow, indexX];
                    if (TargetItem == null)
                        continue;

                    if (!Owner.inventory.tryAddItem(TargetItem.ConvertToItem(), false))
                        ItemManager.dropItem(TargetItem.ConvertToItem(), Owner.transform.position, false, true, false);
                    Data.Inventory.RemoveItem(indexX, CurrentRow);
                }

                if (CurrentRow == Data.Inventory.SizeY)
                    CurrentRow--;

                UpdateLoadBar();
                UpdateItems();
                UpdateButtons();
            }
            else if (ButtonName.StartsWith("ItemInteractable [Y"))
            {
                byte UiIndex = byte.Parse(ButtonName[20].ToString());
                SerializableItem TargetItem = Data.Inventory._items[CurrentRow, UiIndex - 1];
                if (TargetItem != null)
                {
                    if (!Owner.inventory.tryAddItem(TargetItem.ConvertToItem(), false))
                        ItemManager.dropItem(TargetItem.ConvertToItem(), Owner.transform.position, false, true, false);
                    Data.Inventory.RemoveItem((byte)(UiIndex - 1), CurrentRow);
                }

                if (CurrentRow == Data.Inventory.SizeY)
                    CurrentRow--;

                UpdateLoadBar();
                UpdateItems();
                UpdateButtons();
            }
            else if (ButtonName == "Left")
            {
                CurrentRow--;
                UpdateItems();
                UpdateButtons();
            }
            else if (ButtonName == "Right")
            {
                CurrentRow++;
                UpdateItems();
                UpdateButtons();
            }
            else if (ButtonName == "Start")
            {
                InteractableVehicle TargetVehicle = Data.Vehicles.FirstOrDefault(X => X != null && PluginCore.Instance.Configuration.Instance.VehicleGroups.Any(Y => Y.IncludeVehicle(X.id)));
                if (TargetVehicle == null)
                {
                    UpdateButtons();
                    return;
                }
                ChatManager.serverSendMessage(PluginCore.Instance.Translate("StartedDismantle", TargetVehicle.asset.FriendlyName), Color.white, null, Owner.channel.owner, EChatMode.SAY, null, true);
                Data.Vehicles.Remove(TargetVehicle);
                Data.ActiveDismantle = new DismantleSession(TargetVehicle, Data.ContainerID);
                EndSession();
            }
        }
        public void EffectManager_OnInputFieldSubmitted(string FieldName, string InputText)
        {

        }

        public void UpdateItems()
        {
            for (byte ItemIndex = 0; ItemIndex < 5; ItemIndex++)
            {
                byte UiIndex = (byte)(ItemIndex + 1);
                bool FindedItem = Data.Inventory.GetItem(ItemIndex, CurrentRow, out SerializableItem TargetItem);
                EffectManager.sendUIEffectText(17321, Owner.channel.owner.transportConnection, true, $"Amount [Y,{UiIndex}]", (FindedItem ? TargetItem.Amount.ToString() : "X"));
                if (FindedItem)
                {
                    ItemImage Image = PluginCore.Instance.Configuration.Instance.Images.FirstOrDefault(X => X.ItemID == TargetItem.ItemID);
                    if (Image != null)
                        EffectManager.sendUIEffectImageURL(17321, Owner.channel.owner.transportConnection, true, $"InsertImage [Y,{UiIndex}]", Image.ImageURL);
                }
                EffectManager.sendUIEffectVisibility(17321, Owner.channel.owner.transportConnection, true, $"Loading [Y,{UiIndex}]", !FindedItem);
                EffectManager.sendUIEffectVisibility(17321, Owner.channel.owner.transportConnection, true, $"ItemInteractable [Y,{UiIndex}]", FindedItem);
            }
        }
        public void UpdateLoadBar()
        {
            byte Porcentage = (byte)(100 - (Data.Inventory.SizeY / 2.55f));
            for (byte UiBarIndex = 100; UiBarIndex > 0; UiBarIndex--)
                EffectManager.sendUIEffectVisibility(17321, Owner.channel.owner.transportConnection, true, $"FillerLoad{UiBarIndex}", Porcentage >= UiBarIndex);
        }
        public void UpdateButtons()
        {
            EffectManager.sendUIEffectText(17321, Owner.channel.owner.transportConnection, true, $"IndexText", $"{CurrentRow + 1}\n{Data.Inventory.SizeY}");
            EffectManager.sendUIEffectVisibility(17321, Owner.channel.owner.transportConnection, true, "Start Disabled", Data.Vehicles.Count(X => X != null && PluginCore.Instance.Configuration.Instance.VehicleGroups.Any(Y => Y.IncludeVehicle(X.id))) == 0 || (Data.ActiveDismantle != null && Data.ActiveDismantle.IsActive));
            EffectManager.sendUIEffectVisibility(17321, Owner.channel.owner.transportConnection, true, "Left Disabled", CurrentRow == 0);
            EffectManager.sendUIEffectVisibility(17321, Owner.channel.owner.transportConnection, true, "Right Disabled", CurrentRow == (Data.Inventory.SizeY - 1));
            EffectManager.sendUIEffectVisibility(17321, Owner.channel.owner.transportConnection, true, "All Disabled", Data.Inventory.GetItemCount() == 0);
        }
    }
}
