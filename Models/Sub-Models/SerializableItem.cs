using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dismantle_Plugin__UnturnedStore_Version_.Models.Sub_Models
{
    public class SerializableItem
    {
        public ushort ItemID { get; set; }
        public byte Quality { get; set; }
        public byte[] State { get; set; }
        public byte Amount { get; set; }
        public SerializableItem()
        {
            
        }
        public SerializableItem(ushort itemID, byte quality, byte[] state, byte amount)
        {
            ItemID = itemID;
            Quality = quality;
            State = state;
            Amount = amount;
        }
        public SerializableItem(Item Item)
        {
            ItemID = Item.id;
            Quality = Item.quality;
            State = Item.state;
            Amount = Item.amount;
        }
        public Item ConvertToItem() => new Item(ItemID, Amount, Quality, State);
    }
}
