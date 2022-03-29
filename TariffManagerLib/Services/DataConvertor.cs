using System.Data;
using TariffManagerLib.Helpers;
using TariffManagerLib.Interfaces;
using TariffManagerLib.Model;

namespace TariffManagerLib.Services
{
    public class DataConvertor : IDataConvertor
    {
        #region Members
        private static Serilog.ILogger Log => Serilog.Log.ForContext<DataConvertor>();
        List<TariffRow> _tariffRows = new List<TariffRow>();
        Dictionary<SeasonInfo, TariffSeason> _tariffsSeasons = new Dictionary<SeasonInfo,TariffSeason>();

        #endregion

        #region Constructor
        public DataConvertor() {
            _tariffRows= new List<TariffRow>();
        }
        #endregion

        #region Private Methods
        
        private TariffSeason CreateNewDaysTariffDictioanry(SeasonInfo seasonInfo)
        {
            TariffSeason tariffSeason = new TariffSeason();
            tariffSeason.SeasonInfo = seasonInfo;
            Dictionary<DayType, List<TariffPeriodInfo>> daysTariff = new Dictionary<DayType, List<TariffPeriodInfo>>();
          
            daysTariff.Add(DayType.WorkingDay, new List<TariffPeriodInfo>());
            daysTariff.Add(DayType.Weekend1, new List<TariffPeriodInfo>());
            daysTariff.Add(DayType.Weekend2, new List<TariffPeriodInfo>());
            daysTariff.Add(DayType.Holiday, new List<TariffPeriodInfo>());
            daysTariff.Add(DayType.SpecialDay, new List<TariffPeriodInfo>());
            tariffSeason.DaysTariffInfo = daysTariff;
            return tariffSeason;
        }

        private void ConvertorTariffInfoToDaysTariff(TariffInfo tariffInfo, TariffSeason tariffSession)
        {
            foreach (var timeInfo in tariffInfo.TimeInfo)
            {
                List<TimePeriod> timePeriods = timeInfo.Value;
                foreach (var timePeriod in timePeriods)
                {
                    TariffPeriodInfo tariffDayTypeToDB = new TariffPeriodInfo()
                    {
                        TariffSmall = tariffInfo.TariffSmall,
                        TariffFull = tariffInfo.TariffFull,
                        Level = tariffInfo.Level,
                        DayType = timeInfo.Key,
                        TimePeriod = timePeriod,
                        SeasonInfo = tariffSession.SeasonInfo
                    };

                    tariffSession.DaysTariffInfo[timeInfo.Key].Add(tariffDayTypeToDB);
                }
            }
        }

        #endregion

        #region IDataConvertor
        public void Parser(ExcelOutput excelOutput)
        {
            Log.Here().Information("List to seasons List convert start");


            for (int i = 1; i < excelOutput.RowCount; i++)
            {
                TariffRow tariffInfo = new TariffRow();
                for (int j = 0; j < excelOutput.ColumnCount; j++)
                {
                    var keyName = new Tuple<int, int>(0, j);
                    string keyNameValue = excelOutput.Data[keyName].ToString();
                    var key = new Tuple<int, int>(i, j);
                    object value = excelOutput.Data[key];
                    
                    CommandProcessor.Instance.Parse(keyNameValue, value.ToString(), tariffInfo);                  
                }
                _tariffRows.Add(tariffInfo);
            }
        }             
       

        public Dictionary<SeasonInfo, TariffSeason>  GetTariffBySeasons()
        {
            foreach (var row in _tariffRows)
            {
                for (int i = 0; i < row.Seasons.Count; i++)
                {

                    MonthPeriod monthPeriod = row.MonthPeriod[i];
                    Season season = row.Seasons[i];

                    var daysTariff = _tariffsSeasons.Where(x => x.Key.Season == season).FirstOrDefault();                    
                    
                    SeasonInfo seasonInfo = null;
                    if (daysTariff.Key == null)
                    {
                        seasonInfo = new SeasonInfo
                        {
                            Season = season,
                            MonthPeriod = monthPeriod,
                            LastDate = Helper.Instance.GetLastDayOfMonth((int)monthPeriod.End.Value)
                        };
                        _tariffsSeasons.Add(seasonInfo, CreateNewDaysTariffDictioanry(seasonInfo));
                        

                    }
                    TariffSeason tariffSeason = _tariffsSeasons.Where(x => x.Key.Season == season).Select(x=> x.Value).FirstOrDefault();
                    ConvertorTariffInfoToDaysTariff(row.TariffInfo,tariffSeason);
                    Log.Here().Information($"season {tariffSeason.SeasonInfo } parsing successful");
                }
            }
            Log.Here().Information("List to seasons List convert successful");
            return _tariffsSeasons;
        }


        #endregion
    }
}
