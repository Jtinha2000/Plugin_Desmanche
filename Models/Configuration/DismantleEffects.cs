using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dismantle_Plugin__UnturnedStore_Version_.Models.Configuration
{
    public class DismantleEffects
    {
        public bool KillPassengers { get; set; }
        public bool FreezeVehicle { get; set; }
        public byte BatteryDecreaseChance { get; set; }
        public byte HealthDecreaseChance { get; set; }
        public byte BreakTireChance { get; set; }
        public float TickLenght { get; set; }
        public ushort ScreamAudioID { get; set; }
        public DismantleEffects()
        {
            
        }
        public DismantleEffects(bool killPassengers, bool freezeVehicle, byte batteryDecreaseChance, byte healthDecreaseChance, byte breakTireChance, float tickLenght, ushort screamAudioID)
        {
            KillPassengers = killPassengers;
            FreezeVehicle = freezeVehicle;
            BatteryDecreaseChance = batteryDecreaseChance;
            HealthDecreaseChance = healthDecreaseChance;
            BreakTireChance = breakTireChance;
            TickLenght = tickLenght;
            ScreamAudioID = screamAudioID;
        }
    }
}
