using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.Attributes;
using WoA.Lib.Commands.QueryObjects;

namespace WoA.Lib.Commands.Handlers
{
    public class HelpCommandHandler : INotificationHandler<HelpCommand>
    {
        private readonly IStylizedConsole _console;

        public HelpCommandHandler(IStylizedConsole console)
        {
            _console = console;
        }

        public Task Handle(HelpCommand notification, CancellationToken cancellationToken)
        {
            _console.WriteLine("List of available commands:");
            var woaCommands = this.GetType().Assembly.GetTypes().Where(t => t.GetCustomAttribute<WoACommandAttribute>() != null);
            foreach (var woaCommand in woaCommands)
            {
                string regex = woaCommand.GetCustomAttribute<WoACommandAttribute>().RegexToMatch;
                _console.WriteLine(regex);
            }
            return Task.CompletedTask;
        }
    }
}
