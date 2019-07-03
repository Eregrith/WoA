using RestSharp;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WoA.Lib.Blizzard
{
    [Serializable]
    public class AuctionFileRetrievalException : BlizzardApiException
    {
        public AuctionFileRetrievalException(IRestResponse response)
            : base(response)
        {
        }

        public override string Message => "Auctions file retrieval failed. Check your Blizzard ClientId/ClientSecret settings.";
    }
}