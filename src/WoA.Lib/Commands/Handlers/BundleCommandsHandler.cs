using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Auctions;
using WoA.Lib.Commands.QueryObjects;
using WoA.Lib.TSM;

namespace WoA.Lib.Commands.Handlers
{
    public class BundleCommandsHandler
        : INotificationHandler<BundleAddCommand>
        , INotificationHandler<BundleListCommand>
        , INotificationHandler<BundleClearCommand>
    {
        private readonly IItemsBundler _itemsBundler;
        private readonly IAuctionViewer _auctionViewer;
        private readonly IStylizedConsole _console;
        private readonly ITsmClient _tsm;

        public BundleCommandsHandler(IItemsBundler itemsBundler, IAuctionViewer auctionViewer, IStylizedConsole console, ITsmClient tsm)
        {
            _itemsBundler = itemsBundler;
            _auctionViewer = auctionViewer;
            _console = console;
            _tsm = tsm;
        }

        public Task Handle(BundleAddCommand notification, CancellationToken cancellationToken)
        {
            int itemId = _auctionViewer.GetItemId(notification.ItemDescription);
            _itemsBundler.Add(itemId, notification.ItemQuantity);
            _console.WriteLine(notification.ItemQuantity + " x " +  notification.ItemDescription + " added to bundle");
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
    }
}
