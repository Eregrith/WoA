﻿using System;

namespace WoA.Lib.Commands.Attributes
{
    public class WoACommandAttribute : Attribute
    {
        public string RegexToMatch { get; set; }
    }
}