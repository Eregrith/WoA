using log4net;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WoA.Lib.Persistence;

namespace WoA.Lib.Blizzard
{
    public class BlizzardClient : IBlizzardClient
    {
        private readonly IConfiguration _config;
        private readonly IStylizedConsole _console;
        private readonly IGenericRepository _repo;
        private string _token;
        private long _lastUpdate;
        private List<Auction> _auctions = null;
        public List<Auction> Auctions
        {
            get
            {
                if (_auctions == null)
                    _auctions = _repo.GetAll<Auction>().ToList();
                return _auctions;
            }
            set
            {
                _auctions = value;
            }
        }

        private readonly IUserNotifier _notifier;
        private readonly ILog _logger;

        public BlizzardClient(IConfiguration config, IStylizedConsole console, IGenericRepository repo, IUserNotifier notifier, ILog logger)
        {
            _config = config;
            _console = console;
            _repo = repo;
            _lastUpdate = 0;
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
                AuctionApiResponse auctionsResponse = GetAuctions();
                (int added, int updated, int removed) = ProcessAuctions(auctionsResponse.auctions, DateTime.Now.Ticks);
                Auctions = _repo.GetAll<Auction>().ToList();

                _notifier.Toast($"{added + updated + removed} auctions processed in " + (stopwatch.ElapsedMilliseconds / 1000) + " sec"
                                + Environment.NewLine + $"{updated} updated"
                                + Environment.NewLine + $"{added} new"
                                + Environment.NewLine + $"{removed} removed");
            }
            catch (BlizzardApiException e)
            {
                _logger.Error("BlizzardApiException in LoadAuctions()", e);
                _logger.Error("Response content : " + e.Response.Content);
                _notifier.Toast("Error while loading auctions. There probably were no auctions loaded as a result");
                _console.WriteNotificationLine("BLI > Error while loading auctions. There probably were no auctions loaded as a result");
                _console.WriteNotificationLine(e.Message);
                _console.WriteNotificationLine("Error Message: " + e.Response.ErrorMessage);
            }
            catch (Exception e)
            {
                _logger.Error("Unknown Exception in LoadAuctions()", e);
                _notifier.Toast("Error while loading auctions. There probably were no auctions loaded as a result");
                _console.WriteNotificationLine("BLI > Error while loading auctions. There probably were no auctions loaded as a result");
                _console.WriteNotificationLine(e.Message);
            }
            stopwatch.Stop();
        }

        private (int, int, int) ProcessAuctions(List<Auction> auctionsFromFile, long timestamp)
        {
            (int updated, int removed) = UpdateOrDeleteExistingAuctions(auctionsFromFile, timestamp);
            int added = InsertNewAuctions(auctionsFromFile);

            return (added, updated, removed);
        }

