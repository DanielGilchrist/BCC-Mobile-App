using MvvmCross.Core.ViewModels;

namespace BCC_Bridge.Core.ViewModels
{
    public class FirstViewModel : MvxViewModel
    {
        public IMvxCommand MapsBtn { get { return ShowCommand<MapViewModel>(); } }

        private MvxCommand ShowCommand<TViewModel>() where TViewModel : IMvxViewModel
        {
            return new MvxCommand(() => ShowViewModel<TViewModel>());
        }
    }
}
