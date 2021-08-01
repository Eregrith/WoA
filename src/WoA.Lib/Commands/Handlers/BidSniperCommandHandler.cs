using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Blizzard;
using WoA.Lib.Commands.QueryObjects;

namespace WoA.Lib.Commands.Handlers
{
    public class BidSniperCommandHandler : INotificationHandler<BidSniperCommand>
    {
        private readonly IBlizzardClient _blizzard;
        private readonly IAuctionViewer _auctions;

        public BidSniperCommandHandler(IAuctionViewer auctions, IBlizzardClient blizzard)
        {
            _auctions = auctions;
            _blizzard = blizzard;
        }

        public Task Handle(BidSniperCommand notification, CancellationToken cancellationToken)
        {
            IEnumerable<Auction> auctionsSoonExpired = _blizzard.Auctions.Where(a => a.time_left == "SHORT" && a.bid < (a.buyout / 5));
            _auctions.ShowAuctionsForMultiItems(auctionsSoonExpired);
            return Task.CompletedTask;
        }
    }
}
