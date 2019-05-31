using MediatR;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.Attributes;
using WoA.Lib.Commands.QueryObjects;

namespace WoA.Lib.Commands.Handlers
{
    public class ParseCommandHandler : INotificationHandler<ParseCommand>
    {
        private readonly IMediator _mediator;
        private readonly IStylizedConsole _console;

        public ParseCommandHandler(IMediator mediator, IStylizedConsole console)
        {
            _mediator = mediator;
            _console = console;
        }

        public Task Handle(ParseCommand notification, CancellationToken cancellationToken)
        {
            var woaCommands = this.GetType().Assembly.GetTypes().Where(t => t.GetCustomAttribute<WoACommandAttribute>() != null);
            bool matched = false;
            foreach (var woaCommand in woaCommands)
            {
                string regex = woaCommand.GetCustomAttribute<WoACommandAttribute>().RegexToMatch;
                Regex r = new Regex(regex);
                Match m = r.Match(notification.UserInput);
                if (m.Success)
                {
                    var command = Activator.CreateInstance(woaCommand, m);
                    _mediator.Publish(command);
                    matched = true;
                }
            }
            if (!matched)
                _console.WriteLine("No command matched your input. To see a list of commands type: help");
            return Task.CompletedTask;
        }
    }
}
