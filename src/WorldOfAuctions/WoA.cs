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
                    Console.ForegroundColor = ConsoleColor.White;
                    _console.Write("> ", System.Drawing.Color.White);
                    line = Console.ReadLine();
                    if (!String.IsNullOrEmpty(line) && line != "exit")
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
