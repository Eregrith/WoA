using MediatR;
using Newtonsoft.Json;
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
    public class WhoIsCommandHandler : INotificationHandler<WhoIsCommand>
    {
        private readonly IBlizzardClient _blizzard;
        private readonly IStylizedConsole _console;

        public WhoIsCommandHandler(IBlizzardClient blizzard, IStylizedConsole console)
        {
            _blizzard = blizzard;
            _console = console;
        }

        public Task Handle(WhoIsCommand notification, CancellationToken cancellationToken)
        {
            if (!_blizzard.Auctions.Any(a => a.owner == notification.SellerName))
            {
                _console.WriteLine("No seller found with this name.");
                return Task.CompletedTask;
            }
            _console.WriteAscii(notification.SellerName);
            string sellerRealm = _blizzard.Auctions.First(a => a.owner == notification.SellerName).ownerRealm;
            CharacterProfile seller = _blizzard.GetInfosOnCharacter(notification.SellerName, sellerRealm);

            _console.WriteLine(String.Format("Lvl {0} {1} {2} {3} ({4})", seller.level, seller.Gender, seller.RaceName, seller.ClassName, seller.Faction));
            _console.WriteLine(String.Format("{0} achievement points", seller.achievementPoints));
            _console.WriteLine(String.Format("{0} honorable kills", seller.totalHonorableKills));
            return Task.CompletedTask;
        }
    }
}
