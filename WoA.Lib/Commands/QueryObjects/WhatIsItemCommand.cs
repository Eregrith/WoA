using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = @"what ?is (?<itemDesc>.*)")]
    public class WhatIsItemCommand : INotification
    {
        public string ItemDescription { get; set; }

        public WhatIsItemCommand(Match m)
        {
            ItemDescription = m.Groups["itemDesc"].Value;
        }
    }
}
