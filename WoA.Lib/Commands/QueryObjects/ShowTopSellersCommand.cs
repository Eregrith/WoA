﻿using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = "top")]
    public class ShowTopSellersCommand : INotification
    {
        public ShowTopSellersCommand(Match m)
        {
            //nothing to do
        }
    }
}
