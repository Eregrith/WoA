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
                Exception inner = e.InnerException;
                while (inner != null)
                {
                    Console.WriteLine("InnerException: ", inner);
                    inner = inner.InnerException;
                }
                Console.ReadLine();
            }
        }

        private void CheckConfig()
        {
            bool needSave = false;
            if (String.IsNullOrEmpty(_config.CurrentRegion))
            {
                _console.WriteLine("What is your region [eu|us] ?");
                string line = Console.ReadLine();
                _config.CurrentRegion = line;
                needSave = true;
            }
            if (String.IsNullOrEmpty(_config.CurrentRealm))
            {
                _console.WriteLine("What is your realm slug (no special chars, example Drek'Thar is drekthar) ?");
                string line = Console.ReadLine();
                _config.CurrentRealm = line;
                needSave = true;
            }
            if (String.IsNullOrEmpty(_config.Blizzard_ClientId))
            {
                _console.WriteLine("What is your Blizzard Client Id?");
                string line = Console.ReadLine();
                _config.Blizzard_ClientId = line;
                needSave = true;
            }
            if (String.IsNullOrEmpty(_config.Blizzard_ClientSecret))
            {
                _console.WriteLine("What is your Blizzard Client Secret?");
                string line = Console.ReadLine();
                _config.Blizzard_ClientSecret = line;
                needSave = true;
            }
            if (String.IsNullOrEmpty(_config.TsmApiKey))
            {
                _console.WriteLine("What is your TSM ApiKey ?");
                string line = Console.ReadLine();
                _config.TsmApiKey = line;
                needSave = true;
            }

            if (needSave)
                _config.Save();
        }
    }
}
