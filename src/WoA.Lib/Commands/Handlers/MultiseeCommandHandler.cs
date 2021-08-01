using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Blizzard;
using WoA.Lib.Commands.QueryObjects;
using WoA.Lib.TSM;

namespace WoA.Lib.Commands.Handlers
{
    public class MultiseeCommandHandler : INotificationHandler<MultiseeCommand>
    {
        private readonly IAuctionViewer _auctions;
        private readonly IBlizzardClient _blizzard;
        private readonly IStylizedConsole _console;

        public MultiseeCommandHandler(IAuctionViewer auctions, IBlizzardClient blizzard, IStylizedConsole console)
        {
            _auctions = auctions;
            _blizzard = blizzard;
            _console = console;
        }

        public Task Handle(MultiseeCommand notification, CancellationToken cancellationToken)
        {
            IEnumerable<WowItem> items = _blizzard.GetItemsWithNameLike(notification.PartialItemName);
            if (!items.Any())
            {
                _console.WriteLine("No auctions to show for this partial item name: " + notification.PartialItemName);
                return Task.CompletedTask;
            }
            _auctions.ShowAuctionsForMultiItems(new List<Auction>(), true, false);
            foreach (WowItem item in items)
            {
                _console.WriteLine($"--------------------------------------------------");
                IEnumerable<Auction> auctions = _blizzard.Auctions.Where(a => a.item.id == item.id).OrderBy(a => a.PricePerItem).Take(notification.Amount);
                if (auctions.Any())
                    _auctions.ShowAuctionsForMultiItems(auctions, false, false);
                else
                    _console.WriteLine($"No {item.name.en_US.WithQuality(item.quality.AsQualityTypeEnum)} [{item.id}] found.");
                _console.WriteLine();
            }
            return Task.CompletedTask;
        }
    }
}
