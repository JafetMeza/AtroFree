using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace AtroFree.Models
{
    public static class CurrentStoreValues
    {
        private static StoreValues _values;

        public static StoreValues Values
        {
            get
            {
                if (_values == null)
                {
                    string rawData = SecureStorage.GetAsync(nameof(StoreValues)).Result;
                    if (string.IsNullOrEmpty(rawData))
                        return null;
                    _values = JsonConvert.DeserializeObject<StoreValues>(rawData);
                }
                return _values;
            }
            set
            {
                _values = value;
                if (_values != null)
                {
                    string storeValues = JsonConvert.SerializeObject(_values);
                    SecureStorage.SetAsync(nameof(StoreValues), storeValues).Wait();
                }
                else
                {
                    SecureStorage.SetAsync(nameof(StoreValues), string.Empty).Wait();
                }
            }
        }
    }
}
