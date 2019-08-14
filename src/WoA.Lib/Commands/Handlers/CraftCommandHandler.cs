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
    public class CraftCommandHandler
        : INotificationHandler<CraftCommand>
    {
        private readonly IStylizedConsole _console;
        private readonly IGenericRepository _repository;
        private readonly IBlizzardClient _blizzard;
        private readonly ITsmClient _tsm;

        public CraftCommandHandler(IStylizedConsole console, IGenericRepository repository, IBlizzardClient blizzard, ITsmClient tsm)
        {
            _console = console;
            _repository = repository;
            _blizzard = blizzard;
            _tsm = tsm;
        }

        public Task Handle(CraftCommand notification, CancellationToken cancellationToken)
        {
            int id = _tsm.GetItemIdFromName(notification.ItemDesc);
            TsmItem tsmItem = _tsm.GetItem(id);
            _console.WriteLine($"Crafting for item {tsmItem}");
            _console.WriteLine();

            List<Recipe> recipes = _repository.GetAll<Recipe>().Where(r => r.ItemId == id).ToList();

            if (recipes.Count == 0)
            {
                _console.WriteLine("No recipe found for this item");
                return Task.CompletedTask;
            }

            foreach (Recipe recipe in recipes)
            {
                List<Reagent> reagents = _repository.GetAll<Reagent>().Where(r => r.Recipe == recipe.Name).ToList();
                _console.WriteLine($"With recipe {recipe.Name}:");
                _console.WriteLine(String.Format("{0,40}{1,12}{2,40}{3,20}{4,20}", "Item", "Quantity", "Source", "Cost", "Total"));
                long totalForRecipe = 0;
                foreach (Reagent reagent in reagents)
                {
                    TsmItem tsmReagent = _tsm.GetItem(reagent.ItemId);
                    WowItem wowReagent = _blizzard.GetItem(reagent.ItemId);
                    ReagentSource reagentSource = GetBestReagentSource(tsmReagent, reagent.Quantity);
                    _console.WriteLine(String.Format("{0,46}{1,12}{2,40}{3,20}{4,20}", wowReagent.name.WithQuality((WowQuality)wowReagent.quality), reagent.Quantity, reagentSource.Source, reagentSource.Cost.ToGoldString(), (reagent.Quantity * reagentSource.Cost).ToGoldString()));
                    totalForRecipe += reagent.Quantity * reagentSource.Cost;
                }
                _console.WriteLine($"Total crafting price (using market values) for recipe {recipe.Name}: " + totalForRecipe.ToGoldString());
                _console.WriteLine();
            }

            return Task.CompletedTask;
        }

        private ReagentSource GetBestReagentSource(TsmItem item, int quantity)
        {
            ReagentSource source = new ReagentSource
            {
                Source = "AH buy",
                Cost = item.MarketValue,
            };
            CheckRealAHBuy(item, source, quantity);
            CheckBestCraftingSource(item, source, quantity);
            return source;
        }

        private void CheckRealAHBuy(TsmItem item, ReagentSource source, int quantity)
        {
            List<Auction> auctions = _blizzard.Auctions.Where(a => a.item == item.ItemId).ToList();
            if (auctions.Sum(a => a.quantity) < quantity) return;

            int quantityNeeded = quantity;
            long totalPrice = 0;
            foreach (Auction auction in auctions.OrderBy(a => a.PricePerItem))
            {
                if (auction.buyout > 0)
                {
                    totalPrice += auction.buyout;
                    quantityNeeded -= auction.quantity;
                }
                if (quantityNeeded <= 0)
                    break;
            }
            long averagePrice = totalPrice / quantity;
            if (averagePrice < source.Cost)
            {
                source.Source = "Real AH Buy (x" + (quantity - quantityNeeded) + ")";
                source.Cost = averagePrice;
            }
        }

        private ReagentSource CheckBestCraftingSource(TsmItem item, ReagentSource source, int quantity)
        {
            List<Recipe> recipesForReagent = _repository.GetAll<Recipe>().Where(r => r.ItemId == item.ItemId).ToList();

            if (recipesForReagent.Count == 0)
                return source;

            foreach (Recipe recipe in recipesForReagent)
            {
                List<Reagent> reagents = _repository.GetAll<Reagent>().Where(r => r.Recipe == recipe.Name).ToList();
                long totalForRecipe = 0;
                foreach (Reagent reagent in reagents)
                {
                    TsmItem tsmReagent = _tsm.GetItem(reagent.ItemId);
                    ReagentSource reagentSource = GetBestReagentSource(tsmReagent, quantity);
                    totalForRecipe += reagent.Quantity * reagentSource.Cost;
                }
                if (totalForRecipe < source.Cost)
                {
                    source.Source = "Craft: " + recipe.Name;
                    source.Cost = totalForRecipe;
                }
            }

            return source;
        }
    }
}
