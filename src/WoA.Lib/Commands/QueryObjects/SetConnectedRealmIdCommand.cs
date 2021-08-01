using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(Description = "Starts the wizard to set the connected realm id, used to determine which game server to query data for",
            DisplayedInHelp = true,
            RegexToMatch = "^setconnectedrealm$",
            Usage = "setconnectedrealm")]
    public class SetConnectedRealmIdCommand : INotification
    { }
}
