using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = @"^spy (?<sellerName>.+)", Usage = "spy <seller>", Description = "See all auctions and info for given seller", DisplayedInHelp = true)]
    public class SpySellerCommand : INotification
    {
        public string SellerName { get; set; }

        public SpySellerCommand(Match match)
        {
            SellerName = match.Groups["sellerName"].Value;
        }
    }

    [WoACommand(RegexToMatch = @"^spy$")]
    public class SpySellerUsageCommand : INotification
    {}
}
