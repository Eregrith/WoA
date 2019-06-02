using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.Auctions
{
    public interface IItemsBundler
    {
        void Add(int itemId, int quantity);
        Dictionary<int, int> GetItems();
        void Clear();
    }
}
