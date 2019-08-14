using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Blizzard;
using WoA.Lib.Business;
using WoA.Lib.Commands.QueryObjects;
using WoA.Lib.SQLite;
using WoA.Lib.TSM;

namespace WoA.Lib.Commands.Handlers
{
    public class FarmablesCommandsHandler
        : INotificationHandler<FarmablesAddCommand>
        , INotificationHandler<FarmablesListCommand>
    {
        private readonly IStylizedConsole _console;
        private readonly IBlizzardClient _blizzard;
        private readonly IGenericRepository _repository;
        private readonly ITsmClient _tsm;

        public FarmablesCommandsHandler(IStylizedConsole console, IBlizzardClient blizzard, IGenericRepository repository, ITsmClient tsm)
        {
            _console = console;
            _blizzard = blizzard;
            _repository = repository;
            _tsm = tsm;
        }

        public Task Handle(FarmablesAddCommand notification, CancellationToken cancellationToken)
        {
            IEnumerable<WowItem> items = _blizzard.GetItemsWithNameLike(notification.ItemDesc);
            if (items.Count() == 1)
            {
                WowItem item = items.First();
                Farmable existingFarmable = _repository.GetById<Farmable>(item.id);
                if (existingFarmable == null)
                {
                    _console.WriteLine($"Creating a farmable for item {item.name.WithQuality((WowQuality)item.quality)} at a rate of {notification.Quantity} {(notification.TimeFrame == TimeFrame.PerHour ? "per hour" : "per minute")}");
                    Farmable farmable = new Farmable
                    {
                        Id = int.Parse(item.id),
                        Quantity = notification.Quantity,
                        TimeFrame = notification.TimeFrame
                    };
                    _repository.Add(farmable);
                }
                else
                {
                    _console.WriteLine($"Updating farmable for item {item.name.WithQuality((WowQuality)item.quality)} to a rate of {notification.Quantity} {(notification.TimeFrame == TimeFrame.PerHour ? "per hour" : "per minute")}");
                    existingFarmable.Quantity = notification.Quantity;
                    existingFarmable.TimeFrame = notification.TimeFrame;
                    _repository.Update(existingFarmable);
                }
            }
            return Task.CompletedTask;
        }

        public Task Handle(FarmablesListCommand notification, CancellationToken cancellationToken)
        {
            IEnumerable<Farmable> farmables = _repository.GetAll<Farmable>();

            _console.WriteLine("You have setup these farmables :");
            _console.WriteLine();
            _console.WriteLine(String.Format("{0,40}{1,12}{2,25}", "Item", "Rate", "Generated value per hour"));

            foreach (Farmable farmable in farmables)
            {
                WowItem item = _blizzard.GetItem(farmable.Id);
                TsmItem tsmItem = _tsm.GetItem(farmable.Id);
                double generatedValue = tsmItem.MarketValue * farmable.Quantity;
                if (farmable.TimeFrame == TimeFrame.PerMinute)
                    generatedValue *= 60;
                _console.WriteLine(String.Format("{0,46}{1,12}{2,25}", item.name.WithQuality((WowQuality)item.quality), farmable.Quantity + (farmable.TimeFrame == TimeFrame.PerHour ? " / h " : " /min"), ((long)generatedValue).ToGoldString()));
            }

            return Task.CompletedTask;
        }
    }
}
