using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Blizzard;
using WoA.Lib.Commands.QueryObjects;
using WoA.Lib.TSM;

namespace WoA.Lib.Commands.Handlers
{
    public class StartupCommandHandler : INotificationHandler<StartupCommand>
    {
        private readonly ITsmClient _tsm;
        private readonly IBlizzardClient _blizzard;
        private readonly IStylizedConsole _console;

        public StartupCommandHandler(ITsmClient tsm, IBlizzardClient blizzard, IStylizedConsole console)
        {
            _tsm = tsm;
            _blizzard = blizzard;
            _console = console;
        }

        public Task Handle(StartupCommand notification, CancellationToken cancellationToken)
        {
            _console.WriteLine($"World of Auctions v{notification.CurrentVersion} started");
            _tsm.RefreshTsmItemsInRepository();
            return Task.CompletedTask;
        }
    }
}
