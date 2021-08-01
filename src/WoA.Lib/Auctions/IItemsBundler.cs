using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.Auctions
{
    public interface IItemsBundler
    {
        void Add(string itemId, int quantity);
        bool Remove(string itemId, int quantity, bool removeItem);
        Dictionary<string, int> GetItems();
        void Clear();
    }
}
