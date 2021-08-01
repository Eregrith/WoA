using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.Auctions
{
    public class ItemBundle : IIdentifiable
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string BundleName { get => Id; set => Id = value; }
        public string ItemsId { get; set; }
        public string ItemsValue { get; set; }
    }
}
