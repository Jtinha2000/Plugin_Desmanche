using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDG.Provider.UnturnedEconInfo;
using static UnityEngine.Random;

namespace Dismantle_Plugin__UnturnedStore_Version_.Models.Configuration
{
    public class ConfigItem
    {
        public ushort ItemID { get; set; }
        public byte Probability { get; set; }
        public int MinAmount { get; set; }
        public int MaxAmount { get; set; }
        public ConfigItem()
        {
            
        }
        public ConfigItem(ushort itemID, byte probability, int minAmount, int maxAmount)
        {
            ItemID = itemID;
            Probability = probability;
            MinAmount = minAmount;
            MaxAmount = maxAmount;
        }

        public Item ConvertToItem() => new Item(ItemID, true);
    }
}
