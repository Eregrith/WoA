using MongoDB.Bson;
using MongoDB.Driver.Builders;
using MongoRepository;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WoA.Lib.TSM
{
    public class TsmClient
    {
        private readonly string _apiKey;
        private string _baseUrl => "http://api.tradeskillmaster.com/v1/";
        private string getUrlFor(string subUrl) => _baseUrl + $"{subUrl}?format=json&apiKey=" + _apiKey;
        private readonly MongoRepository<TsmItem> _items;
        private readonly MongoRepository<TsmRealmData> _realms;

        public TsmClient(string apiKey, MongoRepository<TsmItem> items, MongoRepository<TsmRealmData> realms)
        {
            _apiKey = apiKey;
            _items = items;
            _realms = realms;
        }

        public void RefreshTsmItemsInRepository(string realm)
        {
            if (LastUpdateIsOlderThanOneHour(realm))
            {
                Console.WriteLine("TSM > Updating data for " + realm);
                List<TsmItem> items = GetItemsForRealm(realm);
                MarkRealmUpdated(realm);
                ReplaceItems(realm, items);
                Console.WriteLine("TSM > Data updated for " + realm);
            }
            else
            {
                Console.WriteLine("TSM > No Update done for "+realm+", last update is too recent.");
            }
        }

        private void MarkRealmUpdated(string realm)
        {
            TsmRealmData realmData = _realms.FirstOrDefault(r => r.Realm == realm);
            if (realmData == null)
            {
                realmData = new TsmRealmData { Realm = realm };
                _realms.Add(realmData);
            }
            realmData.LastUpdate = DateTime.UtcNow;
            _realms.Update(realmData);
        }

        private bool LastUpdateIsOlderThanOneHour(string realm)
        {
            TsmRealmData realmData = _realms.FirstOrDefault(r => r.Realm == realm);
            if (realmData == null) return true;

            return (DateTime.UtcNow - realmData.LastUpdate) > TimeSpan.FromHours(1);
        }

        private List<TsmItem> GetItemsForRealm(string realm)
        {
            string url = getUrlFor("item/EU/" + realm);
            var items = CallTsmApi<List<TsmItem>>(url);
            items.ForEach(i => { i.ItemId = int.Parse(i.Id);  i.Realm = realm; i.Id = realm + "-" + i.Id; });
            return items;
        }

        private void ReplaceItems(string realm, List<TsmItem> items)
        {
            _items.Collection.Remove(Query.EQ("Realm", realm));
            _items.Add(items);
        }

        public TsmItem GetItem(int id, string realm)
        {
            return _items.FirstOrDefault(i => i.Id == realm + "-" + id);
        }

        private T CallTsmApi<T>(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        public int GetItemIdFromName(string itemName)
        {
            var potential = _items.Where(i => i.Name.ToLower().Contains(itemName.ToLower()))
                                        .AsEnumerable()
                                        .GroupBy(i => new { i.ItemId, i.Name })
                                        .ToList();

            if (potential.Any(i => i.Key.Name.ToLower() == itemName.ToLower()))
                return potential.First(i => i.Key.Name.ToLower() == itemName.ToLower()).Key.ItemId;

            if (potential.Count() == 0)
            {
                Console.WriteLine("No item found called " + itemName);
                return 0;
            }
            if (potential.Count() == 1)
                return potential.First().Key.ItemId;
            if (potential.Count() < 5)
            {
                Console.WriteLine("Multiple items found with that name. Which one do you want ?");
                int i = 0;
                potential.ForEach(p => Console.WriteLine("[ {0} ] : {1}", i++, p.First()));
                string line = Console.ReadLine();
                return potential[int.Parse(line)].Key.ItemId;
            }
            Console.WriteLine("Too many items ("+potential.Count+") found with a name like " + itemName + ". Please specify the name further");
            return 0;
        }
    }
}
