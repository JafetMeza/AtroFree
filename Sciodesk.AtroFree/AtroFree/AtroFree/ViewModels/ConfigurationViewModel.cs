﻿using AtroFree.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AtroFree.ViewModels
{
    public class ConfigurationViewModel : BaseViewModel
    {
        #region BINDING VARIABLES
        bool _DefaultData = true;
        public bool DefaultData
        {
            get => _DefaultData;
            set => SetProperty(ref _DefaultData, value);
        }

        string _MinValue = string.Empty;
        public string MinValue
        {
            get => _MinValue;
            set => SetProperty(ref _MinValue, value);
        }

        string _MaxValue = string.Empty;
        public string MaxValue
        {
            get => _MaxValue;
            set => SetProperty(ref _MaxValue, value);
        }
        #endregion

        #region CONSTRUCTOR
        public ConfigurationViewModel()
        {
            var defaultValues = CurrentDefaultValues.Values;
            if (defaultValues != null)
            {
                DefaultData = defaultValues.IsDefault;
                MinValue = defaultValues.Min.ToString();
                MaxValue = defaultValues.Max.ToString();
            }
        } 
        #endregion

        #region GUARDAR CONFIGURACION POR DEFAULT
        public async Task Toogled(bool value)
        {
            if (value)
            {
                var result = await Application.Current.MainPage.DisplayAlert("Guardar configuración por Default", "¿Deseas guardar los cambios?", "Si", "No");
                if (result)
                {
                    var defaultValues = new DefaultValues
                    {
                        Min = 0,
                        Max = 60,
                        IsDefault = true
                    };
                    MinValue = defaultValues.Min.ToString();
                    MaxValue = defaultValues.Max.ToString();
                    CurrentDefaultValues.Values = defaultValues;
                    MessagingCenter.Send<ConfigurationViewModel, bool>(this, "configuration", true);
                }
                else
                {
                    DefaultData = false;
                }
            }
        } 
        #endregion

        #region GUARDAR CONFIGURACIÓN
        public ICommand SaveCommand => new Command(async () =>
        {
            try
            {
                if (string.IsNullOrEmpty(MinValue) || string.IsNullOrEmpty(MaxValue))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Llena los valores minimos y maximos antes de continuar.", "OK");
                }
                else
                {
                    int min = int.Parse(MinValue);
                    int max = int.Parse(MaxValue);
                    if(min < 0 || min > 180 || max < 0 || max > 180)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "El valor mínimo y máximo debe de estar entre los 0 y 180°.", "OK");
                        return;
                    }
                    var result = await Application.Current.MainPage.DisplayAlert("Guardar nueva configuración", "¿Deseas guardar los cambios?", "Si", "No");
                    if (result)
                    {
                        var defaultValues = new DefaultValues
                        {
                            Min = min,
                            Max = max,
                            IsDefault = false
                        };

                        CurrentDefaultValues.Values = defaultValues;
                        await Application.Current.MainPage.DisplayAlert("Datos Guardados", "Se han guardado con exito su nueva configuración", "OK");
                    }
                }
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Debes colocar números solamente", "OK");
            }
        }); 
        #endregion
    }
}