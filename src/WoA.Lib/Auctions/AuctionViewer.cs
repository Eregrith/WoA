using System;
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

        public void SimulateFlippingItem(string itemId)
        {
            SimulateResettingItem(itemId, 80, 100);
        }

        public ItemFlipResult SimulateFlippingItemShortVersion(string itemId)
        {
            TsmItem tsmItem = _tsm.GetItem(itemId);
            IEnumerable<Auction> itemAuctions = _blizzard.Auctions.Where(a => a.item.id == itemId);
            long maxBuy = (long)(tsmItem?.MarketValue * 0.8);
            IEnumerable<Auction> cheapAuctions = itemAuctions.Where(a => a.PricePerItem <= maxBuy && a.buyout != 0);
            long sumBuyoutCheapAuctions = 0;
            long profit = 0;
            double percentProfit = 0;
            int totalQuantity = 0;
            if (cheapAuctions.Any())
            {
                sumBuyoutCheapAuctions = cheapAuctions.Sum(a => a.FullPrice);
                totalQuantity += cheapAuctions.Sum(x => x.quantity);

                long sumSelloutFlippingWithGoblinTaxe = (long)Math.Round(cheapAuctions.Sum(a => a.quantity) * tsmItem.MarketValue * 0.95);
                long goblinTax = (long)Math.Round(cheapAuctions.Sum(a => a.quantity) * tsmItem.MarketValue * 0.05);
                profit = sumSelloutFlippingWithGoblinTaxe - sumBuyoutCheapAuctions;

                percentProfit = Math.Round(((double)sumSelloutFlippingWithGoblinTaxe / sumBuyoutCheapAuctions - 1) * 100, 2);
            }
            _console.WriteLine(String.Format("{0,-35} {1,20} {2,15} {3,20} {4,20} {5,20}", tsmItem.Name, tsmItem.MarketValue.ToGoldString(), totalQuantity, sumBuyoutCheapAuctions.ToGoldString(), profit.ToGoldString(), percentProfit.ToString() + "%"));
            return new ItemFlipResult() { Quantity = totalQuantity, TotalBuyout = sumBuyoutCheapAuctions, NetProfit = profit, PercentProfit = percentProfit };
        }

        public ItemBuyResult SimulateBuyingItemShortVersion(string itemId, int nbItem, int maxPercentBuyout)
        {
            TsmItem tsmItem = _tsm.GetItem(itemId);
            float maxPercentage = (float)maxPercentBuyout / 100;
            IEnumerable<Auction> itemAuctions = _blizzard.Auctions.Where(a => a.item.id == itemId);
            long maxBuy = (long)(tsmItem?.MarketValue * maxPercentage);
            IEnumerable<Auction> cheapAuctions = itemAuctions.Where(a => a.PricePerItem <= maxBuy && a.buyout != 0);
            long sumBuyoutCheapAuctions = 0;
            int totalQuantity = 0;
            int tempNbItem = nbItem;
            if (cheapAuctions.Any())
            {
                var stackAuctions = new Queue<Auction>(cheapAuctions.OrderBy(x => x.PricePerItem));
                while (tempNbItem > 0 && stackAuctions.Any())
                {
                    Auction auction = stackAuctions.Dequeue();
                    sumBuyoutCheapAuctions += auction.FullPrice;
                    totalQuantity += auction.quantity;
                    tempNbItem -= auction.quantity;
                }
            }
            _console.WriteLine(String.Format("{0,-35} {1,20} {2,15} {3,20}", nbItem + " x " + tsmItem.Name, tsmItem.MarketValue.ToGoldString(), totalQuantity, sumBuyoutCheapAuctions.ToGoldString()));
            return new ItemBuyResult() { Quantity = totalQuantity, TotalBuyout = sumBuyoutCheapAuctions };
        }

        public void SimulateResettingItem(string itemId, int buyingPercentageValue, int sellingPercentageValue)
        {
            float buyingPercentage = (float)buyingPercentageValue / 100;
            float sellingPercentage = (float)sellingPercentageValue / 100;
            TsmItem tsmItem = _tsm.GetItem(itemId);
            if (tsmItem == null)
            {
                _console.WriteLine("[Error] Can't simulate reset for item : No TSM Item found with id " + itemId);
                return;
            }
            _console.WriteLine($"Looking at flips for item:");
            _console.WriteAscii(tsmItem.Name);
            var itemAuctions = _blizzard.Auctions.Where(a => a.item.id == itemId);
            _console.WriteLine($"There are {itemAuctions.Count()} {tsmItem?.Name} auctions");
            long maxBuy = (long)(tsmItem?.MarketValue * buyingPercentage);
            _console.WriteLine($"Tsm Item : {tsmItem}");
            var cheapAuctions = itemAuctions.Where(a => a.PricePerItem <= maxBuy && a.buyout != 0);
            _console.WriteLine($"{cheapAuctions.Count()} of which are under {maxBuy.ToGoldString()} per ({buyingPercentageValue}% MarketValue)");
            if (cheapAuctions.Any())
            {
                long sumBuyoutCheapAuctions = cheapAuctions.Sum(a => a.FullPrice);

                var avg = cheapAuctions.Average(a => a.PricePerItem);
                var avgPercent = Math.Round((avg * 100.0) / tsmItem.MarketValue, 2);
                _console.WriteLine($"For a total of {sumBuyoutCheapAuctions.ToGoldString()} for {cheapAuctions.Sum(a => a.quantity)} items at an average of {((long)avg).ToGoldString()} ({avgPercent}% MarketValue) per");

                ShowAuctions(tsmItem, cheapAuctions);

                long sumSelloutFlippingWithGoblinTaxe = (long)Math.Round(cheapAuctions.Sum(a => a.quantity) * tsmItem.MarketValue * sellingPercentage * 0.95);
                long goblinTax = (long)Math.Round(cheapAuctions.Sum(a => a.quantity) * tsmItem.MarketValue * sellingPercentage * 0.05);
                long profit = sumSelloutFlippingWithGoblinTaxe - sumBuyoutCheapAuctions;

                double percentProfit = Math.Round(((double)sumSelloutFlippingWithGoblinTaxe / sumBuyoutCheapAuctions - 1) * 100, 2);
                _console.WriteLine($"Buying them and reselling them at {((long)Math.Round(tsmItem.MarketValue * sellingPercentage)).ToGoldString()} ({sellingPercentageValue}% MarketValue) each would net you {profit.ToGoldString()} (AH cut accounted for. You gain {percentProfit}% of your invested money)");
                _console.WriteLine($"(In the meantime those pesky AH Goblins have taken out {goblinTax.ToGoldString()} with the 5% AH cut)");
            }
        }

        public void SeeAuctionsFor(string itemId)
        {
            TsmItem tsmItem = _tsm.GetItem(itemId);
            string name;
            if (tsmItem == null)
            {
                _console.WriteLine("No item found in TSM data with this id");
                name = _blizzard.GetItem(itemId).name.en_US;
            }
            else
            {
                name = tsmItem.Name;
            }
            _console.WriteLine($"Looking at auctions for item:");
            if (name.Length > 20)
            {
                List<string> parts = name.Split(' ').ToList();
                string currentName = parts.First();
                foreach (string part in parts.Skip(1))
                {
                    if ((currentName + ' ' + part).Length > 20)
                    {
                        _console.WriteAscii(currentName);
                        currentName = part;
                    }
                    else
                        currentName += ' ' + part;
                }
                if (!String.IsNullOrEmpty(currentName))
                    _console.WriteAscii(currentName);
            }
            else
                _console.WriteAscii(name);
            var itemAuctions = _blizzard.Auctions.Where(a => a.item.id == itemId);
            WowQualityType quality = _blizzard.GetQuality(itemId);
            _console.WriteLine($"There are {itemAuctions.Count()} {name.WithQuality(quality)} auctions");
            if (tsmItem != null)
                _console.WriteLine($"Tsm Item : {tsmItem}");

            ShowAuctions(tsmItem, itemAuctions);

            if (tsmItem != null)
                _console.WriteLine($"{tsmItem.Name} is sold at a rate of {tsmItem.RegionSaleRate} in region, for around {tsmItem.RegionAvgDailySold} items per day");
        }

        public void ShowAuctionsForMultiItems(IEnumerable<Auction> auctions)
        {
            ShowAuctionsForMultiItems(auctions, true, true);
        }

        public void ShowAuctionsForMultiItems(IEnumerable<Auction> auctions, bool showHeader, bool showTotals)
        {
            if (showHeader)
                _console.WriteLine(String.Format("{0,40}{1,20}{2,12}{3,30}{4,15}{5,14}", "Item name", "Price per item", "Quantity", "Time Left (first seen on)", "Buyout total", "% MarketValue"));
            foreach (var auctionGroup in auctions.GroupBy(a => new { a.PricePerItem, a.quantity, a.buyout, a.item, a.time_left, a.FirstSeenOn }).OrderBy(a => a.Key.PricePerItem).ThenBy(g => g.Key.item))
            {
                WowItem item = _blizzard.GetItem(auctionGroup.First().item.id);
                TsmItem tsmItem = _tsm.GetItem(auctionGroup.First().item.id);
                string percentDbMarket = tsmItem != null ? Math.Round((auctionGroup.Key.PricePerItem * 100.0) / tsmItem.MarketValue).ToString() : "unknown";
                WowQualityType quality = item.quality.AsQualityTypeEnum;
                string name = item.name.en_US;
                _console.WriteLine(String.Format("{0,46}{1,20}{2,7}x {3,3}{4,30}{5,20}{6,12} %"
                    , name.WithQuality(quality)
                    , auctionGroup.Key.PricePerItem.ToGoldString()
                    , auctionGroup.Count()
                    , auctionGroup.Key.quantity
                    , auctionGroup.Key.time_left.ToAuctionTimeString() + "      " + auctionGroup.Key.FirstSeenOn.ToString("MM/dd HH:mm") + "  "
                    , (auctionGroup.Key.buyout * auctionGroup.Count()).Value.ToGoldString()
                    , percentDbMarket));
            }

            if (!showTotals) return;

            _console.WriteLine(String.Format("{0,20}{1,12}{2,20}"
                , "Grand total"
                , auctions.Sum(a => a.quantity)
                , auctions.Sum(a => a.FullPrice).ToGoldString()));

            _console.WriteLine(String.Format("{0,45}{1,12}{1,20}{2,20}", "Grand total per item", "", "Average %MarketValue"));

            foreach (var auctionGroup in auctions.GroupBy(a => a.item).OrderByDescending(g => g.Sum(a => a.buyout)))
            {
                WowItem item = _blizzard.GetItem(auctionGroup.First().item.id);
                TsmItem tsmItem = _tsm.GetItem(auctionGroup.First().item.id);
                string percentDbMarket = tsmItem != null ? Math.Round(auctionGroup.Average(a => (a.PricePerItem * 100.0) / tsmItem.MarketValue), 2).ToString() : "unknown";
                WowQualityType quality = item.quality.AsQualityTypeEnum;
                string name = item.name.en_US;
                _console.WriteLine(String.Format("{0,51}{1,12}{2,20}{3,18:0.00} %"
                    , name.WithQuality(quality)
                    , auctionGroup.Sum(a => a.quantity)
                    , auctionGroup.Sum(a => a.FullPrice).ToGoldString()
                    , percentDbMarket));
            }
        }

        private void ShowAuctions(TsmItem tsmItem, IEnumerable<Auction> auctions)
        {
            _console.WriteLine(String.Format("{0,20}{1,13}{2,20}{3,30}{4,14}", "Price per item", "Quantity", "Buyout total", "Time left (first seen on)", "% MarketValue"));
            foreach (var auctionGroup in auctions.GroupBy(a => new { a.PricePerItem, a.quantity, a.buyout, a.time_left, a.FirstSeenOn }).OrderBy(g => g.Key.PricePerItem))
            {
                string marketRatio = "?";
                if (tsmItem != null)
                    marketRatio = $"{Math.Round((auctionGroup.Key.PricePerItem * 100.0) / tsmItem.MarketValue)}";
                string pricePerItem = auctionGroup.Key.PricePerItem.ToGoldString();
                int auctionCount = auctionGroup.Count();
                int auctionQuantity = auctionGroup.Key.quantity;
                string auctionPrice = auctionGroup.Key.buyout.HasValue ? auctionGroup.Key.buyout.Value.ToGoldString() : (auctionGroup.Key.PricePerItem * auctionGroup.Key.quantity).ToGoldString();
                string timeInfo = $"{auctionGroup.Key.time_left.ToAuctionTimeString()}      {auctionGroup.Key.FirstSeenOn.ToString("MM/dd HH:mm")}  ";
                _console.WriteLine(string.Format("{0,20}{1,5}x {2,5}{3,20}{4,30}{5,12} %"
                    , pricePerItem
                    , auctionCount
                    , auctionQuantity
                    , auctionPrice
                    , timeInfo
                    , marketRatio));
            }

            string average = "?";
            if (tsmItem != null)
                average = $"{Math.Round(auctions.Average(a => Math.Round((a.PricePerItem * 100.0) / tsmItem.MarketValue)), 2)}";
            _console.WriteLine(String.Format("{0,20}{1,12}{2,20}{3,50}{4,14}"
                , "Grand total"
                , auctions.Sum(a => a.quantity)
                , auctions.Sum(a => a.FullPrice).ToGoldString()
                , ""
                , "avg:" + average + " %"
                )
            );
        }
    }
}
