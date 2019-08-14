using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using WoA.Lib.Blizzard;

namespace WoA.Lib.Business
{
    public class Reagent
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Recipe { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}
