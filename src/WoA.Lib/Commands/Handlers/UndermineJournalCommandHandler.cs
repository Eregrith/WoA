using MediatR;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;
using WoA.Lib.Items;

namespace WoA.Lib.Commands.Handlers
{
    public class UndermineJournalCommandHandler : INotificationHandler<UndermineJournalCommand>
    {
        private readonly IStylizedConsole _console;
        private readonly IConfiguration _config;
        private readonly IItemHelper _itemHelper;

        public UndermineJournalCommandHandler(IStylizedConsole console, IConfiguration config, IItemHelper itemHelper)
        {
            _console = console;
            _config = config;
            _itemHelper = itemHelper;
        }

        public Task Handle(UndermineJournalCommand notification, CancellationToken cancellationToken)
        {
            string itemId = _itemHelper.GetItemId(notification.ItemDescription);
            if (String.IsNullOrEmpty(itemId))
            {
                _console.WriteLine("[Error] Can't open the undermine journal for item:" +
                    " No item id was found for name " + notification.ItemDescription);
                return Task.CompletedTask;
            }
            _console.WriteLine("Opening undermine journal's article on item");
            Process.Start("https://theunderminejournal.com/#" + _config.CurrentRegion + "/" + _config.CurrentRealm + "/item/" + itemId);
            return Task.CompletedTask;
        }
    }
}
