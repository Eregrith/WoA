using System.Collections.Generic;
using WoA.Lib.TSM;

namespace WoA.Lib
{
    public interface IAuctionViewer
    {
        void SeeAuctionsFor(string itemId);
        void SimulateFlippingItem(string itemId);
        ItemFlipResult SimulateFlippingItemShortVersion(string itemId);
        ItemBuyResult SimulateBuyingItemShortVersion(string itemId, int nbItem, int maxPercentBuyout);
        void SimulateResettingItem(string itemId, int buyingPercentageValue, int sellingPercentageValue);
        void ShowAuctionsForMultiItems(IEnumerable<Auction> auctions);
        void ShowAuctionsForMultiItems(IEnumerable<Auction> auctions, bool showHeaders, bool showTotals);
    }
}