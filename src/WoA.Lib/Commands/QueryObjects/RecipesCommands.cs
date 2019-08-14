using MediatR;
using System.Text.RegularExpressions;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{

    [WoACommand(RegexToMatch = "^recipe?$")]
    public class RecipeUsageCommand : INotification
    { }


    [WoACommand(RegexToMatch = "^recipe list$")]
    public class RecipeListCommand : INotification
    { }

    [WoACommand(RegexToMatch = "^recipe add (?<itemDesc>.+)$")]
    public class RecipeAddCommand : INotification
    {
        public string ItemDesc { get; set; }

        public RecipeAddCommand(Match m)
        {
            ItemDesc = m.Groups["itemDesc"].Value;
        }
    }


    [WoACommand(RegexToMatch = "^recipe cancel$")]
    public class RecipeCancelCommand : INotification
    { }


    [WoACommand(RegexToMatch = "^recipe reagent add (?<quantity>[0-9]+) (?<itemDesc>.+)$")]
    public class RecipeAddReagentCommand : INotification
    {
        public string ItemDesc { get; set; }
        public int Quantity { get; set; }

        public RecipeAddReagentCommand(Match m)
        {
            ItemDesc = m.Groups["itemDesc"].Value;
            Quantity = int.Parse(m.Groups["quantity"].Value);
        }
    }


    [WoACommand(RegexToMatch = "^recipe save (?<name>.+)$")]
    public class RecipeSaveCommand : INotification
    {
        public string Name { get; set; }

        public RecipeSaveCommand(Match m)
        {
            Name = m.Groups["name"].Value;
        }
    }


    [WoACommand(RegexToMatch = "^recipe show current$")]
    public class RecipeShowCurrentCommand : INotification
    { }
}
