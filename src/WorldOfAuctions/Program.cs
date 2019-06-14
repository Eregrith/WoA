using Autofac;
using Autofac.Core;
using log4net;
using log4net.Config;
using MediatR;
using System;
using System.Reflection;
using WoA.Lib;
using WoA.Lib.Auctions;
using WoA.Lib.Blizzard;
using WoA.Lib.Commands.QueryObjects;
using WoA.Lib.SQLite;
using WoA.Lib.TSM;

namespace WorldOfAuctions
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            IContainer container = DependencyConfig();
            WoA woa = container.Resolve<WoA>();
            woa.Run();
        }

        private static IContainer DependencyConfig()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterType<Configuration>().As<IConfiguration>().SingleInstance();
            builder.RegisterType<StylizedConsole>().As<IStylizedConsole>().SingleInstance();
            builder.RegisterType<TsmClient>().As<ITsmClient>().SingleInstance();
            builder.RegisterType<BlizzardClient>()
                .WithParameter(new ResolvedParameter(
                       (pi, ctx) => pi.ParameterType == typeof(ILog),
                       (pi, ctx) => ctx.ResolveKeyed<ILog>("application")))
                .As<IBlizzardClient>()
                .SingleInstance();
            builder.RegisterType<AuctionViewer>().As<IAuctionViewer>().SingleInstance();
            builder.RegisterType<GenericRepository>().As<IGenericRepository>().SingleInstance();
            builder.RegisterType<ItemBundler>().As<IItemsBundler>();
            builder.RegisterType<ClipboardManager>().As<IClipboardManager>();
            builder.RegisterType<FlashingUserNotifier>().As<IUserNotifier>();
            builder.RegisterType<Mediator>()
                .As<IMediator>()
                .SingleInstance();
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
            builder.Register(c => LogManager.GetLogger("Commands"))
                    .As<ILog>()
                    .Keyed<ILog>("commands");
            builder.Register(c => LogManager.GetLogger("Application"))
                    .As<ILog>()
                    .Keyed<ILog>("application");
            builder.RegisterAssemblyTypes(typeof(FlipCommand).GetTypeInfo().Assembly)
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterType<WoA>().AsSelf().SingleInstance();

            return builder.Build();
        }
    }
}