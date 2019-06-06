using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;

namespace WoA.Lib.Commands.Handlers
{
    public class UsageCommandsHandler
        : INotificationHandler<ChangeRealmUsageCommand>
        , INotificationHandler<BundleUsageCommand>
        , INotificationHandler<FlipUsageCommand>
        , INotificationHandler<ResetUsageCommand>
        , INotificationHandler<SeeAuctionsUsageCommand>
        , INotificationHandler<SpySellerUsageCommand>
        , INotificationHandler<TsmInfoUsageCommand>
        , INotificationHandler<UndermineJournalUsageCommand>
        , INotificationHandler<WhatIsItemUsageCommand>
        , INotificationHandler<WhoIsUsageCommand>
    {
        private readonly IStylizedConsole _console;

        public UsageCommandsHandler(IStylizedConsole console)
        {
            _console = console;
        }

        public Task Handle(ChangeRealmUsageCommand notification, CancellationToken cancellationToken)
        {
            _console.WriteAscii("chrealm");
            _console.WriteLine();
            _console.WriteLine("Usage:");
            _console.WriteLine();
            _console.WriteLine(String.Format("{0,-30} : {1}", "chrealm <realm name>", "Changes the current realm to <realm name>."));
            _console.WriteLine(String.Format("{0,-30}   {1}", "", "If the given realm is the same as the current one, auction data is refreshed"));
            return Task.CompletedTask;
        }

        public Task Handle(BundleUsageCommand notification, CancellationToken cancellationToken)
        {
            _console.WriteAscii("bundle");
            _console.WriteLine();
            _console.WriteLine("Usage:");
            _console.WriteLine();
            _console.WriteLine(String.Format("{0,-40}", "bundle add <item description>"));
            _console.WriteLine(String.Format("{0,-40} : {1}", "bundle add <quantity> <item description>", "Adds <quantity> (default 1) of given item to the bundle"));
            _console.WriteLine(String.Format("{0,-40}", "bundle remove <item description>"));
            _console.WriteLine(String.Format("{0,-40} : {1}", "bundle remove <quantity> <item description>", "Remove item or <quantity> (default all) of given item to the bundle"));
            _console.WriteLine();
            _console.WriteLine(String.Format("{0,-40} : {1}", "bundle flip", "Shows a summary of potential flips for items in the bundle"));
            _console.WriteLine(String.Format("{0,-40} : {1}", "bundle buy", "Shows a summary of total buying price for items in the bundle"));
            _console.WriteLine();
            _console.WriteLine(String.Format("{0,-40} : {1}", "bundle list", "Lists all items in the current bundle and their tsm prices"));
            _console.WriteLine(String.Format("{0,-40} : {1}", "bundle clear", "Empties the current bundle of all items"));
            _console.WriteLine();
            _console.WriteLine(String.Format("{0,-40} : {1}", "bundle save <bundle name>", "Saves the bundle as <bundle name>"));
            _console.WriteLine(String.Format("{0,-40} : {1}", "bundle load <bundle name>", "Loads the bundle of given name"));
            _console.WriteLine(String.Format("{0,-40} : {1}", "bundle delete save <bundle name>", "Delete the bundle of given name"));
            _console.WriteLine(String.Format("{0,-40} : {1}", "bundle show saved", "Shows saved bundles"));
            _console.WriteLine();
            _console.WriteLine(String.Format("{0,-40} : {1}", "bundle export tsm", "Copies to clipboard the TSM import string for current bundle"));
            _console.WriteLine(String.Format("{0,-40} : {1}", "bundle import tsm", "Replaces current bundle from given TSM import string"));
            return Task.CompletedTask;
        }

        public Task Handle(FlipUsageCommand notification, CancellationToken cancellationToken)
        {
            _console.WriteAscii("flip");
            _console.WriteLine();
            _console.WriteLine("Usage:");
            _console.WriteLine();
            _console.WriteLine(String.Format("{0,-30} : {1}", "flip <item description>", "Shows data for flipping given item."));
            _console.WriteLine(String.Format("{0,-30}   {1}", "", "Auctions are looked at for items sold at or under 80% dbmarket."));
            _console.WriteLine(String.Format("{0,-30}   {1}", "", "Potential profit is calculated from selling these items at 100% dbmarket."));
            return Task.CompletedTask;
        }

        public Task Handle(ResetUsageCommand notification, CancellationToken cancellationToken)
        {
            _console.WriteAscii("reset");
            _console.WriteLine();
            _console.WriteLine("Usage:");
            _console.WriteLine();
            _console.WriteLine(String.Format("{0,-30}", "reset '<item description>' <max buy percent>[%]  <sell price percent>[%]"));
            _console.WriteLine(String.Format("{0,-30} : {1}", "reset <item description>", "Shows data for resetting the market on given item."));
            _console.WriteLine(String.Format("{0,-30}   {1}", "", "Auctions are looked at for items sold at or under <max buy percent>% dbmarket. (defaults to 90%)"));
            _console.WriteLine(String.Format("{0,-30}   {1}", "", "Potential profit is calculated from selling these items at <sell price percent>% dbmarket. (defaults to 110%)"));
            return Task.CompletedTask;
        }

        public Task Handle(SeeAuctionsUsageCommand notification, CancellationToken cancellationToken)
        {
            _console.WriteAscii("see");
            _console.WriteLine();
            _console.WriteLine("Usage:");
            _console.WriteLine();
            _console.WriteLine(String.Format("{0,-30} : {1}", "see <item description>", "Shows auctions for given item."));
            return Task.CompletedTask;
        }

        public Task Handle(SpySellerUsageCommand notification, CancellationToken cancellationToken)
        {
            _console.WriteAscii("spy");
            _console.WriteLine();
            _console.WriteLine("Usage:");
            _console.WriteLine();
            _console.WriteLine(String.Format("{0,-30} : {1}", "spy <seller name>", "Shows all auctions of a given seller."));
            return Task.CompletedTask;
        }

        public Task Handle(TsmInfoUsageCommand notification, CancellationToken cancellationToken)
        {
            _console.WriteAscii("tsm");
            _console.WriteLine();
            _console.WriteLine("Usage:");
            _console.WriteLine();
            _console.WriteLine(String.Format("{0,-30} : {1}", "tsm <iem description>", "Shows TSM info on given item."));
            return Task.CompletedTask;
        }

        public Task Handle(UndermineJournalUsageCommand notification, CancellationToken cancellationToken)
        {
            _console.WriteAscii("tuj");
            _console.WriteLine();
            _console.WriteLine("Usage:");
            _console.WriteLine();
            _console.WriteLine(String.Format("{0,-30} : {1}", "tuj <iem description>", "Opens The Undermine Journal page for given item."));
            return Task.CompletedTask;
        }

        public Task Handle(WhatIsItemUsageCommand notification, CancellationToken cancellationToken)
        {
            _console.WriteAscii("whatis");
            _console.WriteLine();
            _console.WriteLine("Usage:");
            _console.WriteLine();
            _console.WriteLine(String.Format("{0,-30}", "what is <item description>"));
            _console.WriteLine(String.Format("{0,-30} : {1}", "whatis <iem description>", "Opens wowhead's page for given item."));
            return Task.CompletedTask;
        }

        public Task Handle(WhoIsUsageCommand notification, CancellationToken cancellationToken)
        {
            _console.WriteAscii("whois");
            _console.WriteLine();
            _console.WriteLine("Usage:");
            _console.WriteLine();
            _console.WriteLine(String.Format("{0,-30}", "who is <seller name>"));
            _console.WriteLine(String.Format("{0,-30} : {1}", "whois <seller name>", "Shows info on given seller's character."));
            return Task.CompletedTask;
        }
    }
}
