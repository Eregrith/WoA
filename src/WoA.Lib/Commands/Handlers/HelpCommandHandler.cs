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
            _console.WriteLine();
            var woaCommands = this.GetType().Assembly.GetTypes().Where(t => t.GetCustomAttribute<WoACommandAttribute>() != null);
            foreach (var woaCommand in woaCommands)
            {
                var attr = woaCommand.GetCustomAttribute<WoACommandAttribute>();
                if (attr.DisplayedInHelp)
                {
                    string regex = attr.Usage;
                    string desc = attr.Description;
                    _console.WriteLine(String.Format("{0,-40} : {1}", regex, desc));
                }
            }
            _console.WriteLine();
            _console.WriteLine("Values between chevrons like <this> are parameters you need to specify based on the desired outcome.");
            _console.WriteLine("Values between braces like [this] are default values that will be used if you do not enter any.");
            _console.WriteLine("Pipes ( | ) show a different possiblity of writing the command.");
            _console.WriteLine("For all these commands, whenever <item> is required, you can enter either part of the name of the item or its ID");
            _console.WriteLine("Examples:");
            _console.WriteLine("    > flip akunda's");
            _console.WriteLine("    > flip Akunda's Bite");
            _console.WriteLine("    > flip 152507");
            _console.WriteLine("All these will show the result of the 'flip' command for item Akunda's Bite (152507)");
            _console.WriteLine("While this :");
            _console.WriteLine("    > flip 's bite");
            _console.WriteLine("Will ask you to chose from a list of matching items");
            return Task.CompletedTask;
        }
    }
}
