using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = "^sales export (?<realmslug>[^ ]+) (?<path>[^\"]+)$", DisplayedInHelp = true, Description = "Exports sales data for given realm in csv format to given path", Usage = "sales export <realm slug> <path>")]
    public class SalesExportCommand : INotification
    {
        public string Realm { get; set; }
        public string Path { get; set; }

        public SalesExportCommand(Match m)
        {
            Realm = m.Groups["realmslug"].Value;
            Path = m.Groups["path"].Value;
        }
    }
}
