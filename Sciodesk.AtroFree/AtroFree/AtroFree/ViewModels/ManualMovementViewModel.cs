using AtroFree.Models;
using AtroFree.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AtroFree.ViewModels
{
    public class ManualMovementViewModel : BaseViewModel
    {
        #region VARIABLES
        public ObservableCollection<string> PairDevices { get; set; }
        private EasyBluetoothDeviceInfo[] Devices { get; set; }

        private readonly IEasyBluetoothService BluetoothService = DependencyService.Get<IEasyBluetoothService>();
        private IEasyBluetoothConnection Connection { get; set; } 
        #endregion

        #region BINDING VARIABLES
        bool _UpDownValue = false;
        public bool UpDownValue
        {
            get => _UpDownValue;
            set => SetProperty(ref _UpDownValue, value);
        }

        double _SliderValue = 0;
        public double SliderValue
        {
            get => _SliderValue;
            set => SetProperty(ref _SliderValue, value);
        }
        #endregion

        #region CONSTUCTOR
        public ManualMovementViewModel()
        {
            Disconnect();

            //Establecer minimos y maximos del slider
            SetValues();

            Connected = false;

            PairDevices = new ObservableCollection<string>();

            BluetoothConnection();

            MessagingCenter.Subscribe<object, bool>(this, "bluetooth", async (sender, args) =>
            {
                await InitLoading();
                if (args)
                    FindDevices();
                else
                    await Application.Current.MainPage.DisplayAlert("Error", "No se encendió el bluetooth, porfavor enciendelo para poder continuar.", "OK");

                MessagingCenter.Unsubscribe<object>(this, "bluetooth");
                await FinishLoading();
            });
        }
        #endregion

        #region ENVIAR VALORES CON EL SLIDER
        //ENVIAR DATOS DEL SLIDER POR BLUETOOTH
        public void SliderValueChange(double newValue, double oldValue)
        {
            SliderValue = newValue;
            if (newValue > oldValue)
            {
                UpDownValue = true;
            }
            else
            {
                UpDownValue = false;
            }
        }

        public ICommand DragCompletedCommand => new Command(async () =>
        {
            if (SliderValue == MinSliderValue)
            {
                int value = (int)Math.Round(SliderValue + 1, 0);
                await Write("m" + "," + MinSliderValue.ToString() + "," + MaxSliderValue.ToString() + "," + value.ToString() + ",");
            }
            else
            {
                int value = (int)Math.Round(SliderValue, 0);
                await Write("m" + "," + MinSliderValue.ToString() + "," + MaxSliderValue.ToString() + "," + value.ToString() + ",");
            }
        }); 
        #endregion

        #region SEND MESSAGE
        private async Task<bool> Write(string value)
        {
            Debug.WriteLine(value + "\r\n");
            bool message = await Connection.Send(value + "\n"); // devuelve verdadero si se envío satisfactoriamente el mensaje.
            if (!message)
            {
                //await Application.Current.MainPage.DisplayAlert("Error de comunicación", "No se logró enviar información a la báscula.", "OK");
                Disconnect();
            }
            else
            {
                //Se envió exitosamente
            }
            return message;
        }
        #endregion

        #region  BLUETOOTH CONNECTION
        private bool BluetoothConnection()
        {
            if (!BluetoothService.IsEnabled) // ¿El usuario ha activado el bluetooth?
            {
                Task.Run(async () =>
                {
                    return await BluetoothService.RequestEnable(); // Si no, entonces pedimos al usuario que lo active.
                });
            }
            else
            {
                FindDevices();
                return true;
            }

            return false;
        }

        private void FindDevices()
        {
            try
            {
                Devices = BluetoothService.GetPairedDevices();
                PairDevices.Clear();
                foreach (var item in Devices)
                {
                    PairDevices.Add(item.Name);
                }
            }
            catch (Exception)
            {
                Application.Current.MainPage.DisplayAlert("Error", "Ocurrió un problema al ver los dispositivos emparejados", "OK");
            }
        }
        #endregion

        #region CONECTAR CON DISPOSITIVO
        public ICommand ConnectCommand => new Command(async () => await ConectWithDevice());

        private async Task ConectWithDevice()
        {
            try
            {
                await InitLoading();

                SetValues();
                SetValuesOnDay(MaxSliderValue, MinSliderValue);
                EasyBluetoothDeviceInfo deviceSelected = Devices[SelectedIndex];
                Connection = await BluetoothService.Connect(deviceSelected.MAC);
                if (Connection.IsConnected)
                {
                    Connected = true;
                    Debug.WriteLine(MinSliderValue + " " + MaxSliderValue);
                    await Application.Current.MainPage.DisplayAlert("Conexión Exitosa", "Se estableció una conexión con tu dispositivo", "OK");
                    await Write("m" + "," + MinSliderValue.ToString() + "," + MaxSliderValue.ToString() + ",");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error","No se estableció conexión con el dispositivo", "OK");
                }

                await FinishLoading();
            }
            catch (Exception)
            {
                await FinishLoading();
                await Application.Current.MainPage.DisplayAlert("Error", "No se estableció conexión con el dispositivo.", "OK");
            }
        }
        #endregion

        #region SETEAR VALORES DE MAXIMOS Y MINIMOS
        private void SetValues()
        {
            var defaultValues = CurrentDefaultValues.Values;
            if (defaultValues != null)
            {
                MinSliderValue = defaultValues.Min;
                MaxSliderValue = defaultValues.Max;
            }
        }
        #endregion

        #region COMANDO DE DESCONECTAR BLUETOOTH
        public ICommand DisconnectCommand => new Command(async () =>
        {
            Disconnect();
            await Application.Current.MainPage.DisplayAlert("Desconexión Exitosa", "Se logró desconectar el dispositivo con exito.", "OK");
        }); 
        #endregion

        #region READ MESSAGES
        private void Read()
        {
            Connection.NewMessage += (mensaje) =>
            {
                Thread.Sleep(1400);
                System.Diagnostics.Debug.WriteLine("Mensaje: " + mensaje);
                try
                {
                    //TODO Hacer codigo para leer del micro
                }
                catch (Exception)
                {
                    //...
                }
            };
        }
        #endregion

        #region DISCONNECT
        private void Disconnect()
        {
            try
            {
                //Regresar a estado inicial
                Connection.Disconnect();
                Connected = false;
            }
            catch (Exception)
            {
                //...
            }
        }
        #endregion
    }
}
