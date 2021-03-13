using Newtonsoft.Json;
using Xamarin.Essentials;

namespace AtroFree.Models
{
    public static class CurrentDefaultValues
    {
        private static DefaultValues _values;

        public static DefaultValues Values
        {
            get
            {
                if(_values == null)
                {
                    string rawData = SecureStorage.GetAsync(nameof(DefaultValues)).Result;
                    if (string.IsNullOrEmpty(rawData)) 
                        return null;
                    _values = JsonConvert.DeserializeObject<DefaultValues>(rawData);
                }
                return _values;
            }
            set
            {
                _values = value;
                if(_values != null)
                {
                    string defaultValues = JsonConvert.SerializeObject(_values);
                    SecureStorage.SetAsync(nameof(DefaultValues), defaultValues).Wait();
                }
                else
                {
                    SecureStorage.SetAsync(nameof(DefaultValues), string.Empty).Wait();
                }
            }
        }
    }
}
