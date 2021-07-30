using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WoA.Lib.Blizzard;

namespace WoA.Lib.Business
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Reagent : IIdentifiable
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Recipe { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        private string DebuggerDisplay => $"Reagent {ItemId} : {Quantity} used in recipe {Recipe}";
    }
}
