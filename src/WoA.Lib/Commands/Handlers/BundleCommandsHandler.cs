using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Auctions;
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
        , INotificationHandler<BundleShowSavedCommand>
        , INotificationHandler<BundleListCommand>
        , INotificationHandler<BundleClearCommand>
        , INotificationHandler<BundleFlipCommand>
        , INotificationHandler<BundleExportCommand>
        , INotificationHandler<BundleImportCommand>
        , INotificationHandler<BundleBuyCommand>
    {
        private readonly IItemsBundler _itemsBundler;
        private readonly IAuctionViewer _auctionViewer;
        private readonly IStylizedConsole _console;
        private readonly ITsmClient _tsm;
        private readonly IGenericRepository _repo;
        private readonly IClipboardManager _clipboard;

        public BundleCommandsHandler(
            IItemsBundler itemsBundler,
            IAuctionViewer auctionViewer,
            IStylizedConsole console,
            ITsmClient tsm,
            IGenericRepository repo,
            IClipboardManager clipboard
        )
        {
            _itemsBundler = itemsBundler;
            _auctionViewer = auctionViewer;
            _console = console;
            _tsm = tsm;
            _repo = repo;
            _clipboard = clipboard;
        }

        public Task Handle(BundleAddCommand notification, CancellationToken cancellationToken)
        {
            int itemId = _auctionViewer.GetItemId(notification.ItemDescription);
            if (itemId > 0)
            {
                _itemsBundler.Add(itemId, notification.ItemQuantity);
                TsmItem tsmItem = _tsm.GetItem(itemId);
                _console.WriteLine(notification.ItemQuantity + " x " + tsmItem.Name + " added to bundle");
            }
            else
            {
                _console.WriteLine("No item found called " + notification.ItemDescription);
            }
            return Task.CompletedTask;
        }

        public Task Handle(BundleSaveCommand notification, CancellationToken cancellationToken)
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
                    TsmItem tsmItem = _tsm.GetItem(itemId);
                    _console.WriteLine(quantity + " x " + tsmItem.Name + " added to bundle");
                }
                _console.WriteLine($"{notification.BundleName} loaded");
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
            Dictionary<int, int> bundle = _itemsBundler.GetItems();
            _console.WriteLine("Current bundle contains:");
            _console.WriteLine(String.Format("{0,-8} {1,20} {2,20} {3,20}", "Quantity", "Item", "Market Price", "Total"));
            long bigTotal = 0;
            foreach (KeyValuePair<int, int> item in bundle)
            {
                TsmItem tsmItem = _tsm.GetItem(item.Key);
                long itemTotal = (tsmItem.MarketValue * item.Value);
                _console.WriteLine(String.Format("{0,-8} {1,20} {2,20} {3,20}", item.Value, tsmItem.Name, tsmItem.MarketValue.ToGoldString(), itemTotal.ToGoldString()));
                bigTotal += itemTotal;
            }
            _console.WriteLine("This bundle's total price at market value is " + bigTotal.ToGoldString());
            return Task.CompletedTask;
        }

        public Task Handle(BundleClearCommand notification, CancellationToken cancellationToken)
        {
            _itemsBundler.Clear();
            _console.WriteLine("Bundle cleared");
            return Task.CompletedTask;
        }

        public Task Handle(BundleFlipCommand notification, CancellationToken cancellationToken)
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
            return Task.CompletedTask;
        }

        public Task Handle(BundleExportCommand notification, CancellationToken cancellationToken)
        {
            var items = _itemsBundler.GetItems();
            string tsmExportString = String.Join(",", items.Keys.Select(k => "i:" + k));
            _clipboard.SetText(tsmExportString);
            _console.WriteLine("TSM Export string for current bundle has been copied to clipboard");
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

        public Task Handle(BundleBuyCommand notification, CancellationToken cancellationToken)
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
            return Task.CompletedTask;
        }
    }
}
