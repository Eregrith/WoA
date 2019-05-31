using MediatR;

namespace WoA.Lib.Commands.QueryObjects
{
    public class ParseCommand : INotification
    {
        public string UserInput { get; set; }
    }
}
