using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using WoA.Lib;

namespace WorldOfAuctions
{
    public class Configuration : IConfiguration
    {
        public string TsmApiKey { get; set; } = ConfigurationManager.AppSettings.Get("TSM_ApiKey");

        public string Blizzard_ClientId { get; set; } = ConfigurationManager.AppSettings["Blizzard_ClientId"];

        public string Blizzard_ClientSecret { get; set; } = ConfigurationManager.AppSettings["Blizzard_ClientSecret"];

        public string CurrentRegion { get; set; } = ConfigurationManager.AppSettings["DefaultRegion"];
        public string CurrentRealm { get; set; } = ConfigurationManager.AppSettings["DefaultRealm"];

        public string DatabasePath => ConfigurationManager.AppSettings["DatabasePath"];

        public List<string> PlayerToons { get; set; } = ConfigurationManager.AppSettings ["PlayerToons"].Split(';').ToList();

        public void Save()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["TSM_ApiKey"].Value = TsmApiKey;
            config.AppSettings.Settings["Blizzard_ClientId"].Value = Blizzard_ClientId;
            config.AppSettings.Settings["Blizzard_ClientSecret"].Value = Blizzard_ClientSecret;
            config.AppSettings.Settings["DefaultRegion"].Value = CurrentRegion;
            config.AppSettings.Settings["DefaultRealm"].Value = CurrentRealm;
            config.AppSettings.Settings["PlayerToons"].Value = String.Join(";", PlayerToons);
            config.Save(ConfigurationSaveMode.Minimal);
        }
    }
}
