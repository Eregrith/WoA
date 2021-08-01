using System.Collections.Generic;

namespace WoA.Lib.Blizzard
{
    public interface IBlizzardClient
    {
        void LoadAuctions();
        List<Auction> Auctions { get; }

        CharacterProfile GetInfosOnCharacter(string characterName, string realm);
        WowQualityType GetQuality(string itemId);
        WowItem GetItem(string itemId);
        IEnumerable<WowItem> GetItemsWithNameLike(string partialItemName);
        ConnectedRealmSearchData SearchConnectedRealmsForEnglishName(string realmName);
        string GetItemIdFromName(string line);
    }
}