using System.Collections.Generic;
using WoA.Lib.TSM;

namespace WoA.Lib
{
    public interface IAuctionViewer
    {
        int GetItemId(string line);
        void SeeAuctionsFor(int itemId);
        void SimulateFlippingItem(int itemId);
        ItemFlipResult SimulateFlippingItemShortVersion(int itemId);
        ItemBuyResult SimulateBuyingItemShortVersion(int itemId, int nbItem, int maxPercentBuyout);
        void SimulateResettingItem(int itemId, int buyingPercentageValue, int sellingPercentageValue);
        void ShowAuctionsForMultiItems(IEnumerable<Auction> auctions);
        void ShowAuctionsForMultiItems(IEnumerable<Auction> auctions, bool showHeaders, bool showTotals);
    }
}