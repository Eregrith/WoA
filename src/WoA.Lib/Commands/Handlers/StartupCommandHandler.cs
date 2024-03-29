﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Blizzard;
using WoA.Lib.Commands.QueryObjects;
using WoA.Lib.TSM;

namespace WoA.Lib.Commands.Handlers
{
    public class StartupCommandHandler : INotificationHandler<StartupCommand>
    {
        private readonly IStylizedConsole _console;

        public StartupCommandHandler(ITsmClient tsm, IBlizzardClient blizzard, IStylizedConsole console)
        {
            _console = console;
        }

        public Task Handle(StartupCommand notification, CancellationToken cancellationToken)
        {
            _console.WriteAscii("World of Auctions");
            _console.WriteLine($"version {notification.CurrentVersion}");
            return Task.CompletedTask;
        }
    }
}
