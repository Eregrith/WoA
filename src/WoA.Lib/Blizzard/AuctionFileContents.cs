using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoA.Lib.Blizzard;

namespace WoA.Lib
{
    public class AuctionFileContents
    {
        public List<Realm> realms { get; set; }
        public List<Auction> auctions { get; set; }
    }

    public class Realm
    {
        public string name { get; set; }
        public string slug { get; set; }
    }

    public class Auction
    {
        [PrimaryKey]
        public string id { get; set; }
        public WowItem item { get; set; }
        public long? bid { get; set; }
        public long? buyout { get; set; }
        public int quantity { get; set; }
        public long? unit_price { get; set; }
        public string timeLeft { get; set; }
        public long PricePerItem => buyout == null ? (unit_price ?? 0) : buyout.Value / quantity;
        public long FullPrice => buyout == null ? (unit_price == null ? 0 : unit_price.Value * quantity) : buyout.Value;
        public DateTime FirstSeenOn { get; set; }
    }
}
