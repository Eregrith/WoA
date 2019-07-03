using RestSharp;
using System;
using System.Runtime.Serialization;

namespace WoA.Lib.Blizzard
{
    [Serializable]
    internal class CharacterInfoException : BlizzardApiException
    {
        public CharacterInfoException(IRestResponse response)
            : base(response)
        { }

        public override string Message => base.StatusResponse.reason;
    }
}