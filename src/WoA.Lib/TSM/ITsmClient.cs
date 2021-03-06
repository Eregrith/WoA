﻿namespace WoA.Lib.TSM
{
    public interface ITsmClient
    {
        TsmItem GetItem(int id);
        int GetItemIdFromName(string itemName);
        void RefreshTsmItemsInRepository();
    }
}