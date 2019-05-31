using System.Collections.Generic;
using WoA.Lib.TSM;

namespace WoA.Lib
{
    public interface IAuctionViewer
    {
        void ChangeRealm(string realm);
        int GetItemId(ITsmClient tsm, string line);
        void SeeAuctionsFor(ITsmClient tsm, List<Auction> auctions, int itemId);
        void SeeAuctionsOwnedBy(ITsmClient tsm, List<Auction> auctions, string owner);
        void SeeTopSellers(ITsmClient tsm, List<Auction> auctions);
        void ShowAuctions(TsmItem tsmItem, IEnumerable<Auction> auctions);
        void SimulateFlippingItem(ITsmClient tsm, List<Auction> auctions, int itemId);
    }
}