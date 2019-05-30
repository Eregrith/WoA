using MongoDB.Driver;
using MongoRepository;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using WoA.Lib;
using WoA.Lib.Blizzard;
using WoA.Lib.TSM;

namespace WorldOfAuctions
{
    class Program
    {
        private static IStylizedConsole _console;
        private static MongoUrl mongoUrl = new MongoUrlBuilder { DatabaseName = "WoA", Server = new MongoServerAddress("localhost"), Username = "admin", Password = "admin"}.ToMongoUrl();
        private static BlizzardClient _blizzard;
        private static AuctionViewer _auctionViewer;
        private static string _realm;

        static void Main(string[] args)
        {
            try
            {
                string tsmApiKey = ConfigurationManager.AppSettings.Get("TSM_ApiKey");
                if (String.IsNullOrEmpty(tsmApiKey))
                    throw new InvalidOperationException("Please provide a TSM_ApiKey in the configuration");
                _console = new StylizedConsole();
                TsmClient tsm = new TsmClient(tsmApiKey, new MongoRepository<TsmItem>(mongoUrl), new MongoRepository<TsmRealmData>(mongoUrl));
                
                Console.WriteLine("Realm ?");
                _realm = Console.ReadLine();
                _blizzard = new BlizzardClient(ConfigurationManager.AppSettings["Blizzard_ClientId"], ConfigurationManager.AppSettings["Blizzard_ClientSecret"], _realm);
                _auctionViewer = new AuctionViewer(_console, _realm);

                tsm.RefreshTsmItemsInRepository(_realm);
                string token = _blizzard.GetAccessToken();

                string fileUrl = _blizzard.GetAuctionFileUrl(token);

                List<Auction> auctions = _blizzard.GetAuctions(fileUrl);
                _console.WriteLine($"Got {auctions.Count} auctions from the file for realm " + _realm);

                string line;
                do
                {
                    _console.WriteLine("Waiting for a command... [flip|see|spy|chrealm|stop|top|whatis]");
                    line = Console.ReadLine();
                    if (line.StartsWith("flip "))
                    {
                        int itemId = _auctionViewer.GetItemId(tsm, line);
                        if (itemId != 0)
                            _auctionViewer.SimulateFlippingItem(tsm, auctions, itemId);
                    }
                    else if (line.StartsWith("see "))
                    {
                        int itemId = _auctionViewer.GetItemId(tsm, line);
                        if (itemId != 0)
                            _auctionViewer.SeeAuctionsFor(tsm, auctions, itemId);
                    }
                    else if (line.StartsWith("spy"))
                    {
                        string owner = line.Split(' ')[1];
                        _auctionViewer.SeeAuctionsOwnedBy(tsm, auctions, owner);
                    }
                    else if (line.StartsWith("top"))
                    {
                        _auctionViewer.SeeTopSellers(tsm, auctions);
                    }
                    else if (line.StartsWith("whatis"))
                    {
                        int itemId = _auctionViewer.GetItemId(tsm, line);
                        _console.WriteLine("Opening wowhead's article on item");
                        Process.Start("https://www.wowhead.com/item=" + itemId);
                    }
                    else if (line.StartsWith("chrealm"))
                    {
                        string realm = line.Split(' ')[1];
                        _realm = realm;
                        _blizzard.ChangeRealm(_realm);
                        _auctionViewer.ChangeRealm(_realm);
                        tsm.RefreshTsmItemsInRepository(realm);
                        fileUrl = _blizzard.GetAuctionFileUrl(token);
                        auctions = _blizzard.GetAuctions(fileUrl);
                        _console.WriteLine($"Got {auctions.Count} auctions from the file for realm " + _realm);
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
