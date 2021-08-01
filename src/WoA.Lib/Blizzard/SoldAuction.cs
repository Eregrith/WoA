using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.Blizzard
{
    public class SoldAuction : Auction
    {
        public DateTime SaleDetectedOn { get; set; }
        public TimeSpan ElapsedTimeSinceLastUpdate { get; set; }
        public string SourceRealm { get; set; }

        public SoldAuction(Auction a, TimeSpan timeSinceLastUpdate, string sourceRealm)
        {
            id = a.id;
            item = a.item;
            bid = a.bid;
            buyout = a.buyout;
            quantity = a.quantity;
            unit_price = a.unit_price;
            time_left = a.time_left;
            FirstSeenOn = a.FirstSeenOn;
            ElapsedTimeSinceLastUpdate = timeSinceLastUpdate;
            SaleDetectedOn = DateTime.Now;
            SourceRealm = sourceRealm;
        }

        public SoldAuction()
        { }
    }
}
