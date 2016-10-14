using MvvmCross.Platform;
using MvvmCross.Platform.IoC;

namespace BCC_Bridge.Core
{
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterAppStart<ViewModels.VehicleListViewModel>();

        }
    }
}
