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
        , INotificationHandler<BundleSaveCommand>
        , INotificationHandler<BundleLoadCommand>
        , INotificationHandler<BundleListCommand>
        , INotificationHandler<BundleClearCommand>
        , INotificationHandler<BundleFlipCommand>
    {
        private readonly IItemsBundler _itemsBundler;
        private readonly IAuctionViewer _auctionViewer;
        private readonly IStylizedConsole _console;
        private readonly ITsmClient _tsm;
        private readonly IGenericRepository _repo;

        public BundleCommandsHandler(IItemsBundler itemsBundler, IAuctionViewer auctionViewer, IStylizedConsole console, ITsmClient tsm, IGenericRepository repo)
        {
            _itemsBundler = itemsBundler;
            _auctionViewer = auctionViewer;
            _console = console;
            _tsm = tsm;
            _repo = repo;
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
    }
}
