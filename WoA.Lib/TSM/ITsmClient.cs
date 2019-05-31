namespace WoA.Lib.TSM
{
    public interface ITsmClient
    {
        TsmItem GetItem(int id, string realm);
        int GetItemIdFromName(string itemName);
        void RefreshTsmItemsInRepository(string realm);
    }
}