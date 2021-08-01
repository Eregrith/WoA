using MediatR;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;
using WoA.Lib.Items;

namespace WoA.Lib.Commands.Handlers
{
  public  class WowuctionCommandHandler : INotificationHandler<WowuctionCommand>
    {
        private readonly IStylizedConsole _console;
        private readonly IConfiguration _config;
        private readonly IItemHelper _itemHelper;

        public WowuctionCommandHandler(IStylizedConsole console, IConfiguration config, IItemHelper itemHelper)
        {
            _console = console;
            _config = config;
            _itemHelper = itemHelper;
        }

        public Task Handle(WowuctionCommand notification, CancellationToken cancellationToken)
        {
            string itemId = _itemHelper.GetItemId(notification.ItemDescription);
            if (String.IsNullOrEmpty(itemId))
            {
                _console.WriteLine("[Error] Can't open wowuction page for item:" +
                    " No item id was found for name " + notification.ItemDescription);
                return Task.CompletedTask;
            }
            _console.WriteLine("Opening wowuction's article on item");
            Process.Start("https://www.wowuction.com/" + _config.CurrentRegion + "/" + _config.CurrentRealm + "/Items/Stats/" + itemId);
            return Task.CompletedTask;
        }
    }
}
