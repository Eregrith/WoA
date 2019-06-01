using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = "tsm (?<itemDesc>.+)", Description = "Shows TSM info on given item")]
    public class TsmInfoCommand : INotification
    {
        public string ItemDescription { get; set; }

        public TsmInfoCommand(Match m)
        {
            ItemDescription = m.Groups["itemDesc"].Value;
        }
    }
}
