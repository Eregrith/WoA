using RestSharp;
using System;
using System.Runtime.Serialization;

namespace WoA.Lib.Blizzard
{
    [Serializable]
    public class BlizzardTokenRetrievalException : BlizzardApiException
    {
        public BlizzardTokenRetrievalException(IRestResponse response)
            : base(response)
        { }

        public override string Message => "Could not get access token from blizzard api. Check your Blizzard ClientId/ClientSecret settings.";
    }
}