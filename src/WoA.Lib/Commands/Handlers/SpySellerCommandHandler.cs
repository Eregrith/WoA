using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;

namespace WoA.Lib.Commands.Handlers
{
    public class SpySellerCommandHandler : INotificationHandler<SpySellerCommand>
    {
        private readonly IAuctionViewer _auctionViewer;

        public SpySellerCommandHandler(IAuctionViewer auctionViewer)
        {
            _auctionViewer = auctionViewer;
        }

        public Task Handle(SpySellerCommand notification, CancellationToken cancellationToken)
        {
            _auctionViewer.SeeAuctionsOwnedBy(notification.SellerName);
            return Task.CompletedTask;
        }
    }
}
