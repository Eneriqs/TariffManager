using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace TariffManagerLib.Helpers
{
    public class Helper
    {
        #region Members
        private static Helper _instance;
        private static readonly object _lock = new object();
        private static Serilog.ILogger Log => Serilog.Log.ForContext<Helper>();
        #endregion
        #region Constructor & Instance
        
        private Helper() { }
        #endregion
        public bool Compare(string item1, string item2) {
            item1 = item1.Replace(((char)8203).ToString(), String.Empty);
            item2 = item2.Replace(((char)8203).ToString(), String.Empty);

            if (item1.Length != item2.Length)
            {
                return false;
            }
            return item1.Equals(item2);            
        }
        public static Helper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Helper();
                        }
                    }
                }
                return _instance;
            }
        }


        /// <summary>
        /// Gets the type of the attribute of.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumVal">The enum value.</param>
        /// <returns></returns>
        public T GetAttributeOfType<T>(Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            T result = null;
            if ((attributes.Length > 0))
            {
                result = (T)attributes[0];
            }
            else 
            {
                Log.Here().Error($"Enum {enumVal} parse problem");
            }
            return result;
        }

        public T GetValueFromEnumMember<T>(string value)
        {
            var type = typeof(T);
            if (type.GetTypeInfo().IsEnum)
            {
                foreach (var name in Enum.GetNames(type))
                {
                    var attr = type.GetRuntimeField(name).GetCustomAttribute<EnumMemberAttribute>(true);
                    if (attr != null && Compare(attr.Value, value))
                        return (T)Enum.Parse(type, name);
                }              
               // return default(T);
            }
            Log.Here().Error($"Enum {value} parse problem");
            throw new InvalidOperationException("Not Enum");
        }

        public T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public DateTime GetLastDayOfMonth(int month)
        {
            int year = (month >= DateTime.Now.Month) ? DateTime.Now.Year : DateTime.Now.Year + 1;
            DateTime dateTime = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            return dateTime;
        }

        public string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                var result = appSettings[key] ?? string.Empty;
                if (string.IsNullOrEmpty(result))
                {
                    Log.Here().Error($"Configuration {key} is not exists");
                }
                return result;
            }
            catch (ConfigurationErrorsException ex)
            {
                Log.Here().Error($"Read configuration {key} problem",ex);
            }
            return string.Empty;
        }

        public void AddUpdateAppSettings(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)) {
                Log.Here().Error($"Save configuration problem. Key:{key} or value:{value} is empty");
                return;            
            }

            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings.Count == 0 | settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException ex)
            {
                Log.Here().Error($"Save configuration {key} problem", ex);
            }
        }
    }
}
