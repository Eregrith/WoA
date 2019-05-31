using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;

namespace WoA.Lib.Commands.Handlers
{
    public class ShowTopSellersCommandHandler : INotificationHandler<ShowTopSellersCommand>
    {
        private readonly IAuctionViewer _auctionViewer;

        public ShowTopSellersCommandHandler(IAuctionViewer auctionViewer)
        {
            _auctionViewer = auctionViewer;
        }

        public Task Handle(ShowTopSellersCommand notification, CancellationToken cancellationToken)
        {
            _auctionViewer.SeeTopSellers();
            return Task.CompletedTask;
        }
    }
}
