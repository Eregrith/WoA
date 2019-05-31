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

        public StartupCommandHandler(ITsmClient tsm, IBlizzardClient blizzard)
        {
            _tsm = tsm;
            _blizzard = blizzard;
        }

        public Task Handle(StartupCommand notification, CancellationToken cancellationToken)
        {
            _tsm.RefreshTsmItemsInRepository();
            _blizzard.LoadAuctions();
            return Task.CompletedTask;
        }
    }
}
