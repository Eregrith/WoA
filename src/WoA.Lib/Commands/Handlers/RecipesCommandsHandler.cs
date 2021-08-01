using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoA.Lib.Blizzard;
using WoA.Lib.Business;
using WoA.Lib.Business.StateObjects;
using WoA.Lib.Commands.QueryObjects;
using WoA.Lib.Persistence;
using WoA.Lib.TSM;

namespace WoA.Lib.Commands.Handlers
{
    public class RecipesCommandsHandler
        : INotificationHandler<RecipeAddCommand>
        , INotificationHandler<RecipeListCommand>
        , INotificationHandler<RecipeCancelCommand>
        , INotificationHandler<RecipeAddReagentCommand>
        , INotificationHandler<RecipeSaveCommand>
        , INotificationHandler<RecipeShowCurrentCommand>
    {
        private readonly IStylizedConsole _console;
        private readonly ITsmClient _tsm;
        private readonly IBlizzardClient _blizzard;
        private readonly IApplicationStateManager _state;
        private readonly IGenericRepository _repository;

        public RecipesCommandsHandler(IStylizedConsole console, ITsmClient tsm, IBlizzardClient blizzard, IApplicationStateManager state, IGenericRepository repository)
        {
            _console = console;
            _tsm = tsm;
            _blizzard = blizzard;
            _state = state;
            _repository = repository;
        }

        public Task Handle(RecipeAddCommand notification, CancellationToken cancellationToken)
        {
            string id = _tsm.GetItemIdFromName(notification.ItemDesc);
            WowItem item = _blizzard.GetItem(id);
            _console.WriteLine($"Started creating a recipe for item {item.name.en_US.WithQuality(item.quality.AsQualityTypeEnum)}");
            _state.SetState(ApplicationState.RecipeCreation, new RecipeCreationState(id));
            return Task.CompletedTask;
        }

        public Task Handle(RecipeListCommand notification, CancellationToken cancellationToken)
        {
            var recipes = _repository.GetAll<Recipe>();
            _console.WriteLine($"There are {recipes.Count()} recipes :");
            _console.WriteLine();
            foreach (var recipeGroup in recipes.GroupBy(r => r.ItemId))
            {
                WowItem item = _blizzard.GetItem(recipeGroup.Key);
                _console.WriteLine($"{item.name.en_US.WithQuality(item.quality.AsQualityTypeEnum)} :");
                foreach (var recipe in recipeGroup)
                    _console.WriteLine($"  {recipe.Name} => 1");
                _console.WriteLine();
            }
            return Task.CompletedTask;
        }

        public Task Handle(RecipeCancelCommand notification, CancellationToken cancellationToken)
        {
            if (_state.CurrentState != ApplicationState.RecipeCreation)
                _console.WriteLine("You are not in the process of creating a recipe");
            else
            {
                _console.WriteLine($"Canceled recipe creation without saving");
                _state.SetNeutral();
            }
            return Task.CompletedTask;
        }

        public Task Handle(RecipeAddReagentCommand notification, CancellationToken cancellationToken)
        {
            if (_state.CurrentState != ApplicationState.RecipeCreation)
            {
                _console.WriteLine("You are not in the process of creating a recipe");
                return Task.CompletedTask;
            }

            string id = _tsm.GetItemIdFromName(notification.ItemDesc);
            WowItem item = _blizzard.GetItem(id);
            _console.WriteLine($"Adding {notification.Quantity} x {item.name.en_US.WithQuality(item.quality.AsQualityTypeEnum)} to the recipe");
            RecipeCreationState recipe = (_state.StateInfo as RecipeCreationState);
            if (recipe.Reagents.ContainsKey(item))
                recipe.Reagents[item] += notification.Quantity;
            else
                recipe.Reagents.Add(item, notification.Quantity);
            return Task.CompletedTask;
        }

        public Task Handle(RecipeSaveCommand notification, CancellationToken cancellationToken)
        {
            if (_state.CurrentState != ApplicationState.RecipeCreation)
            {
                _console.WriteLine("You are not in the process of creating a recipe");
                return Task.CompletedTask;
            }
            _console.WriteLine($"Saving recipe as {notification.Name}");
            RecipeCreationState recipeState = (_state.StateInfo as RecipeCreationState);

            Recipe recipe = new Recipe
            {
                Name = notification.Name,
                ItemId = recipeState.ItemId
            };
            _repository.Add(recipe);

            foreach (KeyValuePair<WowItem, int> reagent in recipeState.Reagents)
            {
                Reagent r = new Reagent
                {
                    Id = Guid.NewGuid().ToString(),
                    Recipe = notification.Name,
                    ItemId = reagent.Key.id,
                    Quantity = reagent.Value
                };
                _repository.Add(r);
            }

            _state.SetNeutral();

            return Task.CompletedTask;
        }

        public Task Handle(RecipeShowCurrentCommand notification, CancellationToken cancellationToken)
        {
            if (_state.CurrentState != ApplicationState.RecipeCreation)
            {
                _console.WriteLine("You are not in the process of creating a recipe");
                return Task.CompletedTask;
            }

            RecipeCreationState recipeState = (_state.StateInfo as RecipeCreationState);
            WowItem item = _blizzard.GetItem(recipeState.ItemId);
            _console.WriteLine($"You are making a recipe for {item.name.en_US.WithQuality(item.quality.AsQualityTypeEnum)}");

            _console.WriteLine(String.Format("{0,40}{1,12}", "Item", "Quantity"));

            foreach (KeyValuePair<WowItem, int> reagent in recipeState.Reagents)
            {
                _console.WriteLine(String.Format("{0,46}{1,12}", reagent.Key.name.en_US.WithQuality(reagent.Key.quality.AsQualityTypeEnum), reagent.Value));
            }

            return Task.CompletedTask;
        }
    }
}
