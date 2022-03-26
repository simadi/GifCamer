using Prism.Mvvm;

namespace GifCamer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "[江南鹤牌]动图相机1.0";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
