using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;
using WoA.Lib.Items;

namespace WoA.Lib.Commands.Handlers
{
    public class SeeAuctionsCommandHandler : INotificationHandler<SeeAuctionsCommand>
    {
        private readonly IAuctionViewer _auctionViewer;
        private readonly IItemHelper _itemHelper;
        private readonly IStylizedConsole _console;

        public SeeAuctionsCommandHandler(IAuctionViewer auctionViewer, IItemHelper itemHelper, IStylizedConsole console)
        {
            _auctionViewer = auctionViewer;
            _itemHelper = itemHelper;
            _console = console;
        }

        public Task Handle(SeeAuctionsCommand notification, CancellationToken cancellationToken)
        {
            string itemId = _itemHelper.GetItemId(notification.ItemDescription);
            if (String.IsNullOrEmpty(itemId))
            {
                _console.WriteLine("[Error] Can't see auctions for item:" +
                    " No item id was found for name " + notification.ItemDescription);
                return Task.CompletedTask;
            }
            _auctionViewer.SeeAuctionsFor(itemId);
            return Task.CompletedTask;
        }
    }
}
