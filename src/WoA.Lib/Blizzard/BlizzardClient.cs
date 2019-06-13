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

        public BlizzardClient(IConfiguration config, IStylizedConsole console, IGenericRepository repo, IUserNotifier notifier)
        {
            _config = config;
            _console = console;
            _repo = repo;
            _lastFileGot = 0;
            Auctions = _repo.GetAll<Auction>().ToList();
            _notifier = notifier;
        }

        public void LoadAuctions()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _console.WriteNotificationLine($"BLI > Loading auctions for realm " + _config.CurrentRealm + " started.");

            _token = GetAccessToken();

            AuctionApiResponse file = GetAuctionFile();
            if (file.files.First().lastModified > _lastFileGot)
            {
                ProcessAuctions(file.files.First().url, file.files.First().lastModified);
                Auctions = _repo.GetAll<Auction>().ToList();
                _lastFileGot = file.files.First().lastModified;
                _console.WriteNotificationLine($"BLI > Got {Auctions.Count} auctions from the file for realm " + _config.CurrentRealm + " in " + stopwatch.ElapsedMilliseconds + "ms");
            }
            else
                _console.WriteNotificationLine($"BLI > No new data fetched");

            stopwatch.Stop();
        }

        private void ProcessAuctions(string url, long timestamp)
        {
            List<Auction> auctionsFromFile = GetAuctions(url);

            UpdateOrDeleteExistingAuctions(auctionsFromFile, timestamp);
            InsertNewAuctions(auctionsFromFile);
        }

        private void UpdateOrDeleteExistingAuctions(List<Auction> auctionsFromFile, long timestamp)
        {
            var savedAuctions = _repo.GetAll<Auction>();
            int updated = 0;
            int removed = 0;
            List<Auction> playerAuctionProbablySold = new List<Auction>();
            foreach (Auction savedAuction in savedAuctions)
            {
                var updatedAuction = auctionsFromFile.FirstOrDefault(a => a.auc == savedAuction.auc);
                if (updatedAuction == null)
                {
                    if (_config.PlayerToons.Contains(savedAuction.owner)
                        && savedAuction.timeLeft.ToHoursLeft() > new TimeSpan(timestamp - _lastFileGot).TotalHours)
                        playerAuctionProbablySold.Add(savedAuction);
                    _repo.Delete(savedAuction);
                    removed++;
                }
                else
                {
                    _repo.Update(updatedAuction);
                    updated++;
                }
            }
            if (updated > 0)
                _console.WriteNotificationLine($"BLI > {updated} auctions updated.");
            if (removed > 0)
                _console.WriteNotificationLine($"BLI > {removed} auctions removed.");
            if (playerAuctionProbablySold.Any())
            {
                _notifier.NotifySomethingNew();
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
        }

        private void InsertNewAuctions(List<Auction> auctionsFromFile)
        {
            var savedAuctions = _repo.GetAll<Auction>();
            var newAuctions = auctionsFromFile.Where(a => !savedAuctions.Any(r => r.auc == a.auc)).ToList();
            newAuctions.ForEach(a => a.FirstSeenOn = DateTime.Now);
            _repo.AddAll(newAuctions);
            if (newAuctions.Count > 0)
                _console.WriteNotificationLine($"BLI > {newAuctions.Count} new auctions.");
        }

        private List<Auction> GetAuctions(string fileUrl)
        {
            var client = new RestClient(fileUrl);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

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
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("authorization", $"Bearer {_token}");
            IRestResponse response = client.Execute(request);
            return response;
        }

        private string GetAccessToken()
        {
            var client = new RestClient("https://" + _config.CurrentRegion + ".battle.net/oauth/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", $"grant_type=client_credentials&client_id={_config.Blizzard_ClientId}&client_secret={_config.Blizzard_ClientSecret}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

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
