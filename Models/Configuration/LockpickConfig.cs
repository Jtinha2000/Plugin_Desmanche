using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Dismantle_Plugin__UnturnedStore_Version_.Models.Configuration
{
    public class LockpickConfig
    {
        public ushort ItemID { get; set; }
        public string NeededPermission { get; set; }
        public uint MinTime { get; set; }
        public uint MaxTime { get; set; }
        public byte SucessProbability { get; set; }
        public byte AlarmProbability { get; set; }
        public bool AlarmAlertPolice { get; set; }
        public bool AlarmOnFail { get; set; }
        public bool ConsumeJustOnFail { get; set; }
        [XmlArrayItem("VehicleID")]
        public List<ushort> WhitelistVehicles { get; set; }
        public LockpickConfig()
        {

        }
        public LockpickConfig(ushort itemID, string neededPermission, uint minTime, uint maxTime, byte sucessProbability, byte alarmProbability, bool alarmAlertPolice, bool consumeJustOnFail, List<ushort> whitelistVehicles, bool alarmonfail)
        {
            ItemID = itemID;
            NeededPermission = neededPermission;
            MinTime = minTime;
            MaxTime = maxTime;
            SucessProbability = sucessProbability;
            AlarmProbability = alarmProbability;
            AlarmAlertPolice = alarmAlertPolice;
            ConsumeJustOnFail = consumeJustOnFail;
            WhitelistVehicles = whitelistVehicles;
            AlarmOnFail = alarmonfail;
        }

        public bool IncludeVehicle(ushort VehicleID) =>
            WhitelistVehicles.Contains(VehicleID) || WhitelistVehicles.Contains(0);
        public int GetDuration() =>
            new Random().Next((int)MinTime, (int)MaxTime);
    }
}
