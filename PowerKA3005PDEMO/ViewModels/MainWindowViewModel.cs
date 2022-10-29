using Prism.Mvvm;

namespace PowerKA3005PDEMO.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "PowerController";
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
