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
                var attr = woaCommand.GetCustomAttribute<WoACommandAttribute>();
                string regex = attr.RegexToMatch;
                string desc = attr.Description;
                _console.WriteLine(String.Format("{0,-40} : {1}", regex, desc));
            }
            return Task.CompletedTask;
        }
    }
}
