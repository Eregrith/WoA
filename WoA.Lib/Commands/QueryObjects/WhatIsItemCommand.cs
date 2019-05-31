using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.Commands.QueryObjects
{
    public class WhatIsItemCommand : INotification
    {
        public string UserInput { get; set; }
    }
}
