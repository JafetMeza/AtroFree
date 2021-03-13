using AtroFree.Models;
using Xamarin.Forms;

namespace AtroFree
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            var defaultValues = CurrentDefaultValues.Values;
            if(defaultValues == null)
            {
                var defaultVal = new DefaultValues
                {
                    Min = 0,
                    Max = 60,
                    IsDefault = true
                };

                CurrentDefaultValues.Values = defaultVal;
            }

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
