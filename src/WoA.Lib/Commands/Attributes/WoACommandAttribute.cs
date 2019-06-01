using System;

namespace WoA.Lib.Commands.Attributes
{
    public class WoACommandAttribute : Attribute
    {
        public string RegexToMatch { get; set; }
        public string Description { get; set; }
    }
}
