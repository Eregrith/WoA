using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = "^help$", Description = "See this list", DisplayedInHelp = true)]
    public class HelpCommand : INotification
    {
    }
}
