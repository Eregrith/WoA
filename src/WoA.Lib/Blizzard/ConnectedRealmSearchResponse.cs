using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WoA.Lib.Blizzard
{
    public class ConnectedRealmSearchResponse
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public int maxPageSize { get; set; }
        public int pageCount { get; set; }
        public List<ConnectedRealmSearchResult> results { get; set; }
    }

    public class ConnectedRealmSearchResult
    {
        public WowKey key { get; set; }
        public ConnectedRealmSearchData data { get; set; }
    }

    public class ConnectedRealmSearchData
    {
        public List<ConnectedRealmSubRealm> realms { get; set; }
        public int id { get; set; }
        public bool has_queue { get; set; }
        public ConnectedRealmStatus status { get; set; }
        public ConnectedRealmPopulation population { get; set; }
        public string FullConnectedRealmEnglishNames => String.Join(" - ", realms.Select(r => r.name.en_US));
    }

    public class ConnectedRealmSubRealm
    {
        public bool is_tournament { get; set; }
        public string timezone { get; set; }
        public WowLocalizedString name { get; set; }
        public int id { get; set; }
        public WowRegion region { get; set; }
        public WowLocalizedString category { get; set; }
        public string locale { get; set; }
        // public ConnectedRealmType type { get; set; }
        public string slug { get; set; }
    }

    public enum ConnectedRealmStatusType
    {
        UP,
        DOWN
    }

    public class ConnectedRealmStatus
    {
        public WowLocalizedString name { get; set; }
        public ConnectedRealmStatusType type { get; set; }
    }

    public enum ConnectedRealmPopulationType
    {
        LOW,
        MEDIUM,
        FULL
    }

    public class ConnectedRealmPopulation
    {
        public WowLocalizedString name { get; set; }
        public ConnectedRealmPopulationType type { get; set; }
    }
}
