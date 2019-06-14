using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WorldOfAuctions
{
    [Serializable]
    internal class ConfigurationValidationException : Exception
    {
        private readonly List<string> _configProblems;

        public ConfigurationValidationException()
        {
        }

        public ConfigurationValidationException(List<string> configProblems)
        {
            this._configProblems = configProblems;
        }

        public ConfigurationValidationException(string message) : base(message)
        {
        }

        public ConfigurationValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConfigurationValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string Message => $"Error: One or more things are not configured properly:"
                                            + Environment.NewLine + "     - "
                                            + String.Join(Environment.NewLine + "     - ", _configProblems);
    }
}