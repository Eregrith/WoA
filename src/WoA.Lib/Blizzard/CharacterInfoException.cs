using System;
using System.Runtime.Serialization;

namespace WoA.Lib.Blizzard
{
    [Serializable]
    internal class CharacterInfoException : Exception
    {
        public CharacterInfoException()
        {
        }

        public CharacterInfoException(string message) : base(message)
        {
        }

        public CharacterInfoException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CharacterInfoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}