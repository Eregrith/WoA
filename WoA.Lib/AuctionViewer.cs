﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WoA.Lib.TSM;
using WorldOfAuctions;

namespace WoA.Lib
{
    public class AuctionViewer
    {
        private IStylizedConsole _console;
        private string _realm;

        public AuctionViewer(IStylizedConsole console, string realm)
        {
            _console = console;
            _realm = realm;
        }

        public void ChangeRealm(string realm) => _realm = realm;

        public int GetItemId(TsmClient tsm, string line)
        {
            string item = String.Join(" ", line.Split(' ').Skip(1));

            if (int.TryParse(item, out int id))
            {
                return id;
            }
            return tsm.GetItemIdFromName(item);
        }

        public void SeeAuctionsOwnedBy(TsmClient tsm, List<Auction> auctions, string owner)
        {
            _console.WriteLine($"Looking at auctions from owner {owner}");
            var itemAuctions = auctions.Where(a => a.owner == owner);
            _console.WriteLine($"There are {itemAuctions.Count()} auctions for {owner}");

            ShowAuctionsForMultiItems(tsm, itemAuctions);
        }

        public void SimulateFlippingItem(TsmClient tsm, List<Auction> auctions, int itemId)
        {
            TsmItem tsmItem = tsm.GetItem(itemId, _realm);
            _console.WriteLine($"Looking at flips for item:");
            _console.WriteAscii(tsmItem.Name);
            var itemAuctions = auctions.Where(a => a.item == itemId);
            _console.WriteLine($"There are {itemAuctions.Count()} {tsmItem?.Name} auctions");
            long maxBuy = (long)(tsmItem?.MarketValue * 0.8);
            _console.WriteLine($"Tsm Item : {tsmItem}");
            var cheapAuctions = itemAuctions.Where(a => a.PricePerItem <= maxBuy && a.buyout != 0);
            _console.WriteLine($"{cheapAuctions.Count()} of which are under {maxBuy.ToGoldString()} per (80% MarketValue)");
            if (cheapAuctions.Any())
            {
                _console.WriteLine($"For a total of {cheapAuctions.Sum(a => a.buyout).ToGoldString()} for {cheapAuctions.Sum(a => a.quantity)} items at an average of {((long)cheapAuctions.Average(a => a.PricePerItem)).ToGoldString()} per");

                ShowAuctions(tsmItem, cheapAuctions);

                long profit = (cheapAuctions.Sum(a => a.quantity) * tsmItem.MarketValue) - cheapAuctions.Sum(a => a.buyout);
                _console.WriteLine($"Buying them and reselling them at MarketValue would net you {profit.ToGoldString()} profit");
            }
        }

        public void SeeAuctionsFor(TsmClient tsm, List<Auction> auctions, int itemId)
        {
            TsmItem tsmItem = tsm.GetItem(itemId, _realm);
            _console.WriteLine($"Looking at auctions for item:");
            _console.WriteAscii(tsmItem.Name);
            var itemAuctions = auctions.Where(a => a.item == itemId);
            _console.WriteLine($"There are {itemAuctions.Count()} {tsmItem?.Name} auctions");
            _console.WriteLine($"Tsm Item : {tsmItem}");

            ShowAuctions(tsmItem, itemAuctions);

            _console.WriteLine($"{tsmItem.Name} is sold at a rate of {tsmItem.RegionSaleRate} in region, for around {tsmItem.RegionSaleAvg} items per day");
        }

        public void ShowAuctionsForMultiItems(TsmClient tsm, IEnumerable<Auction> auctions)
        {
            _console.WriteLine(String.Format("{4,45}{0,20}{1,12}{2,20}{3,20}", "Price per item", "Quantity", "Buyout total", "Seller", "Item name"));
            foreach (var auctionGroup in auctions.GroupBy(a => new { a.PricePerItem, a.quantity, a.buyout, a.owner, a.item }).OrderBy(a => a.Key.item).ThenBy(g => g.Key.PricePerItem))
            {
                TsmItem tsmItem = tsm.GetItem(auctionGroup.First().item, _realm);
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
                TsmItem tsmItem = tsm.GetItem(auctionGroup.First().item, _realm);
                _console.WriteLine(String.Format("{0,45}{1,12}{2,20}"
                    , tsmItem?.Name
                    , auctionGroup.Sum(a => a.quantity)
                    , auctionGroup.Sum(a => a.buyout).ToGoldString()));
            }
        }

        public void ShowAuctions(TsmItem tsmItem, IEnumerable<Auction> auctions)
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

            _console.WriteLine(String.Format("{0,20}", "Grand total per seller"));

            foreach (var auctionGroup in auctions.GroupBy(a => a.owner).OrderByDescending(g => g.Sum(a => a.buyout)))
            {
                _console.WriteLine(String.Format("{0,20}{1,12}{2,20}"
                    , auctionGroup.Key
                    , auctionGroup.Sum(a => a.quantity)
                    , auctionGroup.Sum(a => a.buyout).ToGoldString()));
            }
        }
    }
}
