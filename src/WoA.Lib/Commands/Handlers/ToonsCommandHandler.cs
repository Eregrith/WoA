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
    public class ToonsCommandHandler : INotificationHandler<ToonsCommand>
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

            foreach (string toon in playerToons)
            {
                long totalAhForToon = _auctions.GetTotalAHPostedValueFor(toon);
                _console.WriteLine(String.Format("{0:40} total posted value : {1}", toon, totalAhForToon.ToGoldString()));
            }

            return Task.CompletedTask;
        }
    }
}
