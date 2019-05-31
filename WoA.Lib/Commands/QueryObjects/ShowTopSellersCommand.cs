using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = "top", Description = "See top 10 sellers (in total AH posted value) in the realm")]
    public class ShowTopSellersCommand : INotification
    {
        public ShowTopSellersCommand(Match m)
        {
            //nothing to do
        }
    }
}
