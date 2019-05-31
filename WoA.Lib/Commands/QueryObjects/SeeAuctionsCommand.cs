using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib.Commands.QueryObjects
{
    public class SeeAuctionsCommand : INotification
    {
        public string UserInput { get; set; }
    }
}
