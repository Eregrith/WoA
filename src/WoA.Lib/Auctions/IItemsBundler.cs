using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.Auctions
{
    public interface IItemsBundler
    {
        void Add(int itemId, int quantity);
        bool Remove(int itemId, int quantity, bool removeItem);
        Dictionary<int, int> GetItems();
        void Clear();
    }
}
