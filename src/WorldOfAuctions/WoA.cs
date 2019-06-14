using MediatR;
using System;
using System.Reflection;
using WoA.Lib;
using WoA.Lib.Commands.QueryObjects;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace WorldOfAuctions
{
    public class WoA
    {
        private readonly IStylizedConsole _console;
        private readonly IMediator _mediator;
        private readonly IUserNotifier _notifier;
        private readonly IConfiguration _config;

        public WoA(IStylizedConsole console, IMediator mediator, IUserNotifier notifier, IConfiguration config)
        {
            _console = console;
            _mediator = mediator;
            _notifier = notifier;
            _config = config;
        }

        public void Run()
        {
            try
            {
                CheckConfig();
                _mediator.Publish(new StartupCommand { CurrentVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion });
                _mediator.Publish(new StartPeriodicRefreshOfAuctionsCommand());

                _console.WriteLine("Type 'help' for a list of commands");
                string line;
                do
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    _console.FlushNotificationLines();
                    _console.Write("> ", System.Drawing.Color.White);
                    line = Console.ReadLine();
                    _notifier.ClearNotifications();
                    if (!String.IsNullOrEmpty(line) && line != "exit")
                        _mediator.Publish(new ParseCommand { UserInput = line });
                } while (line != "exit");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private void CheckConfig()
        {
            List<string> configProblems = new List<string>();
            if (String.IsNullOrEmpty(_config.CurrentRegion))
                configProblems.Add("Missing region");
            if (String.IsNullOrEmpty(_config.CurrentRealm))
                configProblems.Add("Missing realm");
            if (String.IsNullOrEmpty(_config.Blizzard_ClientId)
                || String.IsNullOrEmpty(_config.Blizzard_ClientSecret))
                configProblems.Add("Blizzard ClientId/ClientSecret are not defined properly");
            if (String.IsNullOrEmpty(_config.TsmApiKey))
                configProblems.Add("TSM ApiKey is not defined properly");

            if (configProblems.Any())
                throw new ConfigurationValidationException(configProblems);
        }
    }
}
