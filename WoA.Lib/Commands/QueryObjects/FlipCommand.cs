using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using WoA.Lib.Commands.Attributes;

namespace WoA.Lib.Commands.QueryObjects
{
    public class FlipCommand : INotification
    {
        public string UserInput { get; set; }
    }
}
