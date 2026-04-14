using Dismantle_Plugin__UnturnedStore_Version_.Models;
using Dismantle_Plugin__UnturnedStore_Version_.Models.Interfaces;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dismantle_Plugin__UnturnedStore_Version_.Database
{
    internal interface IDatabase
    {
        void Load();
        void Save();
        List<ContainerModel> GetDataToList();

        ContainerModel GetContainerData(uint BarricadeID);
        ContainerModel GetContainerData(BarricadeDrop ContainerDrop);
        bool ContainerExists(BarricadeDrop ContainerDrop);
        void AddContainer(BarricadeDrop Target);
        void RemoveContainer(ContainerModel Target);

        DismantleSession GetDismantleData(InteractableVehicle Vehicle);
        bool SessionExists(InteractableVehicle Vehicle);

        ContainerModel GetContainerData(Player UiSessionOwner);
        bool ContainerExists(Player UiSessionOwner);
    }
}
