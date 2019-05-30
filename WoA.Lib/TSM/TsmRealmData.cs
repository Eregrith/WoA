using MongoRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.TSM
{
    public class TsmRealmData : IEntity<string>
    {
        public string Id { get; set; }
        public string Realm { get => Id; set => Id = value; }
        public DateTime LastUpdate { get; set; }
    }
}
