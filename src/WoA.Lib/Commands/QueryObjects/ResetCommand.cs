using MediatR;
using System;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(
        RegexToMatch = @"^reset (?:'(?<itemDesc>[^']+)'(?: (?<maxBuyPercent>[0-9]+)%? (?<sellPricePercent>[0-9]+)%?)|(?<itemDesc>.+))",
        Description = "Look at potential resetting for given item (buy all items up to <max buy>% dbmarket and sell them all at <sell price>% dbmarket)",
        Usage = "reset '<item>' <max buy%> <sell price%> | reset <item> [90%] [110%]",
        DisplayedInHelp = true
    )]
    public class ResetCommand : INotification
    {
        public string ItemDescription { get; set; }
        public int MaxBuyPercent { get; set; }
        public int SellPricePercent { get; set; }

        public ResetCommand(Match m)
        {
            ItemDescription = m.Groups["itemDesc"].Value;
            MaxBuyPercent = int.Parse(m.Groups["maxBuyPercent"].Value);
            SellPricePercent = int.Parse(m.Groups["sellPricePercent"].Value);
        }
    }

    [WoACommand(RegexToMatch = @"^reset$")]
    public class ResetUsageCommand : INotification
    { }
}
