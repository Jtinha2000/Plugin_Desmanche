using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dismantle_Plugin__UnturnedStore_Version_.Models.Configuration
{
    public class ItemImage
    {
        public string ImageURL { get; set; }
        public ushort ItemID { get; set; }
        public ItemImage()
        {
            
        }
        public ItemImage(string imageURL, ushort itemID)
        {
            ImageURL = imageURL;
            ItemID = itemID;
        }
    }
}
