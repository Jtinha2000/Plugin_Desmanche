using Newtonsoft.Json;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Dismantle_Plugin__UnturnedStore_Version_.Models.Configuration
{
    public class VehicleGroup
    {
        public string GroupName { get; set; }
        public byte Priority { get; set; }
        [XmlArrayItem("VehicleID")]
        public List<ushort> Vehicles { get; set; }
        public List<ConfigItem> Drops { get; set; }
        public ushort MinDuration { get; set; }
        public ushort MaxDuration { get; set; }
        public VehicleGroup()
        {
            
        }
        public VehicleGroup(string groupName, byte priority, List<ushort> vehicles, List<ConfigItem> drops, ushort minDuration, ushort maxDuration)
        {
            GroupName = groupName;
            Priority = priority;
            Vehicles = vehicles;
            Drops = drops;
            MinDuration = minDuration;
            MaxDuration = maxDuration;
        }

        public ushort GetDuration() =>
            (ushort)new Random().Next(MinDuration, MaxDuration);
        public bool IncludeVehicle(ushort VehicleID) =>
            Vehicles.Contains(VehicleID) || Vehicles.Contains(0);
    }
}