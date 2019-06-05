using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WoA.Lib.Blizzard
{
    public class BlizzardClient : IBlizzardClient
    {
        private readonly IConfiguration _config;
        private readonly IStylizedConsole _console;
        private List<Auction> _auctions;
        private string _token;
        public List<Auction> Auctions { get => _auctions; }

        public BlizzardClient(IConfiguration config, IStylizedConsole console)
        {
            _config = config;
            _console = console;
        }

        public void LoadAuctions()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _console.WriteLine($"BLI > Loading auctions for realm " + _config.CurrentRealm);

            _token = GetAccessToken();

            string fileUrl = GetAuctionFileUrl();

            _auctions = GetAuctions(fileUrl);

            stopwatch.Stop();
            _console.WriteLine($"BLI > Got {_auctions.Count} auctions from the file for realm " + _config.CurrentRealm + " in " + stopwatch.ElapsedMilliseconds + " ms");
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
            IRestResponse response = CallBlizzardAPI("https://" + _config.CurrentRegion + ".api.blizzard.com/wow/item/" + itemId);

            WowItem item = JsonConvert.DeserializeObject<WowItem>(response.Content);

            return (WowQuality)item.quality;
        }
    }
}
