using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WoA.Lib.Business
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ReagentSource
    {
        public string Source { get; set; }
        public long Cost { get; set; }
        public int Quantity { get; set; }
        public long TotalCost { get; set; }
        private string DebuggerDisplay => $"ReagentSource: {Source} for a cost of {Cost} givin {Quantity} items at a total of {TotalCost}";
    }
}
