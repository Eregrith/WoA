using System.Collections.Generic;

namespace WoA.Lib.Blizzard
{
    public interface IBlizzardClient
    {
        string GetAccessToken();
        string GetAuctionFileUrl(string token);
        List<Auction> GetAuctions(string fileUrl);
    }
}