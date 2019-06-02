using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;

namespace WoA.Lib.Commands.Handlers
{
    public class UndermineJournalCommandHandler : INotificationHandler<UndermineJournalCommand>
    {
        private readonly IAuctionViewer _auctionViewer;
        private readonly IStylizedConsole _console;
        private readonly IConfiguration _config;

        public UndermineJournalCommandHandler(IAuctionViewer auctionViewer, IStylizedConsole console, IConfiguration config)
        {
            _auctionViewer = auctionViewer;
            _console = console;
            _config = config;
        }

        public Task Handle(UndermineJournalCommand notification, CancellationToken cancellationToken)
        {
            int itemId = _auctionViewer.GetItemId(notification.ItemDescription);
            _console.WriteLine("Opening undermine journal's article on item");
            Process.Start("https://theunderminejournal.com/#" + _config.CurrentRegion + "/" + _config.CurrentRealm + "/item/" + itemId);
            return Task.CompletedTask;
        }
    }
}
