using System;
using System.IO;
using System.Threading.Tasks;

namespace AtroFree.Services
{
    public interface IEasyBluetoothService
    {
        /// <summary>
        /// Indica si el dispositivo soporta conexión por Bluetooth.
        /// </summary>
        bool IsSupported { get; }

        /// <summary>
        /// Indica si el bluetooth está habilitado en el dispositivo.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Solicita al usuario que habilite el Bluetooth si lo ha desactivado.
        /// </summary>
        /// <returns>Devuelve verdadero si el usuario aceptó habilitar el Bluetooth.</returns>
        Task<bool> RequestEnable();

        /// <summary>
        /// Conecta al dispositivo (previamente emparejado) con la dirección MAC indicada y devuelve una instancia para controlar la conexión del dispositivo.
        /// </summary>
        /// <param name="mac">La dirección MAC del dispositivo.</param>
        /// <returns>Instancia para controlar la conexión del dispositivo, devuelve nulo si no hay un dispositivo emparejado con la MAC indicada.</returns>
        /// <exception cref="IOException">Si la conexión falla con el dispositivo existente, se arrojará esta excepción.</exception>
        Task<IEasyBluetoothConnection> Connect(string mac);

        /// <summary>
        /// Devuelve la lista de dispositivos sincronizados activos en la red bluetooth.
        /// </summary>
        /// <returns></returns>
        EasyBluetoothDeviceInfo[] GetPairedDevices();
    }

    public class EasyBluetoothDeviceInfo
    {
        public string Name { get; }
        public string MAC { get; }
        public string FirstUUID { get; }

        public EasyBluetoothDeviceInfo(string name, string mac, string uuid)
        {
            Name = name;
            MAC = mac;
            FirstUUID = uuid;
        }
    }

    public interface IEasyBluetoothConnection
    {
        EasyBluetoothDeviceInfo Device { get; }
        bool IsConnected { get; }
        event Action<string> NewMessage;
        event Action<IOException> Disconnected;
        void Disconnect();
        /// <summary>
        /// Envía un mensaje a través del canal de la conexión bluetooth.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="IOException">Devuelve esta excepción si se ha perdido la conexión bluetooth.</exception>
        Task<bool> Send(string message);
    }
}
