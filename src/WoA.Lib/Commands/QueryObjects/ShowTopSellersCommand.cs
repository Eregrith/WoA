using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = "^top$", Usage = "top", Description = "See top 10 sellers (in total AH posted value) in the realm", DisplayedInHelp = true)]
    public class ShowTopSellersCommand : INotification
    {
    }
}
