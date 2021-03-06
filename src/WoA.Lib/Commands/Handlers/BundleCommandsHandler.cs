﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Auctions;
using WoA.Lib.Blizzard;
using WoA.Lib.Commands.QueryObjects;
using WoA.Lib.SQLite;
using WoA.Lib.TSM;

namespace WoA.Lib.Commands.Handlers
{
    public class BundleCommandsHandler
        : INotificationHandler<BundleAddCommand>
        , INotificationHandler<BundleRemoveCommand>
        , INotificationHandler<BundleSaveCommand>
        , INotificationHandler<BundleLoadCommand>
        , INotificationHandler<BundleDeleteSaveCommand>
        , INotificationHandler<BundleShowSavedCommand>
        , INotificationHandler<BundleListCommand>
        , INotificationHandler<BundleClearCommand>
        , INotificationHandler<BundleFlipCommand>
        , INotificationHandler<BundleExportCommand>
        , INotificationHandler<BundleImportCommand>
        , INotificationHandler<BundleBuyCommand>
        , INotificationHandler<BundleMultiseeCommand>
    {
        private readonly IItemsBundler _itemsBundler;
        private readonly IAuctionViewer _auctionViewer;
        private readonly IStylizedConsole _console;
        private readonly ITsmClient _tsm;
        private readonly IBlizzardClient _blizzard;
        private readonly IGenericRepository _repo;
        private readonly IClipboardManager _clipboard;

        public BundleCommandsHandler(
            IItemsBundler itemsBundler,
            IAuctionViewer auctionViewer,
            IStylizedConsole console,
            ITsmClient tsm,
            IBlizzardClient blizzard,
            IGenericRepository repo,
            IClipboardManager clipboard
        )
        {
            _itemsBundler = itemsBundler;
            _auctionViewer = auctionViewer;
            _console = console;
            _tsm = tsm;
            _blizzard = blizzard;
            _repo = repo;
            _clipboard = clipboard;
        }

        public Task Handle(BundleAddCommand notification, CancellationToken cancellationToken)
        {
            int itemId = _auctionViewer.GetItemId(notification.ItemDescription);
            if (itemId > 0)
            {
                _itemsBundler.Add(itemId, notification.ItemQuantity);
                WowItem wowItem = _blizzard.GetItem(itemId);
                WowQuality quality = (WowQuality)wowItem.quality;
                _console.WriteLine(notification.ItemQuantity + " x " + wowItem.name.WithQuality(quality) + " added to bundle");
            }
            else
            {
                _console.WriteLine("No item found called " + notification.ItemDescription);
            }
            return Task.CompletedTask;
        }

        public Task Handle(BundleRemoveCommand notification, CancellationToken cancellationToken)
        {
            if (IsYourCurrentBundleValid())
            {
                int itemId = _auctionViewer.GetItemId(notification.ItemDescription);
                if (itemId > 0)
                {
                    TsmItem tsmItem = _tsm.GetItem(itemId);
                    WowItem wowItem = _blizzard.GetItem(itemId);
                    WowQuality quality = (WowQuality)wowItem.quality;
                    if (tsmItem != null)
                        _console.WriteLine("No TSM data for item " + notification.ItemDescription);

                    if (_itemsBundler.Remove(itemId, notification.ItemQuantity, notification.RemoveAllQuantity))
                    {
                        if (notification.RemoveAllQuantity)
                        {
                            _console.WriteLine(wowItem.name.WithQuality(quality) + " removed from bundle");
                        }
                        else
                        {
                            _console.WriteLine(notification.ItemQuantity + " x " + wowItem.name.WithQuality(quality) + " removed from bundle");
                        }
                    }
                    else
                    {
                        _console.WriteLine(wowItem.name.WithQuality(quality) + " doesn't exist in bundle");
                    }
                }
                else
                {
                    _console.WriteLine("No item found called " + notification.ItemDescription);
                }
            }
            return Task.CompletedTask;
        }

        public Task Handle(BundleSaveCommand notification, CancellationToken cancellationToken)
        {
            if (IsYourCurrentBundleValid())
            {
                ItemBundle itemBundle = _repo.GetById<ItemBundle>(notification.BundleName);
                if (itemBundle != null && !string.IsNullOrWhiteSpace(itemBundle.ItemsId))
                {
                    _console.WriteLine($"There already a bundle saved with name : {itemBundle.BundleName}");
                    _console.WriteLine($"Do you want to update it (yes/no) ?");
                    string answer = Console.ReadLine();
                    if (answer.Equals("yes"))
                    {
                        Dictionary<int, int> bundle = _itemsBundler.GetItems();
                        itemBundle = new ItemBundle()
                        {
                            BundleName = notification.BundleName,
                            ItemsId = String.Join(",", bundle.Keys),
                            ItemsValue = String.Join(",", bundle.Values),
                        };
                        _repo.Update(itemBundle);
                        _console.WriteLine($"{itemBundle.BundleName} updated");
                    }
                }
                else
                {
                    Dictionary<int, int> bundle = _itemsBundler.GetItems();
                    itemBundle = new ItemBundle()
                    {
                        BundleName = notification.BundleName,
                        ItemsId = String.Join(",", bundle.Keys),
                        ItemsValue = String.Join(",", bundle.Values),
                    };
                    _repo.Add(itemBundle);
                    _console.WriteLine($"Bundle saved with name : {itemBundle.BundleName}");
                }
            }
            return Task.CompletedTask;
        }

        public Task Handle(BundleLoadCommand notification, CancellationToken cancellationToken)
        {
            ItemBundle itemBundle = _repo.GetById<ItemBundle>(notification.BundleName);
            if (itemBundle != null && !string.IsNullOrWhiteSpace(itemBundle.ItemsId))
            {
                _itemsBundler.Clear();
                string[] test = itemBundle.ItemsId.Split(',');
                string[] testValue = itemBundle.ItemsValue.Split(',');
                for (int i = 0; i < test.Length; i++)
                {
                    int itemId = int.Parse(test[i]);
                    int quantity = int.Parse(testValue[i]);
                    _itemsBundler.Add(itemId, quantity);
                    WowItem wowItem = _blizzard.GetItem(itemId);
                    WowQuality quality = (WowQuality)wowItem.quality;
                    _console.WriteLine(quantity + " x " + wowItem.name.WithQuality(quality) + " added to bundle");
                }
                _console.WriteLine($"{notification.BundleName} loaded");
            }
            else
            {
                _console.WriteLine($"No bundle found with name : {notification.BundleName}");
            }

            return Task.CompletedTask;
        }

        public Task Handle(BundleDeleteSaveCommand notification, CancellationToken cancellationToken)
        {
            ItemBundle itemBundle = _repo.GetById<ItemBundle>(notification.BundleName);
            if (itemBundle != null && !string.IsNullOrWhiteSpace(itemBundle.ItemsId))
            {
                _repo.Delete(itemBundle);
                _console.WriteLine($"{notification.BundleName} deleted");
            }
            else
            {
                _console.WriteLine($"No bundle found with name : {notification.BundleName}");
            }

            return Task.CompletedTask;
        }

        public Task Handle(BundleShowSavedCommand notification, CancellationToken cancellationToken)
        {
            ItemBundle[] itemBundles = _repo.GetAll<ItemBundle>();
            if (itemBundles != null && itemBundles.Any())
            {
                _console.WriteLine("Bundles saved : ");
                _console.WriteLine(String.Format("{0,-35} {1,20}", "Name", "# distinct items"));
                foreach (ItemBundle itemBundle in itemBundles)
                {
                    _console.WriteLine(String.Format("{0,-35} {1,20}", itemBundle.BundleName, itemBundle.ItemsId.Split(',').Count()));
                }
            }
            else
            {
                _console.WriteLine("No bundle found.");
            }

            return Task.CompletedTask;
        }

        public Task Handle(BundleListCommand notification, CancellationToken cancellationToken)
        {
            if (IsYourCurrentBundleValid())
            {
                Dictionary<int, int> bundle = _itemsBundler.GetItems();
                _console.WriteLine("Current bundle contains:");
                _console.WriteLine(String.Format("{0,7} {1,8} {2,40} {3,40} {4,40}", "ItemId", "Quantity", "Item", "Market Price", "Total"));
                long bigTotal = 0;
                foreach (KeyValuePair<int, int> item in bundle)
                {
                    TsmItem tsmItem = _tsm.GetItem(item.Key);
                    WowItem wowItem = _blizzard.GetItem(item.Key);
                    WowQuality quality = (WowQuality)wowItem.quality;
                    if (tsmItem == null)
                        _console.WriteLine(String.Format("{0,7} {1,8} {2,46} {3,40} {4,40}", item.Key, item.Value + " x", wowItem.name.WithQuality(quality), "unknown", "unknown"));
                    else
                    {
                        long itemPrice = tsmItem.VendorBuy != 0 ? tsmItem.VendorBuy : tsmItem.MarketValue;
                        long itemTotal = (itemPrice * item.Value);
                        _console.WriteLine(String.Format("{0,7} {1,8} {2,46} {3,40} {4,40}", item.Key, item.Value + " x", wowItem.name.WithQuality(quality), (itemPrice == tsmItem.VendorBuy ? "(vendor) " + itemPrice.ToGoldString() : itemPrice.ToGoldString()), itemTotal.ToGoldString()));
                        bigTotal += itemTotal;
                    }
                }
                _console.WriteLine("This bundle's total price at market value is " + bigTotal.ToGoldString());
            }
            return Task.CompletedTask;
        }

        public Task Handle(BundleClearCommand notification, CancellationToken cancellationToken)
        {
            if (IsYourCurrentBundleValid())
            {
                _itemsBundler.Clear();
                _console.WriteLine("Bundle cleared");
            }
            return Task.CompletedTask;
        }

        public Task Handle(BundleFlipCommand notification, CancellationToken cancellationToken)
        {
            if (IsYourCurrentBundleValid())
            {
                Dictionary<int, int> bundle = _itemsBundler.GetItems();
                List<ItemFlipResult> itemFlipResults = new List<ItemFlipResult>();

                _console.WriteLine("Flipping current bundle will result:");
                _console.WriteLine(String.Format("{0,-35} {1,20} {2,15} {3,20} {4,20} {5,20}", "Item", "Market Price", "Qty available", "Total buyout", "Net profit", "Percent profit"));
                foreach (KeyValuePair<int, int> item in bundle)
                {
                    itemFlipResults.Add(_auctionViewer.SimulateFlippingItemShortVersion(item.Key));
                }

                _console.WriteLine(" ");
                _console.WriteLine(String.Format("{0,-35} {1,20} {2,15} {3,20} {4,20} {5,20}",
                    "Total", " ", itemFlipResults.Sum(x => x.Quantity), itemFlipResults.Sum(x => x.TotalBuyout).ToGoldString(),
                    itemFlipResults.Sum(x => x.NetProfit).ToGoldString(),
                    Math.Round(((double)itemFlipResults.Sum(x => x.NetProfit) / itemFlipResults.Sum(x => x.TotalBuyout)) * 100, 2) + "%"));
            }
            return Task.CompletedTask;
        }

        public Task Handle(BundleBuyCommand notification, CancellationToken cancellationToken)
        {
            if (IsYourCurrentBundleValid())
            {
                Dictionary<int, int> bundle = _itemsBundler.GetItems();
                List<ItemBuyResult> itemFlipResults = new List<ItemBuyResult>();

                _console.WriteLine($"Buying current bundle's items at {notification.PercentMax}% of market value :");
                _console.WriteLine(String.Format("{0,-35} {1,20} {2,15} {3,20}", "Item", "Market Price", "Qty available", "Total buyout"));
                foreach (KeyValuePair<int, int> item in bundle)
                {
                    itemFlipResults.Add(_auctionViewer.SimulateBuyingItemShortVersion(item.Key, item.Value, notification.PercentMax));
                }

                _console.WriteLine(" ");
                _console.WriteLine(String.Format("{0,-35} {1,20} {2,15} {3,20}",
                    "Total", " ", itemFlipResults.Sum(x => x.Quantity), itemFlipResults.Sum(x => x.TotalBuyout).ToGoldString()));
            }
            return Task.CompletedTask;
        }

        public Task Handle(BundleExportCommand notification, CancellationToken cancellationToken)
        {
            if (IsYourCurrentBundleValid())
            {
                var items = _itemsBundler.GetItems();
                string tsmExportString = String.Join(",", items.Keys.Select(k => "i:" + k));
                _clipboard.SetText(tsmExportString);
                _console.WriteLine("TSM Export string for current bundle has been copied to clipboard");
            }
            return Task.CompletedTask;
        }

        public Task Handle(BundleMultiseeCommand notification, CancellationToken cancellationToken)
        {
            if (IsYourCurrentBundleValid())
            {
                Dictionary<int, int> bundle = _itemsBundler.GetItems();
                _auctionViewer.ShowAuctionsForMultiItems(new List<Auction>(), true, false);
                foreach (KeyValuePair<int, int> item in bundle)
                {
                    _console.WriteLine($"--------------------------------------------------");
                    IEnumerable<Auction> auctions = _blizzard.Auctions.Where(a => a.item == item.Key).OrderBy(a => a.PricePerItem).Take(notification.Amount);
                    WowItem wowItem = _blizzard.GetItem(item.Key);
                    if (auctions.Any())
                        _auctionViewer.ShowAuctionsForMultiItems(auctions, false, false);
                    else
                        _console.WriteLine($"No {wowItem.name.WithQuality((WowQuality)wowItem.quality)} [{item.Key}] found.");
                    _console.WriteLine();
                }
            }
            return Task.CompletedTask;
        }

        public Task Handle(BundleImportCommand notification, CancellationToken cancellationToken)
        {
            _console.WriteLine("Paste TSM import string here");
            _console.WriteLine("============================");
            string line = Console.ReadLine();
            List<int> items = new List<int>();
            List<string> parts = line.Split(',').ToList();
            foreach (string part in parts)
            {
                if (int.TryParse(part.Split(':')[1], out int itemId))
                {
                    items.Add(itemId);
                }
            }
            if (items.Any())
            {
                _itemsBundler.Clear();
                items.ForEach(i => _itemsBundler.Add(i, 1));
                _console.WriteLine(items.Count + " items imported");
            }
            return Task.CompletedTask;
        }

        private bool IsYourCurrentBundleValid()
        {
            Dictionary<int, int> bundle = _itemsBundler.GetItems();
            if (bundle == null || !bundle.Any())
            {
                _console.WriteLine("Your current bundle is empty");
                return false;
            }
            return true;
        }
    }
}
