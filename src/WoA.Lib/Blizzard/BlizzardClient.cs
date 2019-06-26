using log4net;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WoA.Lib.SQLite;

namespace WoA.Lib.Blizzard
{
    public class BlizzardClient : IBlizzardClient
    {
        private readonly IConfiguration _config;
        private readonly IStylizedConsole _console;
        private readonly IGenericRepository _repo;
        private string _token;
        private long _lastFileGot;
        public List<Auction> Auctions { get; set; }

        private readonly IUserNotifier _notifier;
        private readonly ILog _logger;

        public BlizzardClient(IConfiguration config, IStylizedConsole console, IGenericRepository repo, IUserNotifier notifier, ILog logger)
        {
            _config = config;
            _console = console;
            _repo = repo;
            Auctions = _repo.GetAll<Auction>().ToList();
            _lastFileGot = _repo.GetById<BlizzardRealmData>(_config.CurrentRegion + "-" + _config.CurrentRealm)?.LastUpdate ?? 0;
            _notifier = notifier;
            _logger = logger;
        }

        public void LoadAuctions()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            _notifier.Toast($"Loading auctions for realm " + _config.CurrentRealm + " started");

            try
            {
                _token = GetAccessToken();

                AuctionApiResponse file = GetAuctionFile();
                if (file.files.First().lastModified > _lastFileGot)
                {
                    (int added, int updated, int removed) = ProcessAuctions(file.files.First().url, file.files.First().lastModified);
                    Auctions = _repo.GetAll<Auction>().ToList();
                    UpdateLastFileGotTime(file);

                    _notifier.Toast($"{added + updated + removed} auctions processed in " + (stopwatch.ElapsedMilliseconds / 1000) + " sec"
                                    + Environment.NewLine + $"{updated} updated"
                                    + Environment.NewLine + $"{added} new"
                                    + Environment.NewLine + $"{removed} removed");
                }
                else
                    _notifier.Toast($"No new auctions processed.");
            }
            catch (Exception e)
            {
                _logger.Error("Error in LoadAuctions()", e);
                _notifier.Toast("Error while loading auctions. There probably were no auctions loaded as a result");
                _console.WriteNotificationLine("BLI > Error while loading auctions. There probably were no auctions loaded as a result");
                _console.WriteNotificationLine(e.Message);
            }
            stopwatch.Stop();
        }

        private void UpdateLastFileGotTime(AuctionApiResponse file)
        {
            _lastFileGot = file.files.First().lastModified;
            BlizzardRealmData realmData = _repo.GetById<BlizzardRealmData>(_config.CurrentRegion + "-" + _config.CurrentRealm);
            if (realmData == null)
            {
                realmData = new BlizzardRealmData { Id = _config.CurrentRegion + "-" + _config.CurrentRealm, LastUpdate = _lastFileGot };
                _repo.Add(realmData);
            }
            else
            {
                realmData.LastUpdate = _lastFileGot;
                _repo.Update(realmData);
            }
        }

        private (int, int, int) ProcessAuctions(string url, long timestamp)
        {
            List<Auction> auctionsFromFile = GetAuctions(url);

            (int updated, int removed) = UpdateOrDeleteExistingAuctions(auctionsFromFile, timestamp);
            int added = InsertNewAuctions(auctionsFromFile);

            return (added, updated, removed);
        }

        private (int, int) UpdateOrDeleteExistingAuctions(List<Auction> auctionsFromFile, long timestamp)
        {
            TimeSpan timeSinceLastUpdate = new TimeSpan(timestamp - _lastFileGot);
            var savedAuctions = _repo.GetAll<Auction>();
            List<SoldAuction> probablySoldAuctions = new List<SoldAuction>();
            List<Auction> playerAuctionProbablySold = new List<Auction>();
            List<Auction> removed = new List<Auction>();
            List<Auction> updated = new List<Auction>();
            foreach (Auction savedAuction in savedAuctions)
            {
                var updatedAuction = auctionsFromFile.FirstOrDefault(a => a.auc == savedAuction.auc);
                if (updatedAuction == null)
                {
                    if (savedAuction.timeLeft.ToHoursLeft() > timeSinceLastUpdate.TotalHours)
                    {
                        probablySoldAuctions.Add(new SoldAuction(savedAuction, timeSinceLastUpdate));
                        if (_config.PlayerToons.Contains(savedAuction.owner))
                            playerAuctionProbablySold.Add(savedAuction);
                    }
                    removed.Add(savedAuction);
                }
                else
                {
		            updatedAuction.FirstSeenOn = savedAuction.FirstSeenOn;
                    updated.Add(updatedAuction);
                }
            }
            _repo.DeleteAll(removed);
            _repo.UpdateAll(updated);
            _repo.AddAll(probablySoldAuctions);

            if (playerAuctionProbablySold.Any())
            {
                _notifier.NotifySomethingNew();
                _notifier.Toast($"{playerAuctionProbablySold.Count} of your auctions probably sold for a total of {playerAuctionProbablySold.Sum(a => a.buyout).ToGoldString()}.");
                _console.WriteNotificationLine($"BLI > {playerAuctionProbablySold.Count} of your auctions probably sold (or were cancelled before timing out).");
                foreach (Auction a in playerAuctionProbablySold)
                {
                    WowItem item = GetItem(a.item);
                    WowQuality quality = (WowQuality)item.quality;
                    string name = item.name;
                    _console.WriteNotificationLine(String.Format("{0,46}{1,20} x{2,3}{3,20}{4,15}{5,4}"
                        , name.WithQuality(quality)
                        , a.PricePerItem.ToGoldString()
                        , a.quantity
                        , a.buyout.ToGoldString()
                        , a.owner
                        , a.timeLeft.ToAuctionTimeString()));
                }
            }

            return (updated.Count, removed.Count);
        }

