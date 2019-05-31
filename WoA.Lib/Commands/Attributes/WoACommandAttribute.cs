using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.Commands.Attributes
{
    public class WoACommandAttribute : Attribute
    {
        public string RegexToMatch { get; set; }
    }
}
