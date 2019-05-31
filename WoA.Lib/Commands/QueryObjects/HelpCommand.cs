using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = "help")]
    public class HelpCommand : INotification
    {
        public HelpCommand(Match m)
        {
            //Nothing to do
        }
    }
}
