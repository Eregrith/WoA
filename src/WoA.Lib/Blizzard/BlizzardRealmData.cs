using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.Blizzard
{
    public class BlizzardRealmData : IIdentifiable
    {
        [PrimaryKey]
        public string Id { get; set; }
        public long LastUpdate { get; set; }
    }
}
