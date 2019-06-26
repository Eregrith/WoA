using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = "^toons$", Usage = "toons", Description = "Shows a summary of all your Player Toons total AH posted value", DisplayedInHelp = true)]
    public class ToonsCommand : INotification
    {
    }
}
