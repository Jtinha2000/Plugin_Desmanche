using Dismantle_Plugin__UnturnedStore_Version_.Models.Interfaces;
using Dismantle_Plugin__UnturnedStore_Version_.Models.Sub_Models;
using Newtonsoft.Json;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Dismantle_Plugin__UnturnedStore_Version_.Models
{
    public class ContainerModel
    {
        public uint ContainerID { get; set; }
        public ContainerInventory Inventory { get; set; }

        [JsonIgnore]
        public BarricadeDrop Drop { get; set; }
        [JsonIgnore]
        private GameObject _colliderObject { get; set; }

        [JsonIgnore]
        public bool IsInstantiated { get { return Drop != null && _colliderObject != null; } }
        [JsonIgnore]
        public List<InteractableVehicle> Vehicles { get; set; } = new List<InteractableVehicle>();
        [JsonIgnore]
        public DismantleSession ActiveDismantle { get; set; } = null; //pleonasmo
        [JsonIgnore]
        public IUiSession ActiveUI { get; set; } = null; //pleonasmo
        public ContainerModel()
        {

        }
        public ContainerModel(NetId containerID)
        {
            ContainerID = containerID.id;
            Inventory = new ContainerInventory(5, 1);
        }
        public ContainerModel(BarricadeDrop drop) : this(drop.GetNetId())
        {
            Drop = drop;
            InitCollider();
        }

        public void InitCollider()
        {
            if (_colliderObject != null)
                GameObject.Destroy(_colliderObject);

            _colliderObject = new GameObject($"ContainerModel_ColliderObject_{ContainerID}");
            _colliderObject.layer = 30;
            _colliderObject.transform.position = Drop.model.transform.position;
            _colliderObject.transform.rotation = Drop.model.transform.rotation;

            MeshCollider Collider = _colliderObject.AddComponent<MeshCollider>();
            Collider.sharedMesh = Drop.model.gameObject.GetComponent<MeshCollider>().sharedMesh;
            Collider.convex = true;
            Collider.isTrigger = true;

            ColliderTriggerListener Script = _colliderObject.AddComponent<ColliderTriggerListener>();
            Script.Container = this;
        }
        public bool IsBarricadeDrop(BarricadeDrop TDrop)
        {
            if (TDrop != null && TDrop.GetNetId().id == ContainerID)
            {
                Drop = TDrop;
                return true;
            }
            return false;
        }
        public void DeleteContainer(bool Permanently = false)
        {
            if (ActiveDismantle != null && ActiveDismantle.IsActive)
                ActiveDismantle.EndDismantle();
            if (ActiveUI != null && ActiveUI.IsActive)
                ActiveUI.EndSession();
            GameObject.Destroy(_colliderObject);

            if (Permanently)
            {
                if (Level.isLoaded)
                    for (byte indexY = 0; indexY < Inventory.SizeY; indexY++)
                        for (byte indexX = 0; indexX < Inventory.SizeX; indexX++)
                            if (Inventory._items[indexY, indexX] != null)
                            {
                                ItemManager.dropItem(Inventory._items[indexY, indexX].ConvertToItem(), Drop.GetServersideData().point, false, true, false);
                                Inventory.RemoveItem(indexX, indexY);
                            }
                PluginCore.Instance.Database.RemoveContainer(this);
            }
        }
    }
}
