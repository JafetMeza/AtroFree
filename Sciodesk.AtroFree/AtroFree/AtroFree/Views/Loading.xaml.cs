using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace AtroFree.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Loading : PopupPage
    {
        public Loading()
        {
            InitializeComponent();
            CloseWhenBackgroundIsClicked = false;
        }
    }
}