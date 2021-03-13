using EasyBluetooth;
using Xamarin.Forms;
using System;
using Android.Content;
using Android.Bluetooth;
using Android.App;
using System.Threading.Tasks;
using System.Collections.Generic;
using Java.Util;
using System.IO;
using System.Threading;
using System.Text;
using Android.Widget;
using AtroFree.Services;

[assembly: Dependency(typeof(EasyBluetoothService))]
namespace EasyBluetooth
{
    [Activity(Name = "Jaltec.BartenderCtrl.MobileApp.Bluetooth")]
    public class EasyBluetoothService : IEasyBluetoothService
    {
        const int REQUEST_ENABLE_CODE = 101;
        readonly BluetoothAdapter mBluetoothAdapter;
        static Activity Ctx;
        static bool isInitialized = false;
        static TaskCompletionSource<bool> CurrentRequestEnablePromise;

        public bool IsSupported {
            get {
                if (!isInitialized)
                    throw new Exception("EasyBluetoothService->IsSuported: El servicio no ha sido inicializado.");
                return mBluetoothAdapter == null;
            }
        }

        public bool IsEnabled => mBluetoothAdapter != null && mBluetoothAdapter.IsEnabled;

        public EasyBluetoothService()
        {
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (mBluetoothAdapter == null)
                throw new Exception("EasyBluetoothService(): El dispositivo no soporta Bluetooth para poder usar el servicio.");
        }

        public static void Init(Activity ctx)
        {
            Ctx = ctx ?? throw new NullReferenceException("EasyBluetoothService->Init(): El contexto debe ser una instancia de MainActivity.");
            isInitialized = true;
        }

        public async Task<bool> RequestEnable()
        {
            if (!isInitialized)
                throw new Exception("EasyBluetoothService->RequestEnable(): El servicio no ha sido inicializado.");

            if (!mBluetoothAdapter.IsEnabled)
            {
                Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                Ctx.StartActivityForResult(enableBtIntent, REQUEST_ENABLE_CODE);

                CurrentRequestEnablePromise = new TaskCompletionSource<bool>();

                return await CurrentRequestEnablePromise.Task;
            }
            else
            {
                Toast.MakeText(Ctx, "Ya esta encendido el bluetooth", ToastLength.Long);
                return false;
            }    
        }

        public static void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            if (requestCode == REQUEST_ENABLE_CODE)
            {
                if (resultCode == Result.Ok)
                    CurrentRequestEnablePromise.TrySetResult(true);
                else
                    CurrentRequestEnablePromise.TrySetResult(false);

                CurrentRequestEnablePromise = null;
            }
        }

        BluetoothDevice FindPairedDevice(string mac)
        {
            ICollection<BluetoothDevice> pairedDevices = mBluetoothAdapter.BondedDevices;

            if (pairedDevices.Count > 0)
            {
                foreach (BluetoothDevice device in pairedDevices)
                {
                    if (device.Address == mac)
                        return device;
                }
            }

            return null;
        }

        UUID FindPairedDeviceFirstUUID(string mac)
        {
            var device = FindPairedDevice(mac);

            if (device == null)
                return null;

            var uuids = device.GetUuids();
            return uuids.Length > 0 ? UUID.FromString(uuids[0].ToString()) : null;
        }

        public async Task<IEasyBluetoothConnection> Connect(string mac)
        {
            var device = FindPairedDevice(mac);
            
            if (device == null)
                return null;

            var uuid = FindPairedDeviceFirstUUID(mac);

            if (uuid == null)
                return null;
            BluetoothSocket socket = device.CreateRfcommSocketToServiceRecord(uuid);
            mBluetoothAdapter.CancelDiscovery();

            try
            {
                System.Diagnostics.Debug.WriteLine("Tratando de conectar");
                await socket.ConnectAsync();
            }
            catch (Java.IO.IOException ex)
            {
                socket.Close();
                System.Diagnostics.Debug.WriteLine("No se puede conectar");
                throw new IOException("Error de conexión bluetooth al dispositivo con MAC: " + mac, ex);
            }

            return new EasyConnection(device, socket) as IEasyBluetoothConnection;
        }

        public EasyBluetoothDeviceInfo[] GetPairedDevices()
        {
            ICollection<BluetoothDevice> pairedDevices = mBluetoothAdapter.BondedDevices;
            List<EasyBluetoothDeviceInfo> easyDevices = new List<EasyBluetoothDeviceInfo>();

            if (pairedDevices.Count > 0)
            {
                foreach (BluetoothDevice device in pairedDevices)
                {
                    easyDevices.Add(new EasyBluetoothDeviceInfo(device.Name, device.Address, FindPairedDeviceFirstUUID(device.Address).ToString()));
                }
            }

            return easyDevices.ToArray();
        }

        private class EasyConnection : IEasyBluetoothConnection
        {
            private readonly BluetoothSocket mSocket;
            private readonly BluetoothDevice mDevice;
            private readonly Stream mInStream;
            private readonly Stream mOutStream;
            private CancellationTokenSource readingCts;

            public EasyBluetoothDeviceInfo Device { get; }
            public bool IsConnected { get; private set; }

            public event Action<string> NewMessage;
            public event Action<IOException> Disconnected;

            public EasyConnection(BluetoothDevice device, BluetoothSocket socket)
            {
                mDevice = device;
                mSocket = socket;
                mInStream = socket.InputStream;
                mOutStream = socket.OutputStream;
                StartReading();

                IsConnected = mSocket.IsConnected;
            }

            public void Disconnect()
            {
                StopReading();
                mSocket.Close();
                IsConnected = false;
                Disconnected?.Invoke(null);
            }

            void Disconnect(IOException ex)
            {
                StopReading();
                mSocket.Close();
                IsConnected = false;
                Disconnected?.Invoke(ex);
            }

            public async Task<bool> Send(string message)
            {
                var bytes = Encoding.ASCII.GetBytes(message);
                try
                {
                    await mOutStream.WriteAsync(bytes, 0, bytes.Length);
                    return true;
                }
                catch (Java.IO.IOException ex)
                {
                    Disconnect(new IOException("La conexión bluetooth con el dispositivo se ha perdido.", ex));
                    return false;
                }
            }

            async void StartReading()
            {
                try { readingCts?.Cancel(); } catch (ObjectDisposedException) { }
                readingCts = new CancellationTokenSource();
                var ct = readingCts.Token;

                byte[] buffer = new byte[1024];
                int bytes;

                try
                {
                    await Task.Factory.StartNew(async () =>
                    {
                        while (true)
                        {
                            try
                            {
                                bytes = await mInStream.ReadAsync(buffer);
                                var message = Encoding.UTF8.GetString(buffer);
                                NewMessage?.Invoke(message);
                            }
                            catch (IOException ex)
                            {
                                Disconnect(ex);
                            }
                        }
                    }, ct, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }
                catch (TaskCanceledException) { }
            }

            void StopReading()
            {
                try { readingCts?.Cancel(); } catch (ObjectDisposedException) { }
            }
        }
    }
}