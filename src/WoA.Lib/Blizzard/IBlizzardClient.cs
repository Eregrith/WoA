using System.Collections.Generic;

namespace WoA.Lib.Blizzard
{
    public interface IBlizzardClient
    {
        void LoadAuctions();
        List<Auction> Auctions { get; }

        CharacterProfile GetInfosOnCharacter(string characterName, string realm);
        WowQualityType GetQuality(int itemId);
        WowItem GetItem(int itemId);
        IEnumerable<WowItem> GetItemsWithNameLike(string partialItemName);
        ConnectedRealmSearchData SearchConnectedRealmsForEnglishName(string realmName);
        int GetItemIdFromName(string line);
    }
}