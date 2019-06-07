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
        public List<Auction> Auctions { get; private set; }

        public BlizzardClient(IConfiguration config, IStylizedConsole console, IGenericRepository repo)
        {
            _config = config;
            _console = console;
            _repo = repo;
        }

        public void LoadAuctions()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _console.WriteNotificationLine($"BLI > Loading auctions for realm " + _config.CurrentRealm + " started.");

            _token = GetAccessToken();

            string fileUrl = GetAuctionFileUrl();

            Auctions = GetAuctions(fileUrl);

            stopwatch.Stop();
            _console.WriteNotificationLine($"BLI > Got {Auctions.Count} auctions from the file for realm " + _config.CurrentRealm + " in " + stopwatch.ElapsedMilliseconds + "ms");
        }

        private List<Auction> GetAuctions(string fileUrl)
        {
            var client = new RestClient(fileUrl);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            return JsonConvert.DeserializeObject<AuctionFileContents>(response.Content).auctions;
        }

        private string GetAuctionFileUrl()
        {
            string fileUrl;
            IRestResponse response = CallBlizzardAPI("https://" + _config.CurrentRegion + ".api.blizzard.com/wow/auction/data/" + _config.CurrentRealm);

            var auctionApiResponse = JsonConvert.DeserializeObject<AuctionApiResponse>(response.Content);
            fileUrl = auctionApiResponse.files.First().url;
            return fileUrl;
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
