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
                string token = _blizzard.GetAccessToken();

                string fileUrl = _blizzard.GetAuctionFileUrl(token);

                List<Auction> auctions = _blizzard.GetAuctions(fileUrl);
                _console.WriteLine($"Got {auctions.Count} auctions from the file for realm " + _config.CurrentRealm);

                string line;
                do
                {
                    _console.WriteLine("Waiting for a command... [flip|see|spy|chrealm|stop|top|whatis]");
                    line = Console.ReadLine();
                    if (line.StartsWith("flip "))
                    {
                        int itemId = _auctionViewer.GetItemId(line);
                        if (itemId != 0)
                            _auctionViewer.SimulateFlippingItem(auctions, itemId);
                    }
                    else if (line.StartsWith("see "))
                    {
                        int itemId = _auctionViewer.GetItemId(line);
                        if (itemId != 0)
                            _auctionViewer.SeeAuctionsFor(auctions, itemId);
                    }
                    else if (line.StartsWith("spy"))
                    {
                        string owner = line.Split(' ')[1];
                        _auctionViewer.SeeAuctionsOwnedBy(auctions, owner);
                    }
                    else if (line.StartsWith("top"))
                    {
                        _auctionViewer.SeeTopSellers(auctions);
                    }
                    else if (line.StartsWith("whatis"))
                    {
                        int itemId = _auctionViewer.GetItemId(line);
                        _console.WriteLine("Opening wowhead's article on item");
                        Process.Start("https://www.wowhead.com/item=" + itemId);
                    }
                    else if (line.StartsWith("chrealm"))
                    {
                        string realm = line.Split(' ')[1];
                        _config.CurrentRealm = realm;
                        _tsm.RefreshTsmItemsInRepository();
                        fileUrl = _blizzard.GetAuctionFileUrl(token);
                        auctions = _blizzard.GetAuctions(fileUrl);
                        _console.WriteLine($"Got {auctions.Count} auctions from the file for realm " + _config.CurrentRealm);
                    }
                } while (line != "stop");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }
    }
}
