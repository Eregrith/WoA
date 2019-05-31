﻿using System;
using System.Collections.Generic;
using System.Linq;
using WoA.Lib.Blizzard;
using WoA.Lib.TSM;

namespace WoA.Lib
{
    public class AuctionViewer : IAuctionViewer
    {
        private readonly IStylizedConsole _console;
        private readonly ITsmClient _tsm;
        private readonly IBlizzardClient _blizzard;

        public AuctionViewer(IStylizedConsole console, ITsmClient tsm, IBlizzardClient blizzard)
        {
            _console = console;
            _tsm = tsm;
            _blizzard = blizzard;
        }

        public int GetItemId(string line)
        {
            if (int.TryParse(line, out int id))
            {
                return id;
            }
            return _tsm.GetItemIdFromName(line);
        }

        public void SeeAuctionsOwnedBy(string owner)
        {
            _console.WriteLine($"Looking at auctions from owner {owner}");
            var itemAuctions = _blizzard.Auctions.Where(a => a.owner == owner);
            _console.WriteLine($"There are {itemAuctions.Count()} auctions for {owner}");

            ShowAuctionsForMultiItems(itemAuctions);
        }

        public void SimulateFlippingItem(int itemId)
        {
            SimulateResettingItem(itemId, 80, 100);
        }

        public void SimulateResettingItem(int itemId, int buyingPercentageValue, int sellingPercentageValue)
        {
            float buyingPercentage = (float)buyingPercentageValue / 100;
            float sellingPercentage = (float)sellingPercentageValue / 100;
            TsmItem tsmItem = _tsm.GetItem(itemId);
            _console.WriteLine($"Looking at flips for item:");
            _console.WriteAscii(tsmItem.Name);
            var itemAuctions = _blizzard.Auctions.Where(a => a.item == itemId);
            _console.WriteLine($"There are {itemAuctions.Count()} {tsmItem?.Name} auctions");
            long maxBuy = (long)(tsmItem?.MarketValue * buyingPercentage);
            _console.WriteLine($"Tsm Item : {tsmItem}");
            var cheapAuctions = itemAuctions.Where(a => a.PricePerItem <= maxBuy && a.buyout != 0);
            _console.WriteLine($"{cheapAuctions.Count()} of which are under {maxBuy.ToGoldString()} per ({buyingPercentageValue}% MarketValue)");
            if (cheapAuctions.Any())
            {
                long sumBuyoutCheapAuctions = cheapAuctions.Sum(a => a.buyout);

                _console.WriteLine($"For a total of {sumBuyoutCheapAuctions.ToGoldString()} for {cheapAuctions.Sum(a => a.quantity)} items at an average of {((long)cheapAuctions.Average(a => a.PricePerItem)).ToGoldString()} per");

                ShowAuctions(tsmItem, cheapAuctions);

                long sumSelloutFlippingWithGoblinTaxe = (long)Math.Round(cheapAuctions.Sum(a => a.quantity) * tsmItem.MarketValue * sellingPercentage * 0.95);
                long goblinTax = (long)Math.Round(cheapAuctions.Sum(a => a.quantity) * tsmItem.MarketValue * sellingPercentage * 0.05);
                long profit = sumSelloutFlippingWithGoblinTaxe - sumBuyoutCheapAuctions;

                double percentProfit = Math.Round(((double)sumSelloutFlippingWithGoblinTaxe / sumBuyoutCheapAuctions - 1) * 100, 2);
                _console.WriteLine($"Buying them and reselling them at {sellingPercentageValue}% would net you {profit.ToGoldString()} (AH cut accounted for. You gain {percentProfit}% of your invested money)");
                _console.WriteLine($"(In the meantime those pesky AH Goblins have taken out {goblinTax.ToGoldString()} with the 5% AH cut)");
            }
        }

        public void SeeAuctionsFor(int itemId)
        {
            TsmItem tsmItem = _tsm.GetItem(itemId);
            if (tsmItem == null)
            {
                _console.WriteLine("No item found in TSM data with this id");
                return;
            }
            _console.WriteLine($"Looking at auctions for item:");
            _console.WriteAscii(tsmItem.Name);
            var itemAuctions = _blizzard.Auctions.Where(a => a.item == itemId);
            _console.WriteLine($"There are {itemAuctions.Count()} {tsmItem.Name} auctions");
            _console.WriteLine($"Tsm Item : {tsmItem}");

            ShowAuctions(tsmItem, itemAuctions);

            _console.WriteLine($"{tsmItem.Name} is sold at a rate of {tsmItem.RegionSaleRate} in region, for around {tsmItem.RegionAvgDailySold} items per day");
        }

