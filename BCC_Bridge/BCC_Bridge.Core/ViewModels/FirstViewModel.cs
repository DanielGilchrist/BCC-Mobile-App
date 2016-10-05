using MvvmCross.Core.ViewModels;

namespace BCC_Bridge.Core.ViewModels
{
    public class FirstViewModel : MvxViewModel
    {
        public IMvxCommand MapsBtn { get { return ShowCommand<MapViewModel>(); } }
        public IMvxCommand VehicleBtn { get { return ShowCommand<VehicleListViewModel>(); } }

        private MvxCommand ShowCommand<TViewModel>() where TViewModel : IMvxViewModel
        {
            return new MvxCommand(() => ShowViewModel<TViewModel>());
        }
    }
}
