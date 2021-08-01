using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using WoA.Lib.Blizzard;
using WoA.Lib.TSM;

namespace WoA.Lib.Items
{
    public class ItemHelper : IItemHelper
    {
        private readonly IBlizzardClient _blizzard;
        private readonly ITsmClient _tsm;
        private readonly ILog _logger;

        public ItemHelper(IBlizzardClient blizzard, ITsmClient tsm, ILog logger)
        {
            _blizzard = blizzard;
            _tsm = tsm;
            _logger = logger;
        }

        public string GetItemId(string itemName)
        {
            if (int.TryParse(itemName, out int itemId))
            {
                return itemId.ToString();
            }
            string id = GetItemIdFromBlizzard(itemName);
            if (id == null)
                id = GetItemIdFromTsm(itemName);
            return id;
        }

        private string GetItemIdFromBlizzard(string itemName)
        {
            try
            {
                return _blizzard.GetItemIdFromName(itemName);
            }
            catch (Exception e)
            {
                _logger.Error($"Could not get item id from blizzard api for item: [{itemName}]", e);
                return null;
            }
        }

        private string GetItemIdFromTsm(string itemName)
        {
            try
            {
                return _tsm.GetItemIdFromName(itemName);
            }
            catch (Exception e)
            {
                _logger.Error($"Could not get item id from TSM api for item: [{itemName}]", e);
                return null;
            }
        }
    }
}
