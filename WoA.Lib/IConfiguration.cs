using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib
{
    public interface IConfiguration
    {
        string TsmApiKey { get; }
        string Blizzard_ClientId { get; }
        string Blizzard_ClientSecret { get; }

        string CurrentRealm { get; set; }
        MongoUrl MongoUrl { get; }
    }
}
