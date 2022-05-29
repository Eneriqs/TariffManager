using Spire.Xls;
using Spire.Xls.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TariffManagerLib.Helpers;
using TariffManagerLib.Interfaces;
using TariffManagerLib.Model;

namespace TariffManagerLib.Services
{
   
    public class ExcelReaderManager : IExcelReaderManager, IDisposable
    {
        #region Members
        private string _pathExcel;
        private static Serilog.ILogger Log => Serilog.Log.ForContext<ExcelReaderManager>();
        private WorksheetsCollection _worksheets;
        private FileStream _fileStream;
        private Workbook _wbFromStream;

        #endregion

        #region Constructor
        public ExcelReaderManager()
        {
            _pathExcel = Helper.Instance.ReadSetting("ExcelInput");
        }
        #endregion

        #region Private Methods
        private TariffSeason GetTariffBySession(Worksheet worksheet)
        {           
            SeasonInfo seasonInfo =  GetSeasonInfo(worksheet);
            var daysTariffInfo = GetDaysTariffInfo(worksheet, seasonInfo);
            return new TariffSeason()
            {
                SeasonInfo = seasonInfo,
                DaysTariffInfo = daysTariffInfo
            }; 
        }
       
        private SeasonInfo GetSeasonInfo(Worksheet worksheet)
        {
            SeasonInfo seasonInfo = new SeasonInfo();
            string seasonName = worksheet.Name;
            seasonInfo.Season = CommandProcessor.Instance.Parse(Command.Season, seasonName);
            string seasonMonthFrom = worksheet.Rows[1].Cells[1].Value;
            string seasonMonthTo = worksheet.Rows[1].Cells[2].Value;
            MonthPeriod monthPeriod = new MonthPeriod()
            {
                Start = CommandProcessor.Instance.Parse(Command.Month, seasonMonthFrom),
                End = CommandProcessor.Instance.Parse(Command.Month, seasonMonthTo)
            } ;
            seasonInfo.MonthPeriod = monthPeriod;
                    
            seasonInfo.LastDate = Helper.Instance.GetLastDayOfMonth((int)seasonInfo.MonthPeriod.End.Value);
            return seasonInfo;
        }

       

        private List<TariffPeriodInfo> GetDataByDayType(Worksheet worksheet, int columnFrom, int columnTo, DayType dayType, SeasonInfo session) {
            
            List<TariffPeriodInfo> tariffs = new List<TariffPeriodInfo>();
            for (int i = 1; i < worksheet.Rows.Count(); i++)
            {
                string timeFrom = worksheet.Rows[i].Cells[columnFrom].Value;
                
                if (string.IsNullOrEmpty(timeFrom))
                {
                    continue;
                }
                string timeTo = worksheet.Rows[i].Cells[columnFrom].Value;
                TariffPeriodInfo tariffPeriodInfo = new TariffPeriodInfo();
                tariffPeriodInfo.DayType = dayType;
                tariffPeriodInfo.SeasonInfo = session;
                tariffPeriodInfo.TariffFull = CommandProcessor.Instance.Parse(Command.TariffFull, worksheet.Rows[i].Cells[4].Value);
                tariffPeriodInfo.TariffSmall = CommandProcessor.Instance.Parse(Command.TariffSmall, worksheet.Rows[i].Cells[5].Value);
                tariffPeriodInfo.Level = CommandProcessor.Instance.Parse(Command.TariffLevel, worksheet.Rows[i].Cells[3].Value);
                tariffPeriodInfo.TimePeriod = new TimePeriod()
                {
                    Start = CommandProcessor.Instance.Parse(Command.TimeInfo, timeFrom),
                    End = CommandProcessor.Instance.Parse(Command.TimeInfo, timeTo)
                };
               
                tariffs.Add(tariffPeriodInfo);               
            }
            return tariffs;
        }
        private Dictionary<DayType, List<TariffPeriodInfo>> GetDaysTariffInfo(Worksheet worksheet, SeasonInfo season)
        {
            Dictionary<DayType, List<TariffPeriodInfo>> keyValuePairs = new Dictionary<DayType,List<TariffPeriodInfo>>();
            List<TariffPeriodInfo> tariffs = GetDataByDayType(worksheet, 6, 7, DayType.WorkingDay, season);
            keyValuePairs.Add(DayType.WorkingDay, tariffs);
            
            tariffs = GetDataByDayType(worksheet, 8, 9, DayType.Weekend1, season);
            keyValuePairs.Add(DayType.Weekend1, tariffs);

            tariffs = GetDataByDayType(worksheet, 8, 9, DayType.SpecialDay, season);
            keyValuePairs.Add(DayType.SpecialDay, tariffs);

            tariffs = GetDataByDayType(worksheet, 10, 11, DayType.Weekend2, season);
            keyValuePairs.Add(DayType.Weekend2, tariffs);

            tariffs = GetDataByDayType(worksheet, 10, 11, DayType.Holiday, season);
            keyValuePairs.Add(DayType.Holiday, tariffs);


            return keyValuePairs;
        }
        #endregion

        public void ReadExelReader()
        {
            if (!File.Exists(_pathExcel)) {
                Log.Here().Information($"File {_pathExcel} not exits ");
            }
            _wbFromStream = new Workbook();

            using (_fileStream = File.OpenRead(_pathExcel))
            {
                _fileStream.Seek(0, SeekOrigin.Begin);
                _wbFromStream.LoadFromStream(_fileStream);
            }              
                
             _worksheets = _wbFromStream.Worksheets;
                //_worksheet = wbFromStream.Worksheets[0];
                
                //_columnCount = _worksheet.Columns.Count();              

           
        }

     
        public void Dispose()
        {
            for (int i=0; i <  _worksheets.Count; i++)
            {
                _worksheets[i].Dispose();
            }       
            _wbFromStream.Dispose();
        }

        public DateTime? GetStartDate()
        {
            string date = _worksheets[0].Rows[1].Cells[0].Value;
            return CommandProcessor.Instance.Parse(Command.StartDate, date);          
        }

        public Dictionary<SeasonInfo, TariffSeason> GetTariffs()
        {
            Dictionary<SeasonInfo,TariffSeason> tariffs = new Dictionary<SeasonInfo,TariffSeason>();
            for (int i = 0; i < _worksheets.Count; i++)
            {
                TariffSeason tariffInfo = GetTariffBySession(_worksheets[i]) ;
                tariffs.Add(tariffInfo.SeasonInfo, tariffInfo);
            }
            return tariffs;
        }

    }
}
