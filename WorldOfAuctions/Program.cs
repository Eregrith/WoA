using MongoRepository;
using System;
using WoA.Lib;
using WoA.Lib.Blizzard;
using WoA.Lib.TSM;

namespace WorldOfAuctions
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new Configuration();
            IStylizedConsole console = new StylizedConsole();
            string tsmApiKey = config.TsmApiKey;
            if (String.IsNullOrEmpty(tsmApiKey))
                throw new InvalidOperationException("Please provide a TSM_ApiKey in the configuration");
            ITsmClient tsm = new TsmClient(tsmApiKey, new MongoRepository<TsmItem>(config.MongoUrl), new MongoRepository<TsmRealmData>(config.MongoUrl));
            IBlizzardClient blizzard = new BlizzardClient(config.Blizzard_ClientId, config.Blizzard_ClientSecret, config.CurrentRealm);
            var auctionViewer = new AuctionViewer(console, config.CurrentRealm);

            WoA woa = new WoA(config, tsm, blizzard, console, auctionViewer);
            woa.Run();
        }
    }
}