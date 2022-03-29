using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TariffManagerLib.Helpers;
using TariffManagerLib.Interfaces;

namespace TariffManagerLib.Services
{
    public class ActualDateManager : IActualDateManager
    {

        #region Members
        private DateTime _dateFromURL;
        private DateTime? _dateFromCongigFile;
        private const string LastEffectFromDateConfigName = "LastActualDate";
        private static Serilog.ILogger Log => Serilog.Log.ForContext<ActualDateManager>();
        #endregion

        #region Constructors
        public ActualDateManager(DateTime dateFromUrl)
        {
            _dateFromURL = dateFromUrl;            
        }

        #endregion
        #region  Private Method
        private DateTime? GetActualDateFromConfigFile() {

            string dateStr = Helpers.Helper.Instance.ReadSetting(LastEffectFromDateConfigName);
            DateTime? date = null;
            try
            {
                date = DateTime.ParseExact(dateStr, "d.M.yyyy", CultureInfo.InvariantCulture);
                Log.Here().Information($"Actual date from config file: {date}");
            }
            catch (FormatException ex)
            {
                Log.Here().Error($"Actual date from config file: {dateStr} is not valid format");
            }
            return date;
        }
        private void SetConfigFile()
        {
            Helpers.Helper.Instance.AddUpdateAppSettings(LastEffectFromDateConfigName, _dateFromURL.ToString("d.M.yyyy"));
        }
        #endregion

        #region IDateEffectStartedManager
        public void SaveActualDate()
        {
            Log.Here().Information("Actual date save ");
            if (_dateFromCongigFile == null || _dateFromCongigFile.Value < _dateFromURL) {
                SetConfigFile();
            }
            Log.Here().Information("Actual date save successful");
        }

        public bool CheckIfNeedDBUpdate() {
            Log.Here().Information("If need DB Update check");
            bool isNeedUpdate = false;
            _dateFromCongigFile = GetActualDateFromConfigFile();
            if(_dateFromCongigFile != null && _dateFromCongigFile.Value < _dateFromURL)
            {
                isNeedUpdate = true;
            }
            Log.Here().Information($"If need DB Update check successful result: {isNeedUpdate}");
            return isNeedUpdate;
        }
        #endregion
    }
}
