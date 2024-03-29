﻿using log4net;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WoA.Lib.Persistence;

namespace WoA.Lib.TSM
{
    public class TsmClient : ITsmClient
    {
        private readonly IConfiguration _config;
        private string _baseUrl => "http://api.tradeskillmaster.com/v1/";
        private string getUrlFor(string subUrl) => _baseUrl + $"{subUrl}?format=json&apiKey=" + _config.TsmApiKey;
        private readonly IStylizedConsole _console;
        private readonly IGenericRepository _repo;
        private readonly IUserNotifier _notifier;
        private readonly ILog _logger;
        private const int UPDATE_INTERVAL_TIME_PER_SERVER_IN_HOURS = 4;

        public TsmClient(IConfiguration config, IGenericRepository repo, IStylizedConsole console, IUserNotifier notifier, ILog logger)
        {
            _config = config;
            _console = console;
            _repo = repo;
            _notifier = notifier;
            _logger = logger;
        }

        public void RefreshTsmItemsInRepository()
        {
            try
            {
                if (LastUpdateIsOldEnough())
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    _notifier.Toast("Updating TSM data for " + _config.CurrentRealm);
                    List<TsmItem> items = GetItemsForRealm();
                    MarkRealmUpdated();
                    ReplaceItems(items);
                    stopwatch.Stop();
                    _notifier.Toast("TSM Data updated for " + _config.CurrentRealm + " in " + (stopwatch.ElapsedMilliseconds / 1000) + " sec");
                }
            }
            catch (Exception e)
            {
                _console.WriteNotificationLine("TSM > Error > Error while refreshing tsm data.");
            }
        }

        private void MarkRealmUpdated()
        {
            TsmRealmData realmData = GetRealmDataFromRepo();
            if (realmData == null)
            {
                realmData = new TsmRealmData { Realm = _config.CurrentRegion + '-' + _config.CurrentRealm };
                _repo.Add(realmData);
            }
            realmData.LastUpdate = DateTime.UtcNow;

            _repo.Update(realmData);
        }

        private TsmRealmData GetRealmDataFromRepo()
        {
            return _repo.GetById<TsmRealmData>(_config.CurrentRegion + '-' + _config.CurrentRealm);
        }

        private bool LastUpdateIsOldEnough()
        {
            TsmRealmData realmData = GetRealmDataFromRepo();
            if (realmData == null) return true;

            return (DateTime.UtcNow - realmData.LastUpdate) > TimeSpan.FromHours(UPDATE_INTERVAL_TIME_PER_SERVER_IN_HOURS);
        }

        private List<TsmItem> GetItemsForRealm()
        {
            string url = getUrlFor("item/" + _config.CurrentRegion + "/" + _config.CurrentRealm);
            var items = CallTsmApi<List<TsmItem>>(url) ?? new List<TsmItem>();
            items.ForEach(i =>
            {
                i.ItemId = i.Id;
                i.Realm = _config.CurrentRealm;
                i.Id = _config.CurrentRegion + '-' + _config.CurrentRealm + "-" + i.Id;
            });
            return items;
        }

        private void ReplaceItems(List<TsmItem> items)
        {
            _repo.GetAll<TsmItem>().Where(i => i.Realm == _config.CurrentRealm).ToList().ForEach(i => _repo.Delete(i));
            _repo.AddAll(items);
        }

        public TsmItem GetItem(string id)
        {
            return _repo.GetById<TsmItem>(_config.CurrentRegion + "-" + _config.CurrentRealm + "-" + id);
        }

        private T CallTsmApi<T>(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                _console.WriteNotificationLine("TSM > Error > Request to TSM api failed. Check your TSM ApiKey settings.");
                if (!String.IsNullOrEmpty(response.Content))
                    _console.WriteNotificationLine("TSM > Error > Error: " + JsonConvert.DeserializeObject<TsmError>(response.Content).error);
                if (!String.IsNullOrEmpty(response.ErrorMessage))
                    _console.WriteNotificationLine("TSM > Error > Error: " + response.ErrorMessage);
                _logger.Error($"TSM call error: [{response.StatusCode}] {response.StatusDescription}");
                _logger.Error("Got response: " + Environment.NewLine + JsonConvert.SerializeObject(response));
            }

            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        public string GetItemIdFromName(string itemName)
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
                _console.WriteLine("TSM > No item found called " + itemName);
                return String.Empty;
            }
            if (potential.Count == 1)
                return potential.First().Key.ItemId;
            if (potential.Count < 10)
            {
                _console.WriteLine("TSM > Multiple items found with that name. Which one do you want ?");
                int i = 0;
                _console.StartAlternating();
                potential.ForEach(p => _console.WriteLineWithAlternatingBackground(String.Format("[ {0} ] : {1}", i++, p.First())));
                _console.StopAlternating();
                string line = Console.ReadLine();
                return potential[int.Parse(line)].Key.ItemId;
            }
            _console.WriteLine("TSM > Too many items (" + potential.Count + ") found with a name like " + itemName + ". Please specify the name further");
            return String.Empty;
        }
    }
}
