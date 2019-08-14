using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using WoA.Lib.Commands.QueryObjects;

namespace WoA.Lib.Business
{
    public class Farmable
    {
        [PrimaryKey]
        public int Id { get; set; }
        public double Quantity { get; set; }
        public TimeFrame TimeFrame { get; set; }
    }
}
