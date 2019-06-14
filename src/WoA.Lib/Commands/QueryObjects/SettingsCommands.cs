using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = "^settings$", DisplayedInHelp = true, Description = "See and manage settings")]
    public class SettingsUsageCommand : INotification
    { }

    [WoACommand(RegexToMatch = "^settings list$")]
    public class ShowSettingsListCommand : INotification
    { }

    [WoACommand(RegexToMatch = @"^settings set (?<name>[^ ]+) (?<value>.+)$")]
    public class SettingsSetCommand : INotification
    {
        public string SettingName { get; set; }
        public string SettingValue { get; set; }

        public SettingsSetCommand(Match m)
        {
            SettingName = m.Groups["name"].Value;
            SettingValue = m.Groups["value"].Value;
        }
    }
}
