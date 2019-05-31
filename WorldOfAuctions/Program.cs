using MongoDB.Driver;
using MongoRepository;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using WoA.Lib;
using WoA.Lib.Blizzard;
using WoA.Lib.TSM;

namespace WorldOfAuctions
{
    class Program
    {
        static void Main(string[] args)
        {
            WoA woa = new WoA();
            woa.Run();
        }
    }
}
