using MediatR;
using System;
using System.Diagnostics;
using WoA.Lib;
using WoA.Lib.Blizzard;
using WoA.Lib.Commands.QueryObjects;
using WoA.Lib.TSM;

namespace WorldOfAuctions
{
    public class WoA
    {
        private readonly IStylizedConsole _console;
        private readonly IMediator _mediator;

        public WoA(IStylizedConsole console, IMediator mediator)
        {
            _console = console;
            _mediator = mediator;
        }

        public void Run()
        {
            try
            {
                _mediator.Publish(new StartupCommand());

                string line;
                do
                {
                    _console.WriteLine("Waiting for a command... [flip|see|spy|chrealm|top|whatis|stop]");
                    line = Console.ReadLine();
                    _mediator.Publish(new ParseCommand { UserInput = line });
                } while (line != "stop");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }
    }
}
