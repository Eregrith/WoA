using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.Auctions
{
    public class ItemBundler : IItemsBundler
    {
        private Dictionary<string, int> Items { get; set; } = new Dictionary<string, int>();

        public void Add(string itemId, int quantity)
        {
            if (Items.ContainsKey(itemId))
                Items[itemId] += quantity;
            else
                Items.Add(itemId, quantity);
        }

        public bool Remove(string itemId, int quantity, bool removeItem)
        {
            if (Items.ContainsKey(itemId))
            {
                if (removeItem || quantity >= Items[itemId])
                {
                    Items.Remove(itemId);
                }
                else
                {
                    Items[itemId] -= quantity;
                }
                return true;
            }
            return false;
        }

        public void Clear()
        {
            Items.Clear();
        }

        public Dictionary<string, int> GetItems()
        {
            return Items;
        }
    }
}
