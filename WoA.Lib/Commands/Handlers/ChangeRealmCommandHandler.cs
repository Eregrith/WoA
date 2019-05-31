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
    public class ChangeRealmCommandHandler : INotificationHandler<ChangeRealmCommand>
    {
        private readonly IConfiguration _config;
        private readonly ITsmClient _tsm;
        private readonly IBlizzardClient _blizzard;

        public ChangeRealmCommandHandler(IConfiguration config, ITsmClient tsm, IBlizzardClient blizzard)
        {
            _config = config;
            _tsm = tsm;
            _blizzard = blizzard;
        }

        public Task Handle(ChangeRealmCommand notification, CancellationToken cancellationToken)
        {
            string realm = notification.UserInput.Split(' ')[1];
            _config.CurrentRealm = realm;
            _tsm.RefreshTsmItemsInRepository();
            _blizzard.LoadAuctions();
            return Task.CompletedTask;
        }
    }
}