        public void SeeTopSellers()
        {
            var topSellers = _blizzard.Auctions.GroupBy(a => a.owner).OrderByDescending(a => a.Sum(i => i.buyout)).Take(10);
            _console.WriteLine(String.Format("{0,20}{1,20}{2,10}{3,45}", "Seller", "Quantity", "(auctions)", "Buyout total"));
            foreach (var seller in topSellers)
            {
                _console.WriteLine(String.Format("{0,20}{1,20}{2,10}{3,45}", seller.Key, seller.Sum(i => i.quantity), "(" + seller.Count() + ")", seller.Sum(i => i.buyout).ToGoldString()));
            }
        }

        private void ShowAuctionsForMultiItems(IEnumerable<Auction> auctions)
        {
            _console.WriteLine(String.Format("{4,45}{0,20}{1,12}{2,20}{3,20}", "Price per item", "Quantity", "Buyout total", "Seller", "Item name"));
            foreach (var auctionGroup in auctions.GroupBy(a => new { a.PricePerItem, a.quantity, a.buyout, a.owner, a.item }).OrderBy(a => a.Key.PricePerItem).ThenBy(g => g.Key.item))
            {
                TsmItem tsmItem = _tsm.GetItem(auctionGroup.First().item);
                _console.WriteLine(String.Format("{5,45}{0,20}{1,7}x {2,3}{3,20}{4,20}"
                    , auctionGroup.Key.PricePerItem.ToGoldString()
                    , auctionGroup.Count()
                    , auctionGroup.Key.quantity
                    , (auctionGroup.Key.buyout * auctionGroup.Count()).ToGoldString()
                    , auctionGroup.Key.owner
                    , tsmItem?.Name));
            }

            _console.WriteLine(String.Format("{0,20}{1,12}{2,20}"
                , "Grand total"
                , auctions.Sum(a => a.quantity)
                , auctions.Sum(a => a.buyout).ToGoldString()));

            _console.WriteLine(String.Format("{0,45}", "Grand total per item"));

            foreach (var auctionGroup in auctions.GroupBy(a => a.item).OrderByDescending(g => g.Sum(a => a.buyout)))
            {
                TsmItem tsmItem = _tsm.GetItem(auctionGroup.First().item);
                _console.WriteLine(String.Format("{0,45}{1,12}{2,20}"
                    , tsmItem?.Name
                    , auctionGroup.Sum(a => a.quantity)
                    , auctionGroup.Sum(a => a.buyout).ToGoldString()));
            }
        }

        private void ShowAuctions(TsmItem tsmItem, IEnumerable<Auction> auctions)
        {
            _console.WriteLine(String.Format("{0,20}{1,12}{2,20}{3,20}{4,14}", "Price per item", "Quantity", "Buyout total", "Seller", "% MarketValue"));
            foreach (var auctionGroup in auctions.GroupBy(a => new { a.PricePerItem, a.quantity, a.buyout, a.owner }).OrderBy(g => g.Key.PricePerItem))
            {
                _console.WriteLine(String.Format("{0,20}{1,7}x {2,3}{3,20}{4,20}{5,12} %"
                    , auctionGroup.Key.PricePerItem.ToGoldString()
                    , auctionGroup.Count()
                    , auctionGroup.Key.quantity
                    , (auctionGroup.Key.buyout * auctionGroup.Count()).ToGoldString()
                    , auctionGroup.Key.owner
                    , Math.Round((auctionGroup.Key.PricePerItem * 100.0) / tsmItem.MarketValue)));
            }

            _console.WriteLine(String.Format("{0,20}{1,12}{2,20}"
                , "Grand total"
                , auctions.Sum(a => a.quantity)
                , auctions.Sum(a => a.buyout).ToGoldString()));

            _console.WriteLine(String.Format("{0,20}{1,12}{1,20}{2,20}", "Grand total per seller", "", "Average %MarketValue"));

            foreach (var auctionGroup in auctions.GroupBy(a => a.owner).OrderByDescending(g => g.Sum(a => a.quantity)))
            {
                double avgMarketValue = Math.Round(auctionGroup.Average(a => (a.PricePerItem * 100.0) / tsmItem.MarketValue), 2);
                _console.WriteLine(String.Format("{0,20}{1,12}{2,20}{3,20:0.00} %"
                    , auctionGroup.Key
                    , auctionGroup.Sum(a => a.quantity)
                    , auctionGroup.Sum(a => a.buyout).ToGoldString()
                    , avgMarketValue));
            }
        }
    }
}
