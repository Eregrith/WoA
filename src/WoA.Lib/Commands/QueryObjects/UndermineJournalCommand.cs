using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = @"tuj (?<itemDesc>.+)", Usage = "tuj <item>", Description = "See TUJ's page for given item", DisplayedInHelp = true)]
    public class UndermineJournalCommand : INotification
    {
        public string ItemDescription { get; set; }

        public UndermineJournalCommand(Match m)
        {
            ItemDescription = m.Groups["itemDesc"].Value;
        }
    }

    [WoACommand(RegexToMatch = @"^tuj$")]
    public class UndermineJournalUsageCommand : INotification
    { }
}
