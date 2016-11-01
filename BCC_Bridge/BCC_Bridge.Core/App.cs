using MvvmCross.Platform.IoC;
using MvvmCross.Core.ViewModels;
using BCC_Bridge.Core.ViewModels;

namespace BCC_Bridge.Core
{
	public class App : MvxApplication
	{
		public override void Initialize()
		{
			base.Initialize();

			CreatableTypes()
				.EndingWith("Service")
				.AsInterfaces()
				.RegisterAsLazySingleton();
			
			InitializeNavigation();
		}

		void InitializeNavigation()
		{
			RegisterAppStart<MainViewModel>();
		}
	}
}
