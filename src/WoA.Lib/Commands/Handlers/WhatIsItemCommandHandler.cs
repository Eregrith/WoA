using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;

namespace WoA.Lib.Commands.Handlers
{
    public class WhatIsItemCommandHandler : INotificationHandler<WhatIsItemCommand>
    {
        private readonly IAuctionViewer _auctionViewer;
        private readonly IStylizedConsole _console;

        public WhatIsItemCommandHandler(IAuctionViewer auctionViewer, IStylizedConsole console)
        {
            _auctionViewer = auctionViewer;
            _console = console;
        }

        public Task Handle(WhatIsItemCommand notification, CancellationToken cancellationToken)
        {
            int itemId = _auctionViewer.GetItemId(notification.ItemDescription);
            _console.WriteLine("Opening wowhead's article on item");
            Process.Start("https://www.wowhead.com/item=" + itemId);
            return Task.CompletedTask;
        }
    }
}
