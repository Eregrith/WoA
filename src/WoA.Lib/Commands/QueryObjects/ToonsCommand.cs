using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = "^toons$", Usage = "toons", Description = "Shows a summary of all your Player Toons total AH posted value", DisplayedInHelp = true)]
    public class ToonsCommand : INotification
    {
    }

    [WoACommand(RegexToMatch = "^toons add (?<toon>.+)$", Usage = "toons add <toon name>", Description = "Adds a toon to the list of toons", DisplayedInHelp = true)]
    public class ToonsAddCommand : INotification
    {
        public string ToonName { get; set; }

        public ToonsAddCommand(Match m)
        {
            ToonName = m.Groups["toon"].Value;
        }
    }

    [WoACommand(RegexToMatch = "^toons remove (?<toon>.+)$", Usage = "toons remove <toon name>", Description = "Removes the toon from the list", DisplayedInHelp = true)]
    public class ToonsRemoveCommand : INotification
    {
        public string ToonName { get; set; }

        public ToonsRemoveCommand(Match m)
        {
            ToonName = m.Groups["toon"].Value;
        }
    }

    [WoACommand(RegexToMatch = "^toons clear$", Usage = "toons clear", Description = "Empties the list of toons", DisplayedInHelp = true)]
    public class ToonsClearCommand : INotification
    {
    }
}
