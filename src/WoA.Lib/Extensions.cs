using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoA.Lib
{
    public static class Extensions
    {
        public static string ToAuctionTimeString(this string auctionTimeLeft)
        {
            switch (auctionTimeLeft)
            {
                case "VERY_LONG":
                    return "48h";
                case "LONG":
                    return "12h";
                case "MEDIUM":
                    return " 2h";
                case "SHORT":
                    return "30m";
                default:
                    return auctionTimeLeft;
            }
        }

        public static string ToGoldString(this long value)
        {
            StringBuilder goldString = new StringBuilder();
            if (value < 0)
                goldString.Append('-');
            if (value >= 10000)
            {
                if (goldString.Length > 0)
                    goldString.Append(' ');
                goldString.Append(String.Format("{0:N0}", value / 10000));
                goldString.Append("g");
                value -= (value / 10000) * 10000;
            }
            if (value >= 100 || goldString.Length > 0)
            {
                if (goldString.Length > 0)
                    goldString.Append(' ');
                if (value / 100 < 10)
                    goldString.Append('0');
                goldString.Append(value / 100);
                goldString.Append("s");
                value -= (value / 100) * 100;
            }
            if (value > 0 || goldString.Length > 0)
            {
                if (goldString.Length > 0)
                    goldString.Append(' ');
                if (value < 10)
                    goldString.Append('0');
                goldString.Append(value);
                goldString.Append("c");
            }
            else
                goldString.Append("00c");
            return goldString.ToString();
        }

        public static string ToGoldString(this int value)
        {
            return ToGoldString((long)value);
        }
    }
}
