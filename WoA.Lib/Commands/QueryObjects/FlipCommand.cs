﻿using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = @"flip (?<itemDesc>.*)")]
    public class FlipCommand : INotification
    {
        public string ItemDescription { get; set; }

        public FlipCommand(Match m)
        {
            ItemDescription = m.Groups["itemDesc"].Value;
        }
    }
}
