using AtroFree.Models;
using Microcharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace AtroFree.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        #region VARIABLES
        public ObservableCollection<string> Options { get; set; }

        private Chart chart;
        public Chart BarChart
        {
            get => chart;
            set => SetProperty(ref chart, value);
        }

        private bool empty;
        public bool Empty
        {
            get => empty;
            set => SetProperty(ref empty, value);
        }
        #endregion

        #region CONSTRUCTOR
        public StatisticsViewModel()
        {
            Options = new ObservableCollection<string>
            {
                "Lunes",
                "Martes",
                "Miercoles",
                "Jueves",
                "Viernes",
                "Sabado",
                "Domingo",
                "Comparación"
            };
        }
        #endregion

        #region METODO PARA CAMBIAR LA GRAFICA CON EL PICKER
        public void PickerChanged()
        {
            var storeValues = CurrentStoreValues.Values;
            if (storeValues != null)
            {
                var value = Options[SelectedIndex];
                switch (value)
                {
                    case "Lunes":
                        if (storeValues.Monday != null)
                        {
                            MinSliderValue = storeValues.Monday.Min;
                            MaxSliderValue = storeValues.Monday.Max;
                            Empty = false;
                            var values = new List<ChartEntry>()
                            {
                                CreateChartEntry(storeValues.Monday.Min, true),
                                CreateChartEntry(storeValues.Monday.Max)
                            };
                            BarChart = new BarChart() { Entries = values };
                        }
                        else
                        {
                            Empty = true;
                        }
                        break;

                    case "Martes":
                        if (storeValues.Tuesday != null)
                        {
                            MinSliderValue = storeValues.Tuesday.Min;
                            MaxSliderValue = storeValues.Tuesday.Max;
                            Empty = false;
                            var values = new List<ChartEntry>()
                            {
                                CreateChartEntry(storeValues.Tuesday.Min, true),
                                CreateChartEntry(storeValues.Tuesday.Max)
                            };
                            BarChart = new BarChart() { Entries = values };
                        }
                        else
                        {
                            Empty = true;
                        }
                        break;

                    case "Miercoles":
                        if (storeValues.Wednesday != null)
                        {
                            MinSliderValue = storeValues.Wednesday.Min;
                            MaxSliderValue = storeValues.Wednesday.Max;
                            Empty = false;
                            var values = new List<ChartEntry>()
                            {
                                CreateChartEntry(storeValues.Wednesday.Min, true),
                                CreateChartEntry(storeValues.Wednesday.Max)
                            };
                            BarChart = new BarChart() { Entries = values };
                        }
                        else
                        {
                            Empty = true;
                        }
                        break;

                    case "Jueves":
                        if (storeValues.Thursday != null)
                        {
                            MinSliderValue = storeValues.Thursday.Min;
                            MaxSliderValue = storeValues.Thursday.Max;
                            Empty = false;
                            var values = new List<ChartEntry>()
                            {
                                CreateChartEntry(storeValues.Thursday.Min, true),
                                CreateChartEntry(storeValues.Thursday.Max)
                            };
                            BarChart = new BarChart() { Entries = values };
                        }
                        else
                        {
                            Empty = true;
                        }
                        break;

                    case "Viernes":
                        if (storeValues.Friday != null)
                        {
                            MinSliderValue = storeValues.Friday.Min;
                            MaxSliderValue = storeValues.Friday.Max;
                            Empty = false;
                            var values = new List<ChartEntry>()
                            {
                                CreateChartEntry(storeValues.Friday.Min, true),
                                CreateChartEntry(storeValues.Friday.Max)
                            };
                            BarChart = new BarChart() { Entries = values };
                        }
                        else
                        {
                            Empty = true;
                        }
                        break;

                    case "Sabado":
                        if (storeValues.Saturday != null)
                        {
                            MinSliderValue = storeValues.Saturday.Min;
                            MaxSliderValue = storeValues.Saturday.Max;
                            Empty = false;
                            var values = new List<ChartEntry>()
                            {
                                CreateChartEntry(storeValues.Saturday.Min, true),
                                CreateChartEntry(storeValues.Saturday.Max)
                            };
                            BarChart = new BarChart() { Entries = values };
                        }
                        else
                        {
                            Empty = true;
                        }
                        break;

                    case "Domingo":
                        if (storeValues.Sunday != null)
                        {
                            MinSliderValue = storeValues.Sunday.Min;
                            MaxSliderValue = storeValues.Sunday.Max;
                            Empty = false;
                            var values = new List<ChartEntry>()
                            {
                                CreateChartEntry(storeValues.Sunday.Min, true),
                                CreateChartEntry(storeValues.Sunday.Max)
                            };
                            BarChart = new BarChart() { Entries = values };
                        }
                        else
                        {
                            Empty = true;
                        }
                        break;

                    case "Comparación":
                        var tuple = CompareCharts(storeValues);
                        MinSliderValue = tuple.Item1;
                        MaxSliderValue = tuple.Item2;
                        Empty = false;
                        var val = new List<ChartEntry>()
                            {
                                CreateChartEntry(tuple.Item1, true),
                                CreateChartEntry(tuple.Item2)
                            };
                        BarChart = new BarChart() { Entries = val };
                        break;
                }

            }
        } 
        #endregion

        //CREA OBJETOS PARA LOS VALORES DE LA GRAFICA
        private ChartEntry CreateChartEntry(float value, bool first = false)
        {
            SkiaSharp.SKColor color;
            if (first)
            {
                color = SkiaSharp.SKColor.Parse("#FFC000");
            }
            else
            {
                color = SkiaSharp.SKColor.Parse("#92D050");
            }
            return new ChartEntry(value)
            {
                Color = color
            };
        }

        //COMPARAR TODOS LOS VALORES DE LOS DIAS
        private Tuple<int, int> CompareCharts(StoreValues storeValues)
        {
            int minValue = 0;
            int maxValue = 0;
            var minList = new List<int>();
            var maxList = new List<int>();
            foreach (var item in storeValues.GetType().GetProperties())
            {
                var property = GetPropertyValue(storeValues, item.Name);
                if (property != null)
                {
                    var values = property as DefaultValues;
                    minList.Add(values.Min);
                    maxList.Add(values.Max);
                }
            }

            minValue = minList.Min(m => m);
            maxValue = maxList.Max(m => m);
            return Tuple.Create(minValue, maxValue);
        }

        // OBTENER VALOR CON REFLECTION
        public static object GetPropertyValue(object source, string propertyName)
        {
            PropertyInfo property = source.GetType().GetProperty(propertyName);
            return property.GetValue(source, null);
        }
    }
}
