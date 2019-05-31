using Autofac;
using MongoRepository;
using System;
using WoA.Lib;
using WoA.Lib.Blizzard;
using WoA.Lib.TSM;

namespace WorldOfAuctions
{
    static class Program
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

            builder.Register(c => new MongoRepository<TsmItem>(c.Resolve<IConfiguration>().MongoUrl)).As<IRepository<TsmItem>>().SingleInstance();
            builder.Register(c => new MongoRepository<TsmRealmData>(c.Resolve<IConfiguration>().MongoUrl)).As<IRepository<TsmRealmData>>().SingleInstance();
            builder.RegisterType<Configuration>().As<IConfiguration>().SingleInstance();
            builder.RegisterType<StylizedConsole>().As<IStylizedConsole>().SingleInstance();
            builder.RegisterType<TsmClient>().As<ITsmClient>().SingleInstance();
            builder.RegisterType<BlizzardClient>().As<IBlizzardClient>().SingleInstance();
            builder.RegisterType<AuctionViewer>().As<IAuctionViewer>().SingleInstance();
            builder.RegisterType<WoA>().AsSelf().SingleInstance();

            return builder.Build();
        }
    }
}