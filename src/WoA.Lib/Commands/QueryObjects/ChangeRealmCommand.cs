using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = "^chrealm (?<realm>[a-z]+)", Description = "Change realm", DisplayedInHelp = true)]
    public class ChangeRealmCommand : INotification
    {
        public string Realm { get; set; }

        public ChangeRealmCommand(Match m)
        {
            Realm = m.Groups["realm"].Value;
        }
        public ChangeRealmCommand()
        { }
    }

    [WoACommand(RegexToMatch = "^chrealm$")]
    public class ChangeRealmUsageCommand : INotification
    { }
}
