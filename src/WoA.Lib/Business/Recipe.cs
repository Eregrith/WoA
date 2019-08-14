using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.Business
{
    public class Recipe
    {
        [PrimaryKey]
        public string Name { get; set; }
        public int ItemId { get; set; }
    }
}
