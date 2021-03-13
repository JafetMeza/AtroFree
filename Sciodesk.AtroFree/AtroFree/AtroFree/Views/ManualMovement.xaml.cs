using AtroFree.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AtroFree.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManualMovement : ContentPage
    {
        public ManualMovement()
        {
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var viewModel = BindingContext as ManualMovementViewModel;
            viewModel.SliderValueChange(e.NewValue, e.OldValue);
        }
    }
}