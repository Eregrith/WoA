using System.Configuration;
using WoA.Lib;

namespace WorldOfAuctions
{
    public class Configuration : IConfiguration
    {
        public string TsmApiKey => ConfigurationManager.AppSettings.Get("TSM_ApiKey");

        public string Blizzard_ClientId => ConfigurationManager.AppSettings["Blizzard_ClientId"];

        public string Blizzard_ClientSecret => ConfigurationManager.AppSettings["Blizzard_ClientSecret"];

        private string MongoUserName => ConfigurationManager.AppSettings["MongoUsername"];

        private string MongoPassword => ConfigurationManager.AppSettings["MongoPassword"];

        public string CurrentRealm { get; set; } = ConfigurationManager.AppSettings["DefaultRealm"];

        public string DatabasePath => ConfigurationManager.AppSettings["DatabasePath"];
    }
}
