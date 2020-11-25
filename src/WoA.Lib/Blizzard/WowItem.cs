using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;

namespace WoA.Lib.Blizzard
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class WowItem
    {
        [PrimaryKey]
        public int id { get; set; }
        public string name { get; set; }
        public WowQuality quality { get; set; }
        public int level { get; set; }
        public int required_level { get; set; }
        public WowMedia media { get; set; }
        public WowItemClass item_class { get; set; }
        public WowItemClass item_subclass { get; set; }
        public WowInventoryType inventory_type { get; set; }
        public long purchase_price { get; set; }
        public long sell_price { get; set; }
        public int max_count { get; set; }
        public bool is_equippable { get; set; }
        public bool is_stackable { get; set; }
        public string description { get; set; }

        private string DebuggerDisplay => $"{name} ({id})";
    }
}
