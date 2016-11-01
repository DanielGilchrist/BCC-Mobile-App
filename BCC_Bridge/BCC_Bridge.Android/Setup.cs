using Android.Content;
using MvvmCross.Droid.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.Platform;
using MvvmCross.Platform;
using BCC_Bridge.Core.Interfaces;
using BCC_Bridge.Android.Maps;
using BCC_Bridge.Core;
using System.Collections.Generic;
using System.Reflection;
using MvvmCross.Droid.Views;
using MvvmCross.Droid.Shared.Presenter;

namespace BCC_Bridge.Android
{
    public class Setup : MvxAndroidSetup
    {
        public Setup(Context applicationContext) : base(applicationContext)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new App();
        }

		protected override IEnumerable<Assembly> AndroidViewAssemblies => new List<Assembly>(base.AndroidViewAssemblies)
		{
			typeof(global::Android.Support.V7.Widget.Toolbar).Assembly,
		};

		protected override IMvxAndroidViewPresenter CreateViewPresenter()
		{
			var mvxFragmentsPresenter = new MvxFragmentsPresenter(AndroidViewAssemblies);
			Mvx.RegisterSingleton<IMvxAndroidViewPresenter>(mvxFragmentsPresenter);
			return mvxFragmentsPresenter;
		}

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

        protected override void InitializeFirstChance()
        {
            Mvx.LazyConstructAndRegisterSingleton<IGeoCoder, GeoCoder>();
            base.InitializeFirstChance();
        }
    }
}
