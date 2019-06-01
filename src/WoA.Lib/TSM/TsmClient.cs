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
    public class TsmClient : ITsmClient
    {
        private readonly IConfiguration _config;
        private string _baseUrl => "http://api.tradeskillmaster.com/v1/";
        private string getUrlFor(string subUrl) => _baseUrl + $"{subUrl}?format=json&apiKey=" + _config.TsmApiKey;
        private readonly IRepository<TsmItem> _items;
        private readonly IRepository<TsmRealmData> _realms;
        private readonly IStylizedConsole _console;

        public TsmClient(IConfiguration config, IRepository<TsmItem> items, IRepository<TsmRealmData> realms, IStylizedConsole console)
        {
            _config = config;
            _items = items;
            _realms = realms;
            _console = console;
        }

        public void RefreshTsmItemsInRepository()
        {
            if (LastUpdateIsOlderThanOneHour())
            {
                _console.WriteLine("TSM > Updating data for " + _config.CurrentRealm);
                List<TsmItem> items = GetItemsForRealm();
                MarkRealmUpdated();
                ReplaceItems(items);
                _console.WriteLine("TSM > Data updated for " + _config.CurrentRealm);
            }
            else
            {
                _console.WriteLine("TSM > No Update done for "+ _config.CurrentRealm + ", last update is too recent.");
            }
        }

        private void MarkRealmUpdated()
        {
            TsmRealmData realmData = _realms.FirstOrDefault(r => r.Realm == _config.CurrentRealm);
            if (realmData == null)
            {
                realmData = new TsmRealmData { Realm = _config.CurrentRealm };
                _realms.Add(realmData);
            }
            realmData.LastUpdate = DateTime.UtcNow;
            _realms.Update(realmData);
        }

        private bool LastUpdateIsOlderThanOneHour()
        {
            TsmRealmData realmData = _realms.FirstOrDefault(r => r.Realm == _config.CurrentRealm);
            if (realmData == null) return true;

            return (DateTime.UtcNow - realmData.LastUpdate) > TimeSpan.FromHours(1);
        }

        private List<TsmItem> GetItemsForRealm()
        {
            string url = getUrlFor("item/EU/" + _config.CurrentRealm);
            var items = CallTsmApi<List<TsmItem>>(url);
            items.ForEach(i => { i.ItemId = int.Parse(i.Id);  i.Realm = _config.CurrentRealm; i.Id = _config.CurrentRealm + "-" + i.Id; });
            return items;
        }

        private void ReplaceItems(List<TsmItem> items)
        {
            _items.Collection.Remove(Query.EQ("Realm", _config.CurrentRealm));
            _items.Add(items);
        }

        public TsmItem GetItem(int id)
        {
            return _items.FirstOrDefault(i => i.Id == _config.CurrentRealm + "-" + id);
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
                _console.WriteLine("No item found called " + itemName);
                return 0;
            }
            if (potential.Count() == 1)
                return potential.First().Key.ItemId;
            if (potential.Count() < 5)
            {
                _console.WriteLine("Multiple items found with that name. Which one do you want ?");
                int i = 0;
                potential.ForEach(p => _console.WriteLine(String.Format("[ {0} ] : {1}", i++, p.First())));
                string line = Console.ReadLine();
                return potential[int.Parse(line)].Key.ItemId;
            }
            _console.WriteLine("Too many items ("+potential.Count+") found with a name like " + itemName + ". Please specify the name further");
            return 0;
        }
    }
}
