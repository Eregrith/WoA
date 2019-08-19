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
                long totalForRecipe = 0;
                foreach (Reagent reagent in reagents)
                {
                    TsmItem tsmReagent = _tsm.GetItem(reagent.ItemId);
                    WowItem wowReagent = _blizzard.GetItem(reagent.ItemId);
                    _console.WriteLine(String.Format("Sources for : {0,46} (x {1,12})", wowReagent.name.WithQuality((WowQuality)wowReagent.quality), reagent.Quantity));
                    List<ReagentSource> reagentSources = GetBestReagentSource(tsmReagent, reagent.Quantity);
                    int neededQuantity = reagent.Quantity;
                    _console.WriteLine(String.Format("{0,50}{1,10}{2,20}{3,20}", "Source", "Quantity", "Cost per item", "Total cost"));
                    foreach (ReagentSource reagentSource in reagentSources.OrderBy(s => s.Cost))
                    {
                        neededQuantity -= reagentSource.Quantity;
                        totalForRecipe += reagentSource.TotalCost;
                        _console.WriteLine(String.Format("{0,50}{1,10}{2,20}{3,20}", reagentSource.Source, reagentSource.Quantity, reagentSource.Cost.ToGoldString(), reagentSource.TotalCost.ToGoldString()));
                        if (neededQuantity <= 0)
                            break;
                    }
                }
                _console.WriteLine($"Total crafting price (using market values) for recipe {recipe.Name}: " + totalForRecipe.ToGoldString());
                _console.WriteLine();
            }

            return Task.CompletedTask;
        }

        private List<ReagentSource> GetBestReagentSource(TsmItem item, int quantity)
        {
            List<ReagentSource> sources = new List<ReagentSource>();

            if (item.VendorBuy != 0)
            {
                sources.Add(new ReagentSource
                {
                    Source = "Vendor Buy",
                    Quantity = quantity,
                    Cost = item.VendorBuy,
                    TotalCost = item.VendorBuy * quantity
                });
            }
            CheckBestCraftingSource(item, sources, quantity);
            CheckRealAHBuy(item, sources, quantity);
            return sources;
        }

        private void CheckRealAHBuy(TsmItem item, List<ReagentSource> sources, int quantity)
        {
            List<Auction> auctions = _blizzard.Auctions.Where(a => a.item == item.ItemId).ToList();

            int quantityNeeded = quantity;
            foreach (Auction auction in auctions.OrderBy(a => a.PricePerItem))
            {
                if (auction.buyout > 0)
                {
                    sources.Add(new ReagentSource
                    {
                        Source = "AH Buy",
                        Quantity = auction.quantity,
                        Cost = auction.buyout / Math.Min(auction.quantity, quantityNeeded),
                        TotalCost = auction.buyout
                    });
                    quantityNeeded -= auction.quantity;
                }
                if (quantityNeeded <= 0)
                    break;
            }
        }

        private void CheckBestCraftingSource(TsmItem item, List<ReagentSource> sources, int quantity)
        {
            List<Recipe> recipesForReagent = _repository.GetAll<Recipe>().Where(r => r.ItemId == item.ItemId).ToList();

            if (recipesForReagent.Count == 0)
                return;

            foreach (Recipe recipe in recipesForReagent)
            {
                List<Reagent> reagents = _repository.GetAll<Reagent>().Where(r => r.Recipe == recipe.Name).ToList();
                long totalForRecipe = 0;
                foreach (Reagent reagent in reagents)
                {
                    TsmItem tsmReagent = _tsm.GetItem(reagent.ItemId);
                    List<ReagentSource> reagentSources = GetBestReagentSource(tsmReagent, reagent.Quantity * quantity);
                    int quantityNeeded = quantity;
                    foreach (ReagentSource source in reagentSources.OrderBy(r => r.Cost))
                    {
                        quantityNeeded -= source.Quantity;
                        totalForRecipe += source.TotalCost;
                        if (quantityNeeded <= 0)
                            break;
                    }
                }
                sources.Add(new ReagentSource
                {
                    Source = $"Craft recipe '{recipe.Name}'",
                    Quantity = quantity,
                    TotalCost = totalForRecipe,
                    Cost = totalForRecipe,
                });
            }
        }
    }
}
