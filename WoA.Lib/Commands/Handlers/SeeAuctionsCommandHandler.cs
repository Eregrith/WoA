using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;

namespace WoA.Lib.Commands.Handlers
{
    public class SeeAuctionsCommandHandler : INotificationHandler<SeeAuctionsCommand>
    {
        private readonly IAuctionViewer _auctionViewer;

        public SeeAuctionsCommandHandler(IAuctionViewer auctionViewer)
        {
            _auctionViewer = auctionViewer;
        }

        public Task Handle(SeeAuctionsCommand notification, CancellationToken cancellationToken)
        {
            int itemId = _auctionViewer.GetItemId(notification.ItemDescription);
            if (itemId != 0)
                _auctionViewer.SeeAuctionsFor(itemId);
            return Task.CompletedTask;
        }
    }
}
