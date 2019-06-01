using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(
        RegexToMatch = @"reset (?:'(?<itemDesc>[^']+)'(?: (?<maxBuyPercent>[0-9]+)%? (?<sellPricePercent>[0-9]+)%?)|(?<itemDesc>.+))",
        Description = "Look at potential resetting for given item (buy all items up to 90% dbmarket and sell them all at 110% dbmarket)"
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
}
