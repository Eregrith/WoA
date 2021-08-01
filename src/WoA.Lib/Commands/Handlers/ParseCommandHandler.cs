using log4net;
using MediatR;
using Newtonsoft.Json;
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
        private readonly ILog _logger;

        public ParseCommandHandler(IMediator mediator, IStylizedConsole console, ILog logger)
        {
            _mediator = mediator;
            _console = console;
            _logger = logger;
        }

        public Task Handle(ParseCommand notification, CancellationToken cancellationToken)
        {
            var woaCommands = this.GetType().Assembly.GetTypes().Where(t => t.GetCustomAttribute<WoACommandAttribute>() != null);
            bool matched = false;
            foreach (var woaCommand in woaCommands)
            {
                string regex = woaCommand.GetCustomAttribute<WoACommandAttribute>().RegexToMatch;
                Regex r = new Regex(regex);
                Match m = r.Match(notification.UserInput.Trim());
                if (m.Success)
                {
                    object command;
                    if (m.Groups.Count > 1)
                        command = Activator.CreateInstance(woaCommand, m);
                    else
                        command = Activator.CreateInstance(woaCommand);

                    _logger.Info($"Publishing command object {command.GetType().Name} from line:");
                    _logger.Info(notification.UserInput);
                    _logger.Debug(command.GetType().Name + Environment.NewLine + JsonConvert.SerializeObject(command, Formatting.Indented));
                    _mediator.Publish(command);
                    matched = true;
                }
            }
            if (!matched)
            {
                _console.WriteLine("No command matched your input. To see a list of commands type: help");
                _logger.Debug("User input failed to match any command");
                _logger.Debug(notification.UserInput);
            }
            return Task.CompletedTask;
        }
    }
}
