﻿using MediatR;
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
            _auctions.ShowAuctionsForMultiItems(new List<Auction>(), true, false);
            foreach (WowItem item in items)
            {
                _console.WriteLine($"--------------------------------------------------");
                IEnumerable<Auction> auctions = _blizzard.Auctions.Where(a => a.item.ToString() == item.id).OrderBy(a => a.PricePerItem).Take(notification.Amount);
                if (auctions.Any())
                    _auctions.ShowAuctionsForMultiItems(auctions, false, false);
                else
                    _console.WriteLine($"No {item.name.WithQuality((WowQuality)item.quality)} [{item.id}] found.");
                _console.WriteLine();
            }
            return Task.CompletedTask;
        }
    }
}
