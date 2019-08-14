using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    public enum TimeFrame
    {
        PerHour,
        PerMinute
    }

    [WoACommand(RegexToMatch = "^farmables? (?:add|set) (?<quantity>[0-9.,]+)/(?<timeframe>h|min) (?<itemDesc>.+)")]
    public class FarmablesAddCommand : INotification
    {
        public string ItemDesc { get; set; }
        public double Quantity { get; set; }
        public TimeFrame TimeFrame { get; set; }

        public FarmablesAddCommand(Match m)
        {
            ItemDesc = m.Groups["itemDesc"].Value;
            Quantity = double.Parse(m.Groups["quantity"].Value);
            TimeFrame = m.Groups["timeframe"].Value == "h" ? TimeFrame.PerHour : TimeFrame.PerMinute;
        }
    }

    [WoACommand(RegexToMatch = "^farmables? list$")]
    public class FarmablesListCommand : INotification
    { }
}
