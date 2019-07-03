using RestSharp;
using System;
using System.Runtime.Serialization;

namespace WoA.Lib.Blizzard
{
    [Serializable]
    public class BlizzardApiCallException : BlizzardApiException
    {
        public BlizzardApiCallException(IRestResponse response)
            : base(response)
        { }

        public override string Message => "Call to Blizzard API failed. Check your Blizzard ClientId/ClientSecret settings.";
    }
}