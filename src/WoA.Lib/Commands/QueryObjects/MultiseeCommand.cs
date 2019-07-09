using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(
        RegexToMatch = "^multisee (?<max>[0-9]+) (?<itemName>.+)$",
        Usage = "multisee <amount> <item>",
        DisplayedInHelp = true,
        Description = "Shows up to <amount> of items with name matching <item>")]
    public class MultiseeCommand : INotification
    {
        public string PartialItemName { get; set; }
        public int Amount { get; set; }

        public MultiseeCommand(Match m)
        {
            Amount = int.Parse(m.Groups["max"].Value);
            PartialItemName = m.Groups["itemName"].Value;
        }
    }
}