        private int InsertNewAuctions(List<Auction> auctionsFromFile)
        {
            var savedAuctions = _repo.GetAll<Auction>();
            var newAuctions = auctionsFromFile.Where(a => !savedAuctions.Any(r => r.auc == a.auc)).ToList();
            newAuctions.ForEach(a => a.FirstSeenOn = DateTime.Now);
            _repo.AddAll(newAuctions);
            return newAuctions.Count;
        }

        private List<Auction> GetAuctions(string fileUrl)
        {
            var client = new RestClient(fileUrl);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new Exception("Auctions file retrieval failed. Check your Blizzard ClientId/ClientSecret settings.");

            return JsonConvert.DeserializeObject<AuctionFileContents>(response.Content).auctions;
        }

        private AuctionApiResponse GetAuctionFile()
        {
            string fileUrl;
            IRestResponse response = CallBlizzardAPI("https://" + _config.CurrentRegion + ".api.blizzard.com/wow/auction/data/" + _config.CurrentRealm);

            var auctionApiResponse = JsonConvert.DeserializeObject<AuctionApiResponse>(response.Content);
            return auctionApiResponse;
        }

        private IRestResponse CallBlizzardAPI(string url)
        {
            _logger.Debug("Calling POST [" + url + "] with token " + _token);

            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("authorization", $"Bearer {_token}");
            IRestResponse response = client.Execute(request);

            _logger.Debug("Got response : " + Environment.NewLine + JsonConvert.SerializeObject(response));

            if (!response.IsSuccessful)
                throw new Exception("Call to Blizzard API failed. Check your Blizzard ClientId/ClientSecret settings.");

            return response;
        }

        private string GetAccessToken()
        {
            string url = "https://" + _config.CurrentRegion + ".battle.net/oauth/token";
            _logger.Debug("Calling POST [" + url + "]");
            _logger.Debug("Client Id : " + _config.Blizzard_ClientId);
            _logger.Debug("Client Secret : " + _config.Blizzard_ClientSecret);
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", $"grant_type=client_credentials&client_id={_config.Blizzard_ClientId}&client_secret={_config.Blizzard_ClientSecret}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            _logger.Debug("Got response : " + Environment.NewLine + JsonConvert.SerializeObject(response));

            if (!response.IsSuccessful)
                throw new Exception("Could not get access token from blizzard api. Check your Blizzard ClientId/ClientSecret settings.");

            var tokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(response.Content);

            return tokenResponse.access_token;
        }

        public CharacterProfile GetInfosOnCharacter(string characterName, string realm)
        {
            IRestResponse response = CallBlizzardAPI("https://" + _config.CurrentRegion + ".api.blizzard.com/wow/character/" + realm.ToLower() + "/" + characterName);

            var statusResponse = JsonConvert.DeserializeObject<WowStatusResponse>(response.Content);
            if (statusResponse.status == "nok")
                throw new CharacterInfoException(statusResponse.reason);

            return JsonConvert.DeserializeObject<CharacterProfile>(response.Content);
        }

        public WowQuality GetQuality(int itemId)
        {
            return (WowQuality)GetItem(itemId).quality;
        }

        public WowItem GetItem(int itemId)
        {
            WowItem item = _repo.GetById<WowItem>(itemId.ToString());
            if (item == null)
            {
                item = GetItemFromAPI(itemId);
                _repo.Add(item);
            }
            return item;
        }

        private WowItem GetItemFromAPI(int itemId)
        {
            IRestResponse response = CallBlizzardAPI("https://" + _config.CurrentRegion + ".api.blizzard.com/wow/item/" + itemId);

            WowItem item = JsonConvert.DeserializeObject<WowItem>(response.Content);

            return item;
        }
    }
}
