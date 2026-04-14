using Dismantle_Plugin__UnturnedStore_Version_.Models.Sub_Models;
using Newtonsoft.Json;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dismantle_Plugin__UnturnedStore_Version_.Models
{
    public class ContainerInventory
    {
        public SerializableItem[,] _items;
        public byte SizeX { get; set; } //Column
        public byte SizeY { get; set; } //Row
        public ContainerInventory()
        {

        }
        public ContainerInventory(byte sizeX, byte sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            _items = new SerializableItem[SizeY, sizeX];
        }

        public int GetItemCount()
        {
            int ItemCount = 0;
            for (byte indexY = 0; indexY < SizeY; indexY++)
                for (byte indexX = 0; indexX < SizeX; indexX++)
                    if (_items[indexY, indexX] != null)
                        ItemCount++;
            return ItemCount;
        }
        public bool TryAddItem(Item Item)
        {
            if (!FindSpace(out byte IndexX, out byte IndexY))
            {
                if (SizeY == 255)
                    return false;
                else
                    UpdateRows((byte)(SizeY + 1));
                FindSpace(out IndexX, out IndexY);
            }

            return AddItem(Item, IndexX, IndexY);
        }
        public bool AddItem(Item Item, byte IndexX, byte IndexY)
        {
            if (IndexX >= SizeX || IndexY >= SizeY)
                return false;

            if (_items[IndexY, IndexX] == null)
            {
                _items[IndexY, IndexX] = new SerializableItem(Item);
                return true;
            }
            return false;
        }
        public bool RemoveItem(byte IndexX, byte IndexY)
        {
            if (IndexX >= SizeX || IndexY >= SizeY)
                return false;

            _items[IndexY, IndexX] = null;
            if (_items[IndexY, 0] == null && _items[IndexY, 1] == null && _items[IndexY, 2] == null && _items[IndexY, 3] == null && _items[IndexY, 4] == null && SizeY > 1)
            {
                for (int indexY = IndexY; indexY < SizeY - 1; indexY++)
                    for (byte indexX = 0; indexX < SizeX; indexX++)
                        _items[indexY, indexX] = _items[indexY + 1, indexX];
                UpdateRows((byte)(SizeY - 1));
            }
            return true;
        }
        public bool GetItem(byte IndexX, byte IndexY, out SerializableItem FindedItem)
        {
            FindedItem = null;
            if (IndexX >= SizeX || IndexY >= SizeY)
                return false;

            FindedItem = _items[IndexY, IndexX];
            return FindedItem != null;
        }
        public bool FindSpace(out byte IndexX, out byte IndexY)
        {
            for (int indexY = SizeY - 1; indexY >= 0; indexY--)
            {
                for (byte indexX = 0; indexX < SizeX; indexX++)
                {
                    if (_items[indexY, indexX] == null)
                    {
                        IndexX = indexX;
                        IndexY = (byte)indexY;
                        return true;
                    }
                }
            }
            IndexX = 0;
            IndexY = 0;
            return false;
        }
        public void UpdateRows(byte NewSizeY)
        {
            SerializableItem[,] OldItems = new SerializableItem[SizeY, SizeX];
            for (byte indexY = 0; indexY < SizeY; indexY++)
                for (byte indexX = 0; indexX < SizeX; indexX++)
                    if (_items[indexY, indexX] != null)
                        OldItems[indexY, indexX] = _items[indexY, indexX];
            _items = new SerializableItem[NewSizeY, SizeX];
            for (byte indexY = 0; indexY < ((NewSizeY > SizeY) ? SizeY : NewSizeY); indexY++)
                for (byte indexX = 0; indexX < SizeX; indexX++)
                    _items[indexY, indexX] = OldItems[indexY, indexX];
            SizeY = NewSizeY;
        }
    }
}
