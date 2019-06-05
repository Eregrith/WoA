using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = "^see (?<itemDesc>.+)", Description = "See all auctions for given item", DisplayedInHelp = true)]
    public class SeeAuctionsCommand : INotification
    {
        public string ItemDescription { get; set; }

        public SeeAuctionsCommand(Match m)
        {
            ItemDescription = m.Groups["itemDesc"].Value;
        }
    }

    [WoACommand(RegexToMatch = "^see$")]
    public class SeeAuctionsUsageCommand : INotification
    { }
}
