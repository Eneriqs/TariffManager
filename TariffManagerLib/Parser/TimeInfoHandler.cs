using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TariffManagerLib.Helpers;
using TariffManagerLib.Model;

namespace TariffManagerLib.Parser
{
    public class TimeInfoHandler : AbstractParserHandler
    {
        #region Private Methods
        DayType _dayType;
        #endregion
        public TimeInfoHandler(DayType dayType)
        {
            _dayType = dayType;
        }
        public TimeInfoHandler()  { }
        public override void Parse(string data, TariffRow tariffInfo)
        {
            Log.Here().Information($"Time information parse {data}");
            List<TimePeriod> timePeriods = new List<TimePeriod>();
            data = data.Trim();
            var numbers = Regex.Split(data, @"\D+").Where(x => !string.IsNullOrEmpty(x)).ToList();
            for (int i = 0; i < numbers?.Count(); i += 2)
            {
                int start = int.Parse( numbers[i]);
                var end = int.Parse(numbers[i + 1]);
                DateTime startDate = DateTime.MinValue.AddHours(start);
                TimePeriod timePeriod = new TimePeriod()
                {
                    Start = DateTime.MinValue.AddHours(start),
                    End = DateTime.MinValue.AddHours(end)
                };
                timePeriods.Add(timePeriod);
            }
            tariffInfo.TariffInfo.TimeInfo.Add(_dayType, timePeriods);
            if(_dayType == DayType.Weekend1)
            {
                tariffInfo.TariffInfo.TimeInfo.Add(DayType.SpecialDay, timePeriods);
            }
            if (_dayType == DayType.Weekend2)
            {
                tariffInfo.TariffInfo.TimeInfo.Add(DayType.Holiday, timePeriods);
            }
            Log.Here().Information($"Time information parse successful");

        }

        public override void Parse(string data)
        {
            Log.Here().Information($"Time parse {data}");
            data = data.Trim();
            DateTime time;
            if (DateTime.TryParse(data, out time))
            {
                Log.Here().Information($"The Time is valid: {time} ");
                Result = DateTime.MinValue.AddHours(time.Hour);
            }
            else
            {
                Log.Here().Error($"Unable to parse the time {data}");
            }
        }
    }
}
