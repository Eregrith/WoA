using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = @"wowuction (?<itemDesc>.+)", Description = "See Wowuction's page for given item", DisplayedInHelp = true)]
    public class WowuctionCommand : INotification
    {
        public string ItemDescription { get; set; }

        public WowuctionCommand(Match m)
        {
            ItemDescription = m.Groups["itemDesc"].Value;
        }
    }

    [WoACommand(RegexToMatch = @"^wowuction$")]
    public class WowuctionUsageCommand : INotification
    { }
}
