using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;

namespace WoA.Lib.Commands.Handlers
{
    public class ParseCommandHandler : INotificationHandler<ParseCommand>
    {
        private readonly IMediator _mediator;

        public ParseCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Handle(ParseCommand notification, CancellationToken cancellationToken)
        {
            if (notification.UserInput.StartsWith("flip "))
            {
                _mediator.Publish(new FlipCommand { UserInput = notification.UserInput });
            }
            else if (notification.UserInput.StartsWith("see "))
            {
                _mediator.Publish(new SeeAuctionsCommand { UserInput = notification.UserInput });
            }
            else if (notification.UserInput.StartsWith("spy "))
            {
                _mediator.Publish(new SpySellerCommand { UserInput = notification.UserInput });
            }
            else if (notification.UserInput.StartsWith("top "))
            {
                _mediator.Publish(new ShowTopSellersCommand());
            }
            else if (notification.UserInput.StartsWith("whatis "))
            {
                _mediator.Publish(new WhatIsItemCommand { UserInput = notification.UserInput });
            }
            else if (notification.UserInput.StartsWith("chrealm "))
            {
                _mediator.Publish(new ChangeRealmCommand { UserInput = notification.UserInput });
            }
            return Task.CompletedTask;
        }
    }
}
