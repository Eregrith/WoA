using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoA.Lib
{
    public class AuctionApiResponse
    {
        public List<AuctionFile> files { get; set; }
    }

    public class AuctionFile
    {
        public string url { get; set; }
        public long lastModified { get; set; }
    }
}
