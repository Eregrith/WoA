using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = @"spy (?<sellerName>.+)")]
    public class SpySellerCommand : INotification
    {
        public string SellerName { get; set; }

        public SpySellerCommand(Match match)
        {
            SellerName = match.Groups["sellerName"].Value;
        }
    }
}
