using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;

namespace WoA.Lib.Commands.Handlers
{
  public  class ResetCommandHandler : INotificationHandler<ResetCommand>
    {
        private readonly IAuctionViewer _auctionViewer;

        public ResetCommandHandler(IAuctionViewer auctionViewer)
        {
            _auctionViewer = auctionViewer;
        }

        public Task Handle(ResetCommand notification, CancellationToken cancellationToken)
        {
            int itemId = _auctionViewer.GetItemId(notification.ItemDescription);
            if (itemId != 0)
                _auctionViewer.SimulateResettingItem(itemId,90,110);
            return Task.CompletedTask;
        }
    }
}
