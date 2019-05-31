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
            return Task.CompletedTask;
        }
    }
}
