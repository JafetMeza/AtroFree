using AtroFree.Models;
using AtroFree.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AtroFree.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region BINDING VARIABLES
        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        int selectedIndex = 0;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { SetProperty(ref selectedIndex, value); }
        }

        bool _Connected = false;
        public bool Connected
        {
            get => _Connected;
            set => SetProperty(ref _Connected, value);
        }

        int _MaxSliderValue = 0;
        public int MaxSliderValue
        {
            get => _MaxSliderValue;
            set => SetProperty(ref _MaxSliderValue, value);
        }

        int _MinSliderValue = 0;
        public int MinSliderValue
        {
            get => _MinSliderValue;
            set => SetProperty(ref _MinSliderValue, value);
        } 
        #endregion

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INIT & FINISH LOADING
        public async Task InitLoading()
        {
            await PopupNavigation.Instance.PushAsync(new Loading());
        }
        
        public async Task FinishLoading()
        {
            var instance = PopupNavigation.Instance.PopupStack;
            if (instance.Count > 0)
                await PopupNavigation.Instance.PopAllAsync();
        }
        #endregion

        public void SetValuesOnDay(int max, int min)
        {
            var day = DateTime.Now.DayOfWeek;
            var storeValues = CurrentStoreValues.Values;
            if (storeValues == null)
            {
                storeValues = new StoreValues();
            }
            switch (day)
            {
                case DayOfWeek.Monday:
                    var defaultValues = new DefaultValues
                    {
                        Min = min,
                        Max = max,
                        IsDefault = false
                    };
                    storeValues.Monday = defaultValues;
                    CurrentStoreValues.Values = storeValues;
                    break;

                case DayOfWeek.Tuesday:
                    defaultValues = new DefaultValues
                    {
                        Min = min,
                        Max = max,
                        IsDefault = false
                    };
                    storeValues.Tuesday = defaultValues;
                    CurrentStoreValues.Values = storeValues;
                    break;

                case DayOfWeek.Wednesday:
                    defaultValues = new DefaultValues
                    {
                        Min = min,
                        Max = max,
                        IsDefault = false
                    };
                    storeValues.Wednesday = defaultValues;
                    CurrentStoreValues.Values = storeValues;
                    break;

                case DayOfWeek.Thursday:
                    defaultValues = new DefaultValues
                    {
                        Min = min,
                        Max = max,
                        IsDefault = false
                    };
                    storeValues.Thursday = defaultValues;
                    CurrentStoreValues.Values = storeValues;
                    break;

                case DayOfWeek.Friday:
                    defaultValues = new DefaultValues
                    {
                        Min = min,
                        Max = max,
                        IsDefault = false
                    };
                    storeValues.Friday = defaultValues;
                    CurrentStoreValues.Values = storeValues;
                    break;

                case DayOfWeek.Saturday:
                    defaultValues = new DefaultValues
                    {
                        Min = min,
                        Max = max,
                        IsDefault = false
                    };
                    storeValues.Saturday = defaultValues;
                    CurrentStoreValues.Values = storeValues;
                    break;

                case DayOfWeek.Sunday:
                    defaultValues = new DefaultValues
                    {
                        Min = min,
                        Max = max,
                        IsDefault = false
                    };
                    storeValues.Sunday = defaultValues;
                    CurrentStoreValues.Values = storeValues;
                    break;
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
