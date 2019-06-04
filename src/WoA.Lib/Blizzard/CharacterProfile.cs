using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.Blizzard
{
    public class CharacterProfile
    {
        public long lastModified { get; set; }
        public DateTime LastModified => new DateTime(lastModified);
        public string name { get; set; }
        public string realm { get; set; }
        public string battlegroup { get; set; }
        public int @class { get;set; }
        public string ClassName => ((WowClass)@class).GetDisplayName();
        public int race { get; set; }
        public string RaceName => ((WowRace)race).GetDisplayName();
        public int gender { get; set; }
        public string Gender => gender == 0 ? "Male" : "Female";
        public int level { get; set; }
        public int achievementPoints { get; set; }
        public string thumbnail { get; set; }
        public string calcClass { get; set; }
        public int faction { get; set; }
        public String Faction => faction == 1 ? "Horde" : "Alliance";
        public int totalHonorableKills { get; set; }
    }
}
