namespace WoA.Lib.TSM
{
    public interface ITsmClient
    {
        TsmItem GetItem(string id);
        string GetItemIdFromName(string itemName);
        void RefreshTsmItemsInRepository();
    }
}