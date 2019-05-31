using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
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
            string owner = notification.UserInput.Split(' ')[1];
            _auctionViewer.SeeAuctionsOwnedBy(owner);
            return Task.CompletedTask;
        }
    }
}
