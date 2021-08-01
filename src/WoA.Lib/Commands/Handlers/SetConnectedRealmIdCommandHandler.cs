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
    public class SetConnectedRealmIdCommandHandler
        : INotificationHandler<SetConnectedRealmIdCommand>
    {
        private readonly IConfiguration _config;
        private readonly IStylizedConsole _console;
        private readonly IBlizzardClient _blizzard;

        public SetConnectedRealmIdCommandHandler(IConfiguration config, IStylizedConsole console, IBlizzardClient blizzard)
        {
            _config = config;
            _console = console;
            _blizzard = blizzard;
        }

        public Task Handle(SetConnectedRealmIdCommand notification, CancellationToken cancellationToken)
        {
            string confirm;
            do
            {
                _console.WriteLine("What is your realm slug (i.e. without punctuation, e.g. Drek'Thar slug is drekthar");
                string realmName = Console.ReadLine();

                ConnectedRealmSearchData connectedRealm = _blizzard.SearchConnectedRealmsForEnglishName(realmName.ToLower());

                _console.WriteLine("Connected realms found:");
                _console.WriteLine(connectedRealm.id + " : " + connectedRealm.FullConnectedRealmEnglishNames);
                _console.WriteLine("Is that correct ? (y / n)");
                confirm = Console.ReadLine();
                if (confirm == "y" || confirm == "yes")
                {
                    _config.CurrentRealm = realmName.ToLower();
                    _config.ConnectedRealmId = connectedRealm.id;
                }
            } while (confirm != "y" && confirm != "yes");

            return Task.CompletedTask;
        }
    }
}
