using MediatR;
using System;
using WoA.Lib;
using WoA.Lib.Commands.QueryObjects;

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
                    _console.Write("> ");
                    line = Console.ReadLine();
                    _mediator.Publish(new ParseCommand { UserInput = line });
                } while (line != "exit");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }
    }
}
