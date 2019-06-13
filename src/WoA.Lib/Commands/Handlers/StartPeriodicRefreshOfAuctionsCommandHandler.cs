using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Blizzard;
using WoA.Lib.Commands.QueryObjects;

namespace WoA.Lib.Commands.Handlers
{
    public class StartPeriodicRefreshOfAuctionsCommandHandler : INotificationHandler<StartPeriodicRefreshOfAuctionsCommand>
    {
        private readonly IStylizedConsole _console;
        private readonly IBlizzardClient _blizzard;
        private Timer _timer;

        public StartPeriodicRefreshOfAuctionsCommandHandler(IStylizedConsole console, IBlizzardClient blizzard)
        {
            _console = console;
            _blizzard = blizzard;
        }

        public Task Handle(StartPeriodicRefreshOfAuctionsCommand notification, CancellationToken cancellationToken)
        {
            _timer = new Timer(RefreshAuctions, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
            _console.WriteLine("Automatic periodic refresh of auctions started. Auctions will be scanned for an update every ten minutes.");
            _console.WriteLine("Notifications will pop when the updates happen.");
            return Task.CompletedTask;
        }

        private void RefreshAuctions(object state)
        {
            _blizzard.LoadAuctions();
        }
    }
}
