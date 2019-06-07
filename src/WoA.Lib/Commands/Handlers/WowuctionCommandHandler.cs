using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;

namespace WoA.Lib.Commands.Handlers
{
  public  class WowuctionCommandHandler : INotificationHandler<WowuctionCommand>
    {
        private readonly IAuctionViewer _auctionViewer;
        private readonly IStylizedConsole _console;
        private readonly IConfiguration _config;

        public WowuctionCommandHandler(IAuctionViewer auctionViewer, IStylizedConsole console, IConfiguration config)
        {
            _auctionViewer = auctionViewer;
            _console = console;
            _config = config;
        }

        public Task Handle(WowuctionCommand notification, CancellationToken cancellationToken)
        {
            int itemId = _auctionViewer.GetItemId(notification.ItemDescription);
            _console.WriteLine("Opening wowuction's article on item");
            Process.Start("https://www.wowuction.com/" + _config.CurrentRegion + "/" + _config.CurrentRealm + "/Items/Stats/" + itemId);
            return Task.CompletedTask;
        }
    }
}
