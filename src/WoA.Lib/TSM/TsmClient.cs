using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WoA.Lib.Blizzard;
using WoA.Lib.SQLite;

namespace WoA.Lib.TSM
{
    public class TsmClient : ITsmClient
    {
        private readonly IConfiguration _config;
        private string _baseUrl => "http://api.tradeskillmaster.com/v1/";
        private string getUrlFor(string subUrl) => _baseUrl + $"{subUrl}?format=json&apiKey=" + _config.TsmApiKey;
        private readonly IStylizedConsole _console;
        private readonly IGenericRepository _repo;

        public TsmClient(IConfiguration config, IGenericRepository repo, IStylizedConsole console)
        {
            _config = config;
            _console = console;
            _repo = repo;
        }

        public void RefreshTsmItemsInRepository()
        {
            try
            {
                if (LastUpdateIsOlderThanOneHour())
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    _console.WriteLine("TSM > Updating data for " + _config.CurrentRealm);
                    List<TsmItem> items = GetItemsForRealm();
                    MarkRealmUpdated();
                    ReplaceItems(items);
                    stopwatch.Stop();
                    _console.WriteLine("TSM > Data updated for " + _config.CurrentRealm + " in " + stopwatch.ElapsedMilliseconds + "ms");
                }
                else
                {
                    _console.WriteLine("TSM > No Update done for " + _config.CurrentRealm + ", last update is too recent.");
                }
            }
            catch (Exception e)
            {
                _console.WriteLine("TSM > Error > Error while refreshing tsm data.");
            }
        }

        private void MarkRealmUpdated()
        {
            TsmRealmData realmData = _repo.GetById<TsmRealmData>(_config.CurrentRealm);
            if (realmData == null)
            {
                realmData = new TsmRealmData { Realm = _config.CurrentRealm };
                _repo.Add(realmData);
            }
            realmData.LastUpdate = DateTime.UtcNow;

            _repo.Update(realmData);
        }

        private bool LastUpdateIsOlderThanOneHour()
        {
            TsmRealmData realmData = _repo.GetById<TsmRealmData>(_config.CurrentRealm);
            if (realmData == null) return true;

            return (DateTime.UtcNow - realmData.LastUpdate) > TimeSpan.FromHours(1);
        }

        private List<TsmItem> GetItemsForRealm()
        {
            string url = getUrlFor("item/" + _config.CurrentRegion + "/" + _config.CurrentRealm);
            var items = CallTsmApi<List<TsmItem>>(url);
            items.ForEach(i =>
            {
                i.ItemId = int.Parse(i.Id);
                i.Realm = _config.CurrentRealm;
                i.Id = _config.CurrentRealm + "-" + i.Id;
            });
            return items;
        }

        private void ReplaceItems(List<TsmItem> items)
        {
            _repo.GetAll<TsmItem>().Where(i => i.Realm == _config.CurrentRealm).ToList().ForEach(i => _repo.Delete(i));
            _repo.AddAll(items);
        }

        public TsmItem GetItem(int id)
        {
            return _repo.GetById<TsmItem>(_config.CurrentRealm + "-" + id);
        }

        private T CallTsmApi<T>(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                _console.WriteLine("TSM > Error > Request to TSM api failed. Check your TSM ApiKey settings.");

            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        public int GetItemIdFromName(string itemName)
        {
            var potential = _repo.GetAll<TsmItem>()
                                .Where(i => i.Name.ToLower().Contains(itemName.ToLower()))
                                .AsEnumerable()
                                .GroupBy(i => new { i.ItemId, i.Name })
                                .ToList();

            if (potential.Any(i => i.Key.Name.ToLower() == itemName.ToLower()))
                return potential.First(i => i.Key.Name.ToLower() == itemName.ToLower()).Key.ItemId;

            if (potential.Count == 0)
            {
                _console.WriteLine("No item found called " + itemName);
                return 0;
            }
            if (potential.Count == 1)
                return potential.First().Key.ItemId;
            if (potential.Count < 10)
            {
                _console.WriteLine("Multiple items found with that name. Which one do you want ?");
                int i = 0;
                _console.StartAlternating();
                potential.ForEach(p => _console.WriteLineWithAlternatingBackground(String.Format("[ {0} ] : {1}", i++, p.First())));
                _console.StopAlternating();
                string line = Console.ReadLine();
                return potential[int.Parse(line)].Key.ItemId;
            }
            _console.WriteLine("Too many items (" + potential.Count + ") found with a name like " + itemName + ". Please specify the name further");
            return 0;
        }
    }
}
