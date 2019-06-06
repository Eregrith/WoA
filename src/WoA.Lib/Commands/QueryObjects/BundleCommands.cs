using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{

    [WoACommand(RegexToMatch = "^bundle$", Description = "Handles bundles of items", DisplayedInHelp = true)]
    public class BundleUsageCommand : INotification
    {
    }

    [WoACommand(RegexToMatch = "^bundle add (?:(?<itemQuantity>[0-9]+) )?(?<itemDesc>.+)")]
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

    [WoACommand(RegexToMatch = "^bundle remove (?:(?<itemQuantity>[0-9]+) )?(?<itemDesc>.+)")]
    public class BundleRemoveCommand : INotification
    {
        public string ItemDescription { get; set; }
        public int ItemQuantity { get; set; }
        public bool RemoveAllQuantity { get; set; }

        public BundleRemoveCommand(Match m)
        {
            ItemDescription = m.Groups["itemDesc"].Value;
            if (!String.IsNullOrEmpty(m.Groups["itemQuantity"].Value))
            {
                ItemQuantity = int.Parse(m.Groups["itemQuantity"].Value);
                RemoveAllQuantity = false;
            }
            else
            {
                ItemQuantity = 0;
                RemoveAllQuantity = true;
            }
        }
    }

    [WoACommand(RegexToMatch = "^bundle save (?<bundleName>.+)", Description = "Save the current bundle")]
    public class BundleSaveCommand : INotification
    {
        public string BundleName { get; set; }

        public BundleSaveCommand(Match m)
        {
            BundleName = m.Groups["bundleName"].Value;
        }
    }

    [WoACommand(RegexToMatch = "^bundle load (?<bundleName>.+)", Description = "Load the selected bundle")]
    public class BundleLoadCommand : INotification
    {
        public string BundleName { get; set; }

        public BundleLoadCommand(Match m)
        {
            BundleName = m.Groups["bundleName"].Value;
        }
    }

    [WoACommand(RegexToMatch = "^bundle delete save (?<bundleName>.+)", Description = "Delete the selected bundle")]
    public class BundleDeleteSaveCommand : INotification
    {
        public string BundleName { get; set; }

        public BundleDeleteSaveCommand(Match m)
        {
            BundleName = m.Groups["bundleName"].Value;
        }
    }

    [WoACommand(RegexToMatch = "^bundle list", Description = "Shows the bundle's contents and price")]
    public class BundleListCommand : INotification
    {
    }

    [WoACommand(RegexToMatch = "^bundle show saved", Description = "Shows saved bundles")]
    public class BundleShowSavedCommand : INotification
    {
    }

    [WoACommand(RegexToMatch = "^bundle clear", Description = "Clears the bundle of any item")]
    public class BundleClearCommand : INotification
    {
    }

    [WoACommand(RegexToMatch = "^bundle flip", Description = "Look at potential flipping for the current bundle")]
    public class BundleFlipCommand : INotification
    {
    }

    [WoACommand(RegexToMatch = "^bundle export tsm", Description = "Copies to clipboard the TSM import string for current bundle")]
    public class BundleExportCommand : INotification
    {
    }

    [WoACommand(RegexToMatch = "^bundle import tsm", Description = "Replaces current bundle by importing from a TSM group export string")]
    public class BundleImportCommand : INotification
    {
    }

    [WoACommand(RegexToMatch = "^bundle buy (?<percentMax>[0-9]+)", Description = "Buy as many items as needed whose under chosen % of market price")]
    public class BundleBuyCommand : INotification
    {
        public int PercentMax { get; set; }

        public BundleBuyCommand(Match m)
        {
            if (!String.IsNullOrEmpty(m.Groups["percentMax"].Value))
                PercentMax = int.Parse(m.Groups["percentMax"].Value);
            else
                PercentMax = 100;
        }
    }
}
