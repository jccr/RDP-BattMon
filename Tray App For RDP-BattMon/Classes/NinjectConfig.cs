using FieldEffect.Interfaces;
using FieldEffect.Models;
using FieldEffect.Presenters;
using FieldEffect.VCL.Server;
using FieldEffect.VCL.Server.Interfaces;
using FieldEffect.Views;
using log4net;
using Ninject;
using Ninject.Modules;
using System;
using System.Drawing;
using Ninject.Extensions.Factory;
using System.Windows.Forms;

namespace FieldEffect.Classes
{
    internal class NinjectConfig : NinjectModule
    {
        private static Lazy<IKernel> _instance = new Lazy<IKernel>(()=>
        {
            var kernel = new StandardKernel(new NinjectConfig());

            if (!kernel.HasModule(new FuncModule().Name))
            {
                kernel = new StandardKernel(new NinjectConfig(), new FuncModule());
            }

            return kernel;
        });

        public static IKernel Instance
        {
            get
            {
                return _instance.Value;
            }
        }
        public override void Load()
        {
            KernelInstance.Bind<IBatteryInfo>()
                .To<BatteryInfo>();

            KernelInstance.Bind<IBatteryParameters>()
                .To<BatteryParameters>();

            KernelInstance.Bind<IBatteryParametersFactory>()
                .ToFactory()
                .InSingletonScope();

            KernelInstance.Bind<IBatteryDetailPresenter>()
                .To<BatteryDetailPresenter>()
                .InSingletonScope();

            KernelInstance.Bind<IBatteryDetail>()
                .To<BatteryDetail>()
                .InSingletonScope();

            KernelInstance.Bind<IBatteryIcon>()
                .To<BatteryIcon>()
                .InSingletonScope()
                .WithConstructorArgument("batteryTemplate", new Icon(Properties.Resources.BattLevel, SystemInformation.SmallIconSize))
                .WithConstructorArgument("batteryLevelMask", new Rectangle(3, 8, 18, 9))
                .WithConstructorArgument("batteryOrientation", BatteryIcon.BatteryOrientation.HorizontalL)
                .WithPropertyValue("BatteryLevel", 0);

            KernelInstance.Bind<IBatteryDataRetriever>()
                .To<BatteryDataRetriever>()
                .InSingletonScope();

            KernelInstance.Bind<IRdpServerVirtualChannel>()
                 .To<RdpServerVirtualChannel>()
                 .InSingletonScope()
                 .WithConstructorArgument("channelName","BATTMON");

            KernelInstance.Bind<ILog>().ToMethod(context =>
                LogManager.GetLogger(context.Request.Target.Member.ReflectedType));

            KernelInstance.Bind<System.Timers.Timer>()
                .ToSelf()
                .InSingletonScope()
                .WithPropertyValue("AutoReset", true)
                .WithPropertyValue("Interval", Properties.Settings.Default.PollingInterval * 1000.0);
        }
    }
}
