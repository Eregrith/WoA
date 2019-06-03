﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    [WoACommand(RegexToMatch = "bundle add (?:(?<itemQuantity>[0-9]+) )?(?<itemDesc>.+)", Description = "Adds an item to the bundle")]
    public class BundleAddCommand : INotification
    {
        public string ItemDescription { get; set; }
        public int ItemQuantity { get; set; }

        public BundleAddCommand(Match m)
        {
            ItemDescription = m.Groups["itemDesc"].Value;
            if (!String.IsNullOrEmpty(m.Groups["itemQuantity"].Value))
                ItemQuantity = int.Parse(m.Groups["itemQuantity"].Value);
            else
                ItemQuantity = 1;
        }
    }

    [WoACommand(RegexToMatch = "bundle save (?<bundleName>.*)", Description = "Save the current bundle")]
    public class BundleSaveCommand : INotification
    {
        public string BundleName { get; set; }

        public BundleSaveCommand(Match m)
        {
            BundleName = m.Groups["bundleName"].Value;
        }
    }

    [WoACommand(RegexToMatch = "bundle load (?<bundleName>.*)", Description = "Load the selected bundle")]
    public class BundleLoadCommand : INotification
    {
        public string BundleName { get; set; }

        public BundleLoadCommand(Match m)
        {
            BundleName = m.Groups["bundleName"].Value;
        }
    }

    [WoACommand(RegexToMatch = "bundle list", Description = "Shows the bundle's contents and price")]
    public class BundleListCommand : INotification
    {
    }

    [WoACommand(RegexToMatch = "bundle clear", Description = "Clears the bundle of any item")]
    public class BundleClearCommand : INotification
    {
    }

    [WoACommand(RegexToMatch = "bundle flip", Description = "Look at potential flipping for the current bundle")]
    public class BundleFlipCommand : INotification
    {
    }
}
