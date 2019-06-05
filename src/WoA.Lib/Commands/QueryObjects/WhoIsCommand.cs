using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = "whois (?<seller>.+)", Description = "Shows info on given seller", DisplayedInHelp = true)]
    public class WhoIsCommand : INotification
    {
        public string SellerName { get; set; }

        public WhoIsCommand(Match m)
        {
            SellerName = m.Groups["seller"].Value;
        }
    }
}
