using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using WoA.Lib;

namespace WorldOfAuctions
{
    public class Configuration : IConfiguration
    {
        public string TsmApiKey => ConfigurationManager.AppSettings.Get("TSM_ApiKey");

        public string Blizzard_ClientId => ConfigurationManager.AppSettings["Blizzard_ClientId"];

        public string Blizzard_ClientSecret => ConfigurationManager.AppSettings["Blizzard_ClientSecret"];

        public string MongoUserName => ConfigurationManager.AppSettings["MongoUsername"];

        public string MongoPassword => ConfigurationManager.AppSettings["MongoPassword"];

        public string CurrentRealm { get; set; } = ConfigurationManager.AppSettings["DefaultRealm"];

        public MongoUrl MongoUrl => new MongoUrlBuilder { DatabaseName = "WoA", Server = new MongoServerAddress("localhost"), Username = MongoUserName, Password = MongoPassword }.ToMongoUrl();
    }
}
