using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = "^craft (?<itemDesc>.+)$")]
    public class CraftCommand : INotification
    {
        public string ItemDesc { get; set; }

        public CraftCommand(Match m)
        {
            ItemDesc = m.Groups["itemDesc"].Value;
        }
    }
}