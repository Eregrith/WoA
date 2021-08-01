using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;

namespace WoA.Lib.Blizzard
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class WowItem : IIdentifiable
    {
        [PrimaryKey]
        public string id { get; set; }
        public string Id { get => id; set => id = value; }
        [Ignore]
        public WowLocalizedString name { get; set; }
        [Ignore]
        public WowQuality quality { get; set; }
        public int level { get; set; }
        public int required_level { get; set; }
        [Ignore]
        public WowMedia media { get; set; }
        [Ignore]
        public WowItemClass item_class { get; set; }
        [Ignore]
        public WowItemClass item_subclass { get; set; }
        [Ignore]
        public WowInventoryType inventory_type { get; set; }
        public long purchase_price { get; set; }
        public long purchase_quantity { get; set; }
        public long sell_price { get; set; }
        public int max_count { get; set; }
        public bool is_equippable { get; set; }
        public bool is_stackable { get; set; }
        [Ignore]
        public WowLocalizedString description { get; set; }
        private string DebuggerDisplay => $"{name} ({id})";
    }
}
