using Dismantle_Plugin__UnturnedStore_Version_.Models.Configuration;
using Rocket.API;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Dismantle_Plugin__UnturnedStore_Version_
{
    public class Configuration : IRocketPluginConfiguration
    {
        public int MinOfficerLockpickNeeded { get; set; }
        public bool NeedsBeOnGroup { get; set; }
        [XmlArrayItem("VehicleID")]
        public List<ushort> BlacklistedLockpickVehicles { get; set; }
        public string PolicePermission { get; set; }
        public ushort AlarmEffectID { get; set; }
        public float AlarmEffectRange { get; set; }
        public DismantleEffects EffectsConfig { get; set; }
        public List<LockpickConfig> Lockpicks { get; set; }
        public List<VehicleGroup> VehicleGroups { get; set; }
        public List<ItemImage> Images { get; set; }
        public void LoadDefaults()
        {
            MinOfficerLockpickNeeded = 2;
            NeedsBeOnGroup = false;
            BlacklistedLockpickVehicles = new List<ushort> {
            33, 106, 108
            };
            PolicePermission = "DismantlePlugin.Police";
            AlarmEffectID = 0;
            AlarmEffectRange = 55;
            Lockpicks = new List<LockpickConfig> 
            {
                new LockpickConfig(1032, "Lockpick.Normal", 15, 35, 100, 40, true, false, new List <ushort>{ 0 }, true)
            };
            EffectsConfig = new DismantleEffects(true, true, 12, 12, 12, 1.25f, 0);
            VehicleGroups = new List<VehicleGroup>
            {
                // ID 0 = All Cars.
                new VehicleGroup("All Cars", 0, new List<ushort> { 0 }, new List<ConfigItem>
                {
                    new ConfigItem(35920, 100, 1, 1),
                    new ConfigItem(35919, 100, 1, 4),
                    new ConfigItem(35922, 100, 1, 1),
                    new ConfigItem(1451, 100, 1, 4),
                    new ConfigItem(1450, 100, 1, 1), //Metal Scrap
                }, 60, 180),
            };
            Images = new List<ItemImage>
            {
                new ItemImage("https://i.imgur.com/NsSGCHv.png", 67),
                new ItemImage("https://i.imgur.com/9ityxhH.png", 1451),
                new ItemImage("https://i.imgur.com/7vkn1vx.png", 1450),
                new ItemImage("https://i.imgur.com/n1Jt1vj.png", 35919),
                new ItemImage("https://i.imgur.com/n3bPUUZ.png", 35921),
                new ItemImage("https://i.imgur.com/EDyg2Vb.png", 35922),
                new ItemImage("https://i.imgur.com/HwLZY9m.png", 35920),
                new ItemImage("https://i.imgur.com/HwLZY9m.png", 35923),
                new ItemImage("https://i.imgur.com/HwLZY9m.png", 35924),
                new ItemImage("https://i.imgur.com/HwLZY9m.png", 35925),
            };
        }
    }
}
