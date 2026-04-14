using Dismantle_Plugin__UnturnedStore_Version_.Models;
using Newtonsoft.Json;
using SDG.Unturned;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dismantle_Plugin__UnturnedStore_Version_.Database
{
    public class JsonDatabase : IDatabase
    {
        private List<ContainerModel> _currentList;
        public void Load()
        {
            if (!File.Exists(PluginCore.Instance.Directory + @"\Database.json"))
                File.Create(PluginCore.Instance.Directory + @"\Database.json");
            else
                _currentList = JsonConvert.DeserializeObject<List<ContainerModel>>(File.ReadAllText(PluginCore.Instance.Directory + @"\Database.json"));
            if (_currentList is null)
                _currentList = new List<ContainerModel>();
        }
        public void Save()
        {
            using (StreamWriter Writer = new StreamWriter(PluginCore.Instance.Directory + @"\Database.json", false))
            {
                Writer.Write(JsonConvert.SerializeObject(_currentList));
            }
        }
        public void AddContainer(BarricadeDrop Target)
        {
            _currentList.Add(new ContainerModel(Target));
        }
        public void RemoveContainer(ContainerModel Target)
        {
            _currentList.Remove(Target);
        }

        public List<ContainerModel> GetDataToList() =>
             _currentList;
        public bool ContainerExists(BarricadeDrop ContainerDrop) =>
            _currentList.Any(X => X.ContainerID == ContainerDrop.GetNetId().id);
        public bool SessionExists(InteractableVehicle Vehicle) =>
            _currentList.Any(X => X.ActiveDismantle != null && X.ActiveDismantle.IsActive && X.ActiveDismantle.Vehicle == Vehicle);
        public bool ContainerExists(Player UiSessionOwner) =>
            _currentList.Any(X => X.ActiveUI != null && X.ActiveUI.Owner == UiSessionOwner && X.ActiveUI.IsActive);
        public ContainerModel GetContainerData(uint BarricadeID) =>
            _currentList.First(X => X.ContainerID == BarricadeID);
        public ContainerModel GetContainerData(BarricadeDrop ContainerDrop) =>
            GetContainerData(ContainerDrop.GetNetId().id);
        public ContainerModel GetContainerData(Player UiSessionOwner) =>
            _currentList.First(X => X.ActiveUI != null && X.ActiveUI.Owner == UiSessionOwner && X.ActiveUI.IsActive);
        public DismantleSession GetDismantleData(InteractableVehicle Vehicle) =>
            _currentList.First(X => X.ActiveDismantle != null && X.ActiveDismantle.IsActive && X.ActiveDismantle.Vehicle == Vehicle).ActiveDismantle;
    }
}
