﻿using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;

namespace WoA.Lib.Commands.Handlers
{
    public class FlipCommandHandler : INotificationHandler<FlipCommand>
    {
        private readonly IAuctionViewer _auctionViewer;

        public FlipCommandHandler(IAuctionViewer auctionViewer)
        {
            _auctionViewer = auctionViewer;
        }

        public Task Handle(FlipCommand notification, CancellationToken cancellationToken)
        {
            int itemId = _auctionViewer.GetItemId(notification.ItemDescription);
            if (itemId != 0)
                _auctionViewer.SimulateFlippingItem(itemId);
            return Task.CompletedTask;
        }
    }
}
