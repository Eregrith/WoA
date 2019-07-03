using Newtonsoft.Json;
using RestSharp;
using System;
using System.Runtime.Serialization;

namespace WoA.Lib.Blizzard
{
    public abstract class BlizzardApiException : Exception
    {
        public IRestResponse Response { get; }
        public WowStatusResponse StatusResponse => JsonConvert.DeserializeObject<WowStatusResponse>(Response.Content);

        protected BlizzardApiException(IRestResponse response)
        {
            Response = response;
        }

        protected BlizzardApiException(string message) : base(message)
        {
        }

        protected BlizzardApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BlizzardApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}