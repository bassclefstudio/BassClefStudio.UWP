using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BassClefStudio.UWP.Services.Storage
{
    /// <summary>
    /// Represents a service for storing and retrieving serialized settings values.
    /// </summary>
    public interface ISettingsStorageService
    {
        /// <summary>
        /// Writes and stores an item of type <typeparamref name="T"/> in a specified key in the <see cref="ISettingsStorageService"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object being stored.</typeparam>
        /// <param name="key">The key of the item in the <see cref="ISettingsStorageService"/>.</param>
        /// <param name="value">The value to write to settings.</param>
        void StoreValue<T>(string key, T value);

        /// <summary>
        /// Retrieves a value in the <see cref="ISettingsStorageService"/> with the specified key as a <typeparamref name="T"/> object.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="key">The key of the item in the <see cref="ISettingsStorageService"/>.</param>
        T GetValue<T>(string key);

        /// <summary>
        /// Gets a collection of all settings item keys stored in the <see cref="ISettingsStorageService"/>.
        /// </summary>
        IEnumerable<string> GetKeys();
    }

    public class LocalSettingsStorageService : ISettingsStorageService
    {
        public ApplicationDataContainer SettingsContainer { get; }

        public LocalSettingsStorageService()
        {
            SettingsContainer = ApplicationData.Current.LocalSettings.CreateContainer("LocalSettingsStorage", ApplicationDataCreateDisposition.Always);
        }

        public IEnumerable<string> GetKeys()
        {
            return SettingsContainer.Values.Select(v => v.Key);
        }

        public T GetValue<T>(string key)
        {
            if(GetKeys().Contains(key))
            {
                return JsonConvert.DeserializeObject<T>(SettingsContainer.Values[key].ToString());
            }
            else
            {
                throw new KeyNotFoundException($"Key {key} could not be found in the local settings store.");
            }
        }

        public void StoreValue<T>(string key, T value)
        {
            string vJson = JsonConvert.SerializeObject(value);
            if (GetKeys().Contains(key))
            {
                SettingsContainer.Values[key] = vJson;
            }
            else
            {
                SettingsContainer.Values.Add(key, vJson);
            }
        }
    }

    /// <summary>
    /// Represents extension methods for adding <see cref="ISettingsStorageService"/>s to an <see cref="Autofac"/> container.
    /// </summary>
    public static class SettingsExtensions
    {
        /// <summary>
        /// Adds settings storage support through the <see cref="ISettingsStorageService"/> to the <see cref="ContainerBuilder"/>.
        /// </summary>
        /// <param name="builder">The relevant DI <see cref="ContainerBuilder"/>.</param>
        public static void AddSettingsStorage(this ContainerBuilder builder)
        {
            builder.RegisterType<LocalSettingsStorageService>().AsImplementedInterfaces();
        }
    }
}
