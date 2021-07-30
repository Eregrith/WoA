using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Blizzard;
using WoA.Lib.Commands.QueryObjects;
using WoA.Lib.Persistence;

namespace WoA.Lib.Commands.Handlers
{
    public class SalesExportCommandHandler : INotificationHandler<SalesExportCommand>
    {
        private readonly IGenericRepository _repository;
        private readonly IStylizedConsole _console;
        private const string separator = ",";

        public SalesExportCommandHandler(IGenericRepository repository, IStylizedConsole console)
        {
            _repository = repository;
            _console = console;
        }

        public Task Handle(SalesExportCommand notification, CancellationToken cancellationToken)
        {
            List<SoldAuction> soldAuctionsForRealm = _repository.GetAll<SoldAuction>().Where(sa => sa.SourceRealm == notification.Realm).ToList();
            if (!notification.Path.EndsWith(".csv"))
                notification.Path = Path.Combine(notification.Path, notification.Realm.ToLowerInvariant() + ".csv");
            _console.WriteLine($"{soldAuctionsForRealm.Count} auctions to export to {notification.Path}");
            using (StreamWriter sw = new StreamWriter(notification.Path))
            {
                sw.WriteLine("Buyout"
                    + separator + "First seen on"
                    + separator + "Price per item"
                    + separator + "Quantity"
                    + separator + "Item ID"
                    + separator + "Sale detected On"
                    + separator + "Time left when sold"
                );
                foreach (SoldAuction auction in soldAuctionsForRealm)
                {
                    sw.WriteLine(             auction.buyout
                                + separator + auction.FirstSeenOn
                                + separator + auction.PricePerItem
                                + separator + auction.quantity
                                + separator + auction.item
                                + separator + auction.SaleDetectedOn
                                + separator + auction.timeLeft
                    );
                }
                sw.Close();
            }
            return Task.CompletedTask;
        }
    }
}