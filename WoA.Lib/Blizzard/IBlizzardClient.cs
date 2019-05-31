using System.Collections.Generic;

namespace WoA.Lib.Blizzard
{
    public interface IBlizzardClient
    {
        void ChangeRealm(string realm);
        string GetAccessToken();
        string GetAuctionFileUrl(string token);
        List<Auction> GetAuctions(string fileUrl);
    }
}