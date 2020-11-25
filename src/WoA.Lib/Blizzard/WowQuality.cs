using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.Blizzard
{
    public class WowQuality
    {
        public string type { get; set; }
        public string name { get; set; }
        public WowQualityType AsQualityTypeEnum
        {
            get
            {
                switch (type)
                {
                    case "LEGENDARY":
                        return WowQualityType.orange;
                    case "EPIC":
                        return WowQualityType.violet;
                    case "RARE":
                        return WowQualityType.blue;
                    case "UNCOMMON":
                        return WowQualityType.green;
                    case "COMMON":
                        return WowQualityType.white;
                    case "POOR":
                        return WowQualityType.green;
                    default:
                        return WowQualityType.white;
                }
            }
        }
    }
}
