using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WoA.Lib.Business
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Recipe
    {
        [PrimaryKey]
        public string Name { get; set; }
        public int ItemId { get; set; }
        private string DebuggerDisplay => $"Recipe {Name} creates {ItemId}";
    }
}
