using MediatR;
using System;
using System.Reflection;
using WoA.Lib;
using WoA.Lib.Commands.QueryObjects;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using WoA.Lib.Business;

namespace WorldOfAuctions
{
    public class WoA
    {
        private readonly IStylizedConsole _console;
        private readonly IMediator _mediator;
        private readonly IUserNotifier _notifier;
        private readonly IConfiguration _config;
        private readonly IApplicationStateManager _state;

        public WoA(IStylizedConsole console, IMediator mediator, IUserNotifier notifier, IConfiguration config, IApplicationStateManager state)
        {
            _console = console;
            _mediator = mediator;
            _notifier = notifier;
            _config = config;
            _state = state;
        }

        public void Run()
        {
            try
            {
                CheckConfigAndAskUserToCompleteIt();
                PublishStartupCommands();
                DisplayHelpMessage();
                MainLoop();
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

        private void MainLoop()
        {
            string line;
            do
            {
                line = ReadAndProcessLine();
            } while (line != "exit");
        }

        private void DisplayHelpMessage()
        {
            _console.WriteLine("Type 'help' for a list of commands");
        }

        private void PublishStartupCommands()
        {
            _mediator.Publish(new StartupCommand { CurrentVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion });
            _mediator.Publish(new StartPeriodicRefreshOfAuctionsCommand());
        }

        private string ReadAndProcessLine()
        {
            PrepareDisplay();
            DisplayPrompt();
            string line = Console.ReadLine();
            StopWindowFlashing();
            ProcessCommandFromLine(line);
            return line;
        }

        private void ProcessCommandFromLine(string line)
        {
            if (!String.IsNullOrEmpty(line) && line != "exit")
                _mediator.Publish(new ParseCommand { UserInput = line });
        }

        private void StopWindowFlashing() => _notifier.ClearNotifications();

        private void DisplayPrompt()
        {
            string prompt = "> ";
            if (_state.CurrentState != ApplicationState.Neutral)
                prompt = _state.StateInfo.PromptModifier(prompt);
            _console.Write(prompt, System.Drawing.Color.White);
        }

        private void PrepareDisplay()
        {
            Console.ForegroundColor = ConsoleColor.White;
            _console.FlushNotificationLines();
        }

        private void CheckConfigAndAskUserToCompleteIt()
        {
            bool needSave = false;
            if (String.IsNullOrEmpty(_config.CurrentRegion))
            {
                _console.WriteLine("What is your region [eu|us] ?");
                string line = Console.ReadLine();
                _config.CurrentRegion = line;
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
            if (_config.ConnectedRealmId == null)
            {
                _mediator.Publish(new SetConnectedRealmIdCommand());
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
