using MongoDB.Driver;
using MongoRepository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoA.Lib;
using WoA.Lib.Blizzard;
using WoA.Lib.TSM;

namespace WorldOfAuctions
{
    public class WoA
    {
        private readonly IStylizedConsole _console;
        private readonly IBlizzardClient _blizzard;
        private readonly ITsmClient _tsm;
        private readonly IConfiguration _config;
        private readonly IAuctionViewer _auctionViewer;

        public WoA(IConfiguration config, ITsmClient tsm, IBlizzardClient blizzard, IStylizedConsole console, IAuctionViewer auctionViewer)
        {
            _config = config;
            _tsm = tsm;
            _blizzard = blizzard;
            _console = console;
            _auctionViewer = auctionViewer;
        }

        public void Run()
        {
            try
            {
                _tsm.RefreshTsmItemsInRepository();
                _blizzard.LoadAuctions();

                string line;
                do
                {
                    _console.WriteLine("Waiting for a command... [flip|see|spy|chrealm|top|whatis|stop]");
                    line = Console.ReadLine();
                    if (line.StartsWith("flip "))
                    {
                        Flip(line);
                    }
                    else if (line.StartsWith("see "))
                    {
                        See(line);
                    }
                    else if (line.StartsWith("spy"))
                    {
                        Spy(line);
                    }
                    else if (line.StartsWith("top"))
                    {
                        Top();
                    }
                    else if (line.StartsWith("whatis"))
                    {
                        WhatIs(line);
                    }
                    else if (line.StartsWith("chrealm"))
                    {
                        ChangeRealm(line);
                    }
                } while (line != "stop");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }

        private void ChangeRealm(string line)
        {
            string realm = line.Split(' ')[1];
            _config.CurrentRealm = realm;
            _tsm.RefreshTsmItemsInRepository();
            _blizzard.LoadAuctions();
        }

        private void WhatIs(string line)
        {
            int itemId = _auctionViewer.GetItemId(line);
            _console.WriteLine("Opening wowhead's article on item");
            Process.Start("https://www.wowhead.com/item=" + itemId);
        }

        private void Top()
        {
            _auctionViewer.SeeTopSellers();
        }

        private void Spy(string line)
        {
            string owner = line.Split(' ')[1];
            _auctionViewer.SeeAuctionsOwnedBy(owner);
        }

        private void See(string line)
        {
            int itemId = _auctionViewer.GetItemId(line);
            if (itemId != 0)
                _auctionViewer.SeeAuctionsFor(itemId);
        }

        private void Flip(string line)
        {
            int itemId = _auctionViewer.GetItemId(line);
            if (itemId != 0)
                _auctionViewer.SimulateFlippingItem(itemId);
        }
    }
}
