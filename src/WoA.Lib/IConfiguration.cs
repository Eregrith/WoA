using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib
{
    public interface IConfiguration
    {
        string TsmApiKey { get; set; }
        string Blizzard_ClientId { get; set; }
        string Blizzard_ClientSecret { get; set; }

        string CurrentRegion { get; set; }
        string CurrentRealm { get; set; }
        int? ConnectedRealmId { get; set; }
        string DatabasePath { get; }

        void Save();
    }
}
