using System.Collections.Generic;

namespace WoA.Lib.Blizzard
{
    public interface IBlizzardClient
    {
        void LoadAuctions();
        List<Auction> Auctions { get; }
    }
}