using System;
using System.Collections.Generic;
using System.Text;
using WoA.Lib.Blizzard;

namespace WoA.Lib.Business.StateObjects
{
    public class RecipeCreationState : IState
    {
        public int ItemId { get; set; }
        public Dictionary<WowItem, int> Reagents { get; set; }

        public RecipeCreationState(int id)
        {
            ItemId = id;
            Reagents = new Dictionary<WowItem, int>();
        }

        public string PromptModifier(string prompt)
        {
            return "{recipe}" + prompt;
        }
    }
}
