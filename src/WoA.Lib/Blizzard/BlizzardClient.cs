using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WoA.Lib.Blizzard
{
    public class BlizzardClient : IBlizzardClient
    {
        private readonly IConfiguration _config;
        private readonly IStylizedConsole _console;
        private List<Auction> _auctions;
        public List<Auction> Auctions { get => _auctions; }

        public BlizzardClient(IConfiguration config, IStylizedConsole console)
        {
            _config = config;
            _console = console;
        }

        public void LoadAuctions()
        {
            _console.WriteLine($"BLI > Loading auctions for realm " + _config.CurrentRealm);

            string token = GetAccessToken();

            string fileUrl = GetAuctionFileUrl(token);

            _auctions = GetAuctions(fileUrl);

            _console.WriteLine($"BLI > Got {_auctions.Count} auctions from the file for realm " + _config.CurrentRealm);
        }

        private List<Auction> GetAuctions(string fileUrl)
        {
            var client = new RestClient(fileUrl);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            return JsonConvert.DeserializeObject<AuctionFileContents>(response.Content).auctions;
        }

        private string GetAuctionFileUrl(string token)
        {
            string fileUrl;
            var client = new RestClient("https://" + _config.CurrentRegion + ".api.blizzard.com/wow/auction/data/" + _config.CurrentRealm);
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("authorization", $"Bearer {token}");
            IRestResponse response = client.Execute(request);

            var auctionApiResponse = JsonConvert.DeserializeObject<AuctionApiResponse>(response.Content);
            fileUrl = auctionApiResponse.files.First().url;
            return fileUrl;
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
    }
}
