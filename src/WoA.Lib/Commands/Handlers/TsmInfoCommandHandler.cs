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

            if (item == null)
            {
                _console.Write("No item matching " + notification.ItemDescription + " was found");
                return Task.CompletedTask;
            }

            _console.WriteAscii(item.Name);
            _console.WriteLine();
            _console.WriteLine("Id : " + item.ItemId);
            _console.WriteLine("Realm : " + item.Realm);
            _console.WriteLine("Item level " + item.Level);
            _console.WriteLine(item.Class + " : " + item.SubClass);

            _console.WriteLine();
            if (item.VendorBuy != 0)
                _console.WriteLine("Can be bought at the vendor for " + item.VendorBuy.ToGoldString());
            _console.WriteLine("Can be sold to a vendor for " + item.VendorSell.ToGoldString());

            _console.WriteLine();
            _console.WriteLine("MarketValue : " + item.MarketValue.ToGoldString());
            _console.WriteLine("MinBuyout : " + item.MinBuyout.ToGoldString());
            _console.WriteLine(item.Quantity + " items in " + item.NumAuctions + " different auctions");

            _console.WriteLine();
            _console.WriteLine("HistoricalPrice : " + item.HistoricalPrice.ToGoldString());
            _console.WriteLine("RegionMarketAvg : " + item.RegionMarketAvg.ToGoldString());
            _console.WriteLine("RegionMinBuyoutAvg : " + item.RegionMinBuyoutAvg.ToGoldString());
            _console.WriteLine("RegionQuantity : " + item.RegionQuantity);
            _console.WriteLine("RegionHistoricalPrice : " + item.RegionHistoricalPrice.ToGoldString());
            _console.WriteLine("RegionSaleAvg : " + item.RegionSaleAvg.ToGoldString());
            _console.WriteLine("RegionAvgDailySold : " + item.RegionAvgDailySold);
            _console.WriteLine("RegionSaleRate : " + item.RegionSaleRate);
            _console.WriteLine();
            double depositCostFor24h = item.VendorSell * 0.6f;
            _console.WriteLine("AH cut for posting at market value: " + ((long)Math.Round(item.MarketValue * 0.05)).ToGoldString());
            _console.WriteLine("Deposit Cost for 24h: " + ((long)Math.Round(depositCostFor24h)).ToGoldString());
            if (item.RegionSaleRate != 0)
            {
                double averagePostsPerSale = 1.0f / item.RegionSaleRate;
                _console.WriteLine("Average number of 24h posts before it sells : " + Math.Round(averagePostsPerSale, 2));
                _console.WriteLine("Average expected deposit cost to sell : " + ((long)Math.Round(depositCostFor24h * (averagePostsPerSale - 1))).ToGoldString());
                _console.WriteLine("Potential profit after this cost and AH cut: " + ((long)Math.Round((item.MarketValue * 0.95) - (depositCostFor24h * (averagePostsPerSale - 1)))).ToGoldString());
            }
            else
            {
                _console.WriteLine("Warning : Sale Rate is ZERO !");
            }
            return Task.CompletedTask;
        }
    }
}
