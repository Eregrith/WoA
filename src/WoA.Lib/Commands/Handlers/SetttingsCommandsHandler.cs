using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;

namespace WoA.Lib.Commands.Handlers
{
    public class SetttingsCommandsHandler
        : INotificationHandler<ShowSettingsListCommand>
        , INotificationHandler<SettingsSetCommand>
    {
        private readonly IConfiguration _config;
        private readonly IStylizedConsole _console;
        private readonly IMediator _mediator;

        public SetttingsCommandsHandler(IConfiguration config, IStylizedConsole console, IMediator mediator)
        {
            _config = config;
            _console = console;
            _mediator = mediator;
        }

        public Task Handle(ShowSettingsListCommand notification, CancellationToken cancellationToken)
        {
            _console.WriteLine("Current settings :");
            _console.WriteLine();
            _console.WriteLine("CurrentRealm : " + _config.CurrentRealm);
            _console.WriteLine();
            _console.WriteLine("Blizzard_ClientId : " + _config.Blizzard_ClientId);
            _console.WriteLine("Blizzard_ClientSecret : " + _config.Blizzard_ClientSecret);
            _console.WriteLine();
            _console.WriteLine("TsmApiKey : " + _config.TsmApiKey);
            return Task.CompletedTask;
        }

        public async Task Handle(SettingsSetCommand notification, CancellationToken cancellationToken)
        {
            switch (notification.SettingName)
            {
                case "CurrentRealm":
                    ChangeRealmCommand command = new ChangeRealmCommand { Realm = notification.SettingValue };
                    await _mediator.Publish(command);
                    _config.Save();
                    break;
                case "Blizzard_ClientId":
                    _config.Blizzard_ClientId = notification.SettingValue;
                    _config.Save();
                    break;
                case "Blizzard_ClientSecret":
                    _config.Blizzard_ClientSecret = notification.SettingValue;
                    _config.Save();
                    break;
                case "TsmApiKey":
                    _config.TsmApiKey = notification.SettingValue;
                    _config.Save();
                    break;
                default:
                    _console.WriteLine("No setting is named " + notification.SettingName);
                    break;
            }
        }
    }
}
