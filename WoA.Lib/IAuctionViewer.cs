using System.Collections.Generic;
using WoA.Lib.TSM;

namespace WoA.Lib
{
    public interface IAuctionViewer
    {
        int GetItemId(string line);
        void SeeAuctionsFor(List<Auction> auctions, int itemId);
        void SeeAuctionsOwnedBy(List<Auction> auctions, string owner);
        void SeeTopSellers(List<Auction> auctions);
        void SimulateFlippingItem(List<Auction> auctions, int itemId);
    }
}