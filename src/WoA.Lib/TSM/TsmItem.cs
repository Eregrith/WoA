using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoA.Lib.TSM
{
    public class TsmItem : IEntity<string>
    {
        public string Id { get; set; }
        public string Realm { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string Class { get; set; }
        public string SubClass { get; set; }
        public long VendorBuy { get; set; }
        public long VendorSell { get; set; }
        public long MarketValue { get; set; }
        public long MinBuyout { get; set; }
        public long Quantity { get; set; }
        public long NumAuctions { get; set; }
        public long HistoricalPrice { get; set; }
        public long RegionMarketAvg { get; set; }
        public long RegionMinBuyoutAvg { get; set; }
        public long RegionQuantity { get; set; }
        public long RegionHistoricalPrice { get; set; }
        public double RegionSaleAvg { get; set; }
        public double RegionAvgDailySold { get; set; }
        public double RegionSaleRate { get; set; }
        public string URL { get; set; }
        public int LastModified { get; set; }
        public int ItemId { get; internal set; }

        public override string ToString()
        {
            return $"{Name}({Id}) : MkPrice({MarketValue.ToGoldString()})";
        }
    }
}
