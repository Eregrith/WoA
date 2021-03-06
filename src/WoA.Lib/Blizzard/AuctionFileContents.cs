﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string auc { get; set; }
        public int item { get; set; }
        public string owner { get; set; }
        public string ownerRealm { get; set; }
        public long bid { get; set; }
        public long buyout { get; set; }
        public int quantity { get; set; }
        public string timeLeft { get; set; }
        public long PricePerItem => buyout / quantity;
        public DateTime FirstSeenOn { get; set; }
    }
}
