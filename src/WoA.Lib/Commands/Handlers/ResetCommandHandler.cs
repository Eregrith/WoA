using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;
using WoA.Lib.Items;

namespace WoA.Lib.Commands.Handlers
{
  public  class ResetCommandHandler : INotificationHandler<ResetCommand>
    {
        private readonly IAuctionViewer _auctionViewer;
        private readonly IItemHelper _itemHelper;
        private readonly IStylizedConsole _console;

        public ResetCommandHandler(IAuctionViewer auctionViewer, IItemHelper itemHelper, IStylizedConsole console)
        {
            _auctionViewer = auctionViewer;
            _itemHelper = itemHelper;
            _console = console;
        }

        public Task Handle(ResetCommand notification, CancellationToken cancellationToken)
        {
            string itemId = _itemHelper.GetItemId(notification.ItemDescription);
            if (String.IsNullOrEmpty(itemId))
            {
                _console.WriteLine("[Error] Can't simulate reset:" +
                    " No item id was found for name " + notification.ItemDescription);
                return Task.CompletedTask;
            }
            _auctionViewer.SimulateResettingItem(itemId,notification.MaxBuyPercent,notification.SellPricePercent);
            return Task.CompletedTask;
        }
    }
}
