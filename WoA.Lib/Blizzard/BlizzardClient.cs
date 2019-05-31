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
        private string oauthTokenUrl => "https://eu.battle.net/oauth/token";
        private string _realm;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public BlizzardClient(string clientId, string clientSecret, string realm)
        {
            _clientId  = clientId;
            _clientSecret = clientSecret;
            _realm = realm;
        }

        public List<Auction> GetAuctions(string fileUrl)
        {
            var client = new RestClient(fileUrl);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            return JsonConvert.DeserializeObject<AuctionFileContents>(response.Content).auctions;
        }

        public string GetAuctionFileUrl(string token)
        {
            Console.WriteLine("Getting auctions file for realm " + _realm);
            string fileUrl;
            var client = new RestClient("https://eu.api.blizzard.com/wow/auction/data/" + _realm);
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("authorization", $"Bearer {token}");
            IRestResponse response = client.Execute(request);

            var auctionApiResponse = JsonConvert.DeserializeObject<AuctionApiResponse>(response.Content);
            fileUrl = auctionApiResponse.files.First().url;
            return fileUrl;
        }

        public string GetAccessToken()
        {
            var client = new RestClient(oauthTokenUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", $"grant_type=client_credentials&client_id={_clientId}&client_secret={_clientSecret}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            var tokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(response.Content);

            return tokenResponse.access_token;
        }

        public void ChangeRealm(string realm)
        {
            _realm = realm;
        }
    }
}
