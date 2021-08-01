using MediatR;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;
using WoA.Lib.Items;

namespace WoA.Lib.Commands.Handlers
{
    public class WhatIsItemCommandHandler : INotificationHandler<WhatIsItemCommand>
    {
        private readonly IStylizedConsole _console;
        private readonly IItemHelper _itemHelper;

        public WhatIsItemCommandHandler(IStylizedConsole console, IItemHelper itemHelper)
        {
            _console = console;
            _itemHelper = itemHelper;
        }

        public Task Handle(WhatIsItemCommand notification, CancellationToken cancellationToken)
        {
            string itemId = _itemHelper.GetItemId(notification.ItemDescription);
            if (String.IsNullOrEmpty(itemId))
            {
                _console.WriteLine("[Error] Can't open wowhead article:" +
                    " No item id was found for name " + notification.ItemDescription);
                return Task.CompletedTask;
            }
            _console.WriteLine("Opening wowhead's article on item");
            Process.Start("https://www.wowhead.com/item=" + itemId);
            return Task.CompletedTask;
        }
    }
}
