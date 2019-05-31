using Autofac;
using MongoRepository;
using System;
using WoA.Lib;
using WoA.Lib.Blizzard;
using WoA.Lib.TSM;

namespace WorldOfAuctions
{
    class Program
    {
        static void Main(string[] args)
        {
            IContainer container = DependencyConfig();
            WoA woa = container.Resolve<WoA>();
            woa.Run();
        }

        private static IContainer DependencyConfig()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterType<Configuration>().As<IConfiguration>();
            builder.RegisterType<StylizedConsole>().As<IStylizedConsole>();
            builder.Register(c => new TsmClient(
                c.Resolve<IConfiguration>().TsmApiKey,
                new MongoRepository<TsmItem>(c.Resolve<IConfiguration>().MongoUrl),
                new MongoRepository<TsmRealmData>(c.Resolve<IConfiguration>().MongoUrl)
                )
            ).As<ITsmClient>();
            builder.Register(c => new BlizzardClient(
                c.Resolve<IConfiguration>().Blizzard_ClientId,
                c.Resolve<IConfiguration>().Blizzard_ClientSecret,
                c.Resolve<IConfiguration>().CurrentRealm
                )
            ).As<IBlizzardClient>();
            builder.Register(c => new AuctionViewer(
                c.Resolve<IStylizedConsole>(),
                c.Resolve<IConfiguration>().CurrentRealm
                )
            ).As<IAuctionViewer>();
            builder.RegisterType<WoA>().AsSelf();

            return builder.Build();
        }
    }
}