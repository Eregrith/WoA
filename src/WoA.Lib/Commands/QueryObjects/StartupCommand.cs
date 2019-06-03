using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.Commands.QueryObjects
{
    public class StartupCommand : INotification
    {
        public Version CurrentVersion { get; set; }
    }
}
