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
    public class ToonsCommandHandler
        : INotificationHandler<ToonsCommand>
        , INotificationHandler<ToonsAddCommand>
        , INotificationHandler<ToonsRemoveCommand>
        , INotificationHandler<ToonsClearCommand>
    {
        private readonly IConfiguration _config;
        private readonly IStylizedConsole _console;
        private readonly IAuctionViewer _auctions;

        public ToonsCommandHandler(IConfiguration config, IStylizedConsole console, IAuctionViewer auctions)
        {
            _config = config;
            _console = console;
            _auctions = auctions;
        }

        public Task Handle(ToonsCommand notification, CancellationToken cancellationToken)
        {
            List<string> playerToons = _config.PlayerToons;

            if (!playerToons.Any())
                _console.WriteLine("You have no toons set up. Use 'toon add <toon name>' to add some");
            else
                foreach (string toon in playerToons)
                {
                    long totalAhForToon = _auctions.GetTotalAHPostedValueFor(toon);
                    _console.WriteLine(String.Format("{0:40} total posted value : {1}", toon, totalAhForToon.ToGoldString()));
                }

            return Task.CompletedTask;
        }

        public Task Handle(ToonsAddCommand notification, CancellationToken cancellationToken)
        {
            _config.PlayerToons.Add(notification.ToonName);
            _console.WriteLine("Toon " + notification.ToonName + " added to the list of toons");
            _config.Save();
            _console.InitStyleSheet();
            return Task.CompletedTask;
        }

        public Task Handle(ToonsRemoveCommand notification, CancellationToken cancellationToken)
        {
            if (_config.PlayerToons.Contains(notification.ToonName))
            {
                _config.PlayerToons.Remove(notification.ToonName);
                _console.WriteLine("Toon " + notification.ToonName + " removed to the list of toons");
                _config.Save();
                _console.InitStyleSheet();
            }
            else
            {
                _console.Write("No toon named " + notification.ToonName + " was found in the list.");
            }
            return Task.CompletedTask;
        }

        public Task Handle(ToonsClearCommand notification, CancellationToken cancellationToken)
        {
            _config.PlayerToons.Clear();
            _console.WriteLine("List of toons has been cleared");
            _config.Save();
            _console.InitStyleSheet();
            return Task.CompletedTask;
        }
    }
}
