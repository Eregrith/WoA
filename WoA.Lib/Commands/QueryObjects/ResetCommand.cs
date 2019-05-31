using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(
        RegexToMatch = @"reset (?<itemDesc>.*)",
        Description = "Look at potential resetting for given item (buy all items up to 90% dbmarket and sell them all at 110% dbmarket)"
    )]
    public class ResetCommand : INotification
    {
        public string ItemDescription { get; set; }

        public ResetCommand(Match m)
        {
            ItemDescription = m.Groups["itemDesc"].Value;
        }
    }
}
