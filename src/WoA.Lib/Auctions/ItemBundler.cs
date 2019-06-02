using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.Auctions
{
    public class ItemBundler : IItemsBundler
    {
        private Dictionary<int, int> Items { get; set; } = new Dictionary<int, int>();

        public void Add(int itemId, int quantity)
        {
            if (Items.ContainsKey(itemId))
                Items[itemId] += quantity;
            else
                Items.Add(itemId, quantity);
        }

        public void Clear()
        {
            Items.Clear();
        }

        public Dictionary<int, int> GetItems()
        {
            return Items;
        }
    }
}
