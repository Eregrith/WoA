using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(
        RegexToMatch = @"flip (?<itemDesc>.+)",
        Description = "Look at potential flipping for given item (buy all items up to 80% dbmarket and sell them all at 100% dbmarket)",
        DisplayedInHelp = true
    )]
    public class FlipCommand : INotification
    {
        public string ItemDescription { get; set; }

        public FlipCommand(Match m)
        {
            ItemDescription = m.Groups["itemDesc"].Value;
        }
    }
}
