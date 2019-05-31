using MediatR;

namespace WoA.Lib.Commands.QueryObjects
{
    public class ResetCommand : INotification
    {
        public string UserInput { get; set; }
    }
}