        private (int, int) UpdateOrDeleteExistingAuctions(List<Auction> auctionsFromFile, long timestamp)
        {
            TimeSpan timeSinceLastUpdate = new TimeSpan(timestamp - _lastUpdate);
            var savedAuctions = _repo.GetAll<Auction>();
            List<SoldAuction> probablySoldAuctions = new List<SoldAuction>();
            List<Auction> removed = new List<Auction>();
            List<Auction> updated = new List<Auction>();
            foreach (Auction savedAuction in savedAuctions)
            {
                var updatedAuction = auctionsFromFile.FirstOrDefault(a => a.id == savedAuction.id);
                if (updatedAuction == null)
                {
                    if (savedAuction.time_left.ToHoursLeft() > timeSinceLastUpdate.TotalHours)
                    {
                        probablySoldAuctions.Add(new SoldAuction(savedAuction, timeSinceLastUpdate, _config.CurrentRealm));
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
            _lastUpdate = DateTime.Now.Ticks;

            return (updated.Count, removed.Count);
        }

        private int InsertNewAuctions(List<Auction> auctionsFromFile)
        {
            var savedAuctions = _repo.GetAll<Auction>();
            var newAuctions = auctionsFromFile.Where(a => !savedAuctions.Any(r => r.id == a.id)).ToList();
            newAuctions.ForEach(a => a.FirstSeenOn = DateTime.Now);
            _repo.AddAll(newAuctions);
            return newAuctions.Count;
        }

        private AuctionApiResponse GetAuctions()
        {
            IRestResponse response = CallBlizzardAPI($"https://{_config.CurrentRegion}.api.blizzard.com/data/wow/connected-realm/{_config.ConnectedRealmId}/auctions?namespace=dynamic-{_config.CurrentRegion}&locale=en_US");

            var auctionApiResponse = JsonConvert.DeserializeObject<AuctionApiResponse>(response.Content);
            return auctionApiResponse;
        }

        private IRestResponse CallBlizzardAPI(string url)
        {
            if (_token == null)
                _token = GetAccessToken();

            _logger.Debug("Calling POST [" + url + "] with token " + _token);

            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("authorization", $"Bearer {_token}");
            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                _logger.Debug("Got failure response : " + Environment.NewLine + JsonConvert.SerializeObject(response));
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    _token = null;
                throw new BlizzardApiCallException(response);
            }

            return response;
        }

        private string GetAccessToken()
        {
            string url = $"https://{_config.CurrentRegion}.battle.net/oauth/token";
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
                throw new BlizzardTokenRetrievalException(response);

            var tokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(response.Content);

            return tokenResponse.access_token;
        }

        public CharacterProfile GetInfosOnCharacter(string characterName, string realm)
        {
            IRestResponse response = CallBlizzardAPI($"https://{_config.CurrentRegion}.api.blizzard.com/wow/character/{realm.ToLower()}/{characterName}");

            var statusResponse = JsonConvert.DeserializeObject<WowStatusResponse>(response.Content);
            if (statusResponse.status == "nok")
                throw new CharacterInfoException(response);

            return JsonConvert.DeserializeObject<CharacterProfile>(response.Content);
        }

        public WowQualityType GetQuality(string itemId)
        {
            return (WowQualityType)GetItem(itemId).quality.AsQualityTypeEnum;
        }

        public WowItem GetItem(string itemId)
        {
            WowItem item = _repo.GetById<WowItem>(itemId.ToString());
            if (item == null)
            {
                item = GetItemFromAPI(itemId);
                _repo.Delete(item);
                _repo.Add(item);
            }
            return item;
        }

        private WowItem GetItemFromAPI(string itemId)
        {
            IRestResponse response = CallBlizzardAPI($"https://{_config.CurrentRegion}.api.blizzard.com/data/wow/item/{itemId}?namespace=static-{_config.CurrentRegion}");

            WowItem item = JsonConvert.DeserializeObject<WowItem>(response.Content);

            return item;
        }

        public IEnumerable<WowItem> GetItemsWithNameLike(string partialName)
        {
            return _repo.GetAll<WowItem>().Where(i => i.name.en_US.ToLower().Contains(partialName.ToLower()));
        }

        public ConnectedRealmSearchData SearchConnectedRealmsForEnglishName(string realmSlug)
        {
            IRestResponse response = CallBlizzardAPI($"https://{_config.CurrentRegion}.api.blizzard.com/data/wow/search/connected-realm?namespace=dynamic-{_config.CurrentRegion}&realms.slug={realmSlug}");

            ConnectedRealmSearchResponse searchResponse = JsonConvert.DeserializeObject<ConnectedRealmSearchResponse>(response.Content);

            ConnectedRealmSearchData connectedRealm = searchResponse.results[0].data;

            return connectedRealm;
        }

        public string GetItemIdFromName(string name)
        {
            IRestResponse response = CallBlizzardAPI($"https://{_config.CurrentRegion}.api.blizzard.com/data/wow/search/item?namespace=static-{_config.CurrentRegion}&name.en_US={name}");

            ItemSearchResponse searchResponse = JsonConvert.DeserializeObject<ItemSearchResponse>(response.Content);

            if (searchResponse.results.Any())
                return searchResponse.results.First().data.id;
            return null;
        }
    }
}