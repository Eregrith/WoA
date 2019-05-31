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
        private readonly IBlizzardClient _blizzard;
        private readonly ITsmClient _tsm;
        private readonly IConfiguration _config;
        private readonly IAuctionViewer _auctionViewer;
        private readonly IMediator _mediator;

        public WoA(IConfiguration config, ITsmClient tsm, IBlizzardClient blizzard, IStylizedConsole console, IAuctionViewer auctionViewer, IMediator mediator)
        {
            _config = config;
            _tsm = tsm;
            _blizzard = blizzard;
            _console = console;
            _auctionViewer = auctionViewer;
            _mediator = mediator;
        }

        public void Run()
        {
            try
            {
                _tsm.RefreshTsmItemsInRepository();
                _blizzard.LoadAuctions();

                string line;
                do
                {
                    _console.WriteLine("Waiting for a command... [flip|see|spy|chrealm|top|whatis|stop]");
                    line = Console.ReadLine();
                    if (line.StartsWith("flip "))
                    {
                        _mediator.Publish(new FlipCommand { UserInput = line });
                    }
                    else if (line.StartsWith("see "))
                    {
                        _mediator.Publish(new SeeAuctionsCommand { UserInput = line });
                    }
                    else if (line.StartsWith("spy "))
                    {
                        _mediator.Publish(new SpySellerCommand { UserInput = line });
                    }
                    else if (line.StartsWith("top "))
                    {
                        _mediator.Publish(new ShowTopSellersCommand());
                    }
                    else if (line.StartsWith("whatis "))
                    {
                        _mediator.Publish(new WhatIsItemCommand { UserInput = line });
                    }
                    else if (line.StartsWith("chrealm "))
                    {
                        _mediator.Publish(new ChangeRealmCommand { UserInput = line });
                    }
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
