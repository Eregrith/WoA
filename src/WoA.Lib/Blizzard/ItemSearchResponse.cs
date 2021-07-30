using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.Blizzard
{
    public class ItemSearchResponse
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public int maxPageSize { get; set; }
        public int pageCount { get; set; }
        public List<ItemSearchResult> results { get; set; }
    }

    public class ItemSearchResult
    {
        public WowKey key { get; set; }
        public WowItem data { get; set; }
    }
}
