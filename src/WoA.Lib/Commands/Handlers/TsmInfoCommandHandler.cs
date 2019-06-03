using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Commands.QueryObjects;
using WoA.Lib.TSM;

namespace WoA.Lib.Commands.Handlers
{
    public class TsmInfoCommandHandler : INotificationHandler<TsmInfoCommand>
    {
        private readonly ITsmClient _tsm;
        private readonly IAuctionViewer _auctions;
        private readonly IStylizedConsole _console;

        public TsmInfoCommandHandler(ITsmClient tsm, IAuctionViewer auctions, IStylizedConsole console)
        {
            _tsm = tsm;
            _auctions = auctions;
            _console = console;
        }

        public Task Handle(TsmInfoCommand notification, CancellationToken cancellationToken)
        {
            int itemId = _auctions.GetItemId(notification.ItemDescription);
            TsmItem item = _tsm.GetItem(itemId);

            _console.WriteAscii(item.Name);
            _console.WriteLine("");
            _console.WriteLine("Id : " + item.ItemId);
            _console.WriteLine("Realm : " + item.Realm);
            _console.WriteLine("Item level " + item.Level);
            _console.WriteLine(item.Class + " : " + item.SubClass);

            _console.WriteLine("");
            if (item.VendorBuy != 0)
                _console.WriteLine("Can be bought at the vendor for " + item.VendorBuy.ToGoldString());
            _console.WriteLine("Can be sold to a vendor for " + item.VendorSell.ToGoldString());

            _console.WriteLine("");
            _console.WriteLine("MarketValue : " + item.MarketValue.ToGoldString());
            _console.WriteLine("MinBuyout : " + item.MinBuyout.ToGoldString());
            _console.WriteLine(item.Quantity + " items in " + item.NumAuctions + " different auctions");

            _console.WriteLine("");
            _console.WriteLine("HistoricalPrice : " + item.HistoricalPrice.ToGoldString());
            _console.WriteLine("RegionMarketAvg : " + item.RegionMarketAvg.ToGoldString());
            _console.WriteLine("RegionMinBuyoutAvg : " + item.RegionMinBuyoutAvg.ToGoldString());
            _console.WriteLine("RegionQuantity : " + item.RegionQuantity);
            _console.WriteLine("RegionHistoricalPrice : " + item.RegionHistoricalPrice.ToGoldString());
            _console.WriteLine("RegionSaleAvg : " + item.RegionSaleAvg.ToGoldString());
            _console.WriteLine("RegionAvgDailySold : " + item.RegionAvgDailySold);
            _console.WriteLine("RegionSaleRate : " + item.RegionSaleRate);

            return Task.CompletedTask;
        }
    }
}
