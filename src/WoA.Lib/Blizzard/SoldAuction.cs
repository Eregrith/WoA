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
            auc = a.auc;
            item = a.item;
            owner = a.owner;
            ownerRealm = a.ownerRealm;
            bid = a.bid;
            buyout = a.buyout;
            quantity = a.quantity;
            timeLeft = a.timeLeft;
            FirstSeenOn = a.FirstSeenOn;
            ElapsedTimeSinceLastUpdate = timeSinceLastUpdate;
            SaleDetectedOn = DateTime.Now;
            SourceRealm = sourceRealm;
        }

        public SoldAuction()
        { }
    }
}
