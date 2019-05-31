using System.Collections.Generic;
using WoA.Lib.TSM;

namespace WoA.Lib
{
    public interface IAuctionViewer
    {
        int GetItemId(string line);
        void SeeAuctionsFor(int itemId);
        void SeeAuctionsOwnedBy(string owner);
        void SeeTopSellers();
        void SimulateFlippingItem(int itemId);
    }
}