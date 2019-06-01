using SQLite;
using System;

namespace WoA.Lib.TSM
{
    public class TsmRealmData
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Realm { get => Id; set => Id = value; }
        public DateTime LastUpdate { get; set; }
    }
}
