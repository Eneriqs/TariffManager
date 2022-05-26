using TariffManagerLib.Helpers;
using TariffManagerLib.Interfaces;
using TariffManagerLib.Model;

namespace TariffManagerLib.Services
{
    public class ProcessManager
    {
        #region
        private static Serilog.ILogger Log => Serilog.Log.ForContext<ProcessManager>();
        private const string _url = "https://www-iec-co-il.translate.goog/content/tariffs/contentpages/taozb-gavoaa?_x_tr_sl=auto&_x_tr_tl=en&_x_tr_hl=ru&_x_tr_pto=op,wapp";
        //"https://www-iec-co-il.translate.goog/businessclients/pages/highvoltage.aspx?_x_tr_sl=auto&_x_tr_tl=en&_x_tr_hl=ru&_x_tr_pto=wapp";
        //;"https://www.iec.co.il/content/tariffs/contentpages/taozb-gavoaa";

        private DateTime? _startDate = null;
        #endregion

        private Dictionary<SeasonInfo, TariffSeason> CreateTariffsUrl() {
            ExcelOutput excelOutput;
           
            using (IExcelManager excelManager = new ExcelManager())
            {
                WebScraper webScrupper = new WebScraper(excelManager);
                if (!webScrupper.LoadURL(_url))
                {
                    return null;
                }
                _startDate = webScrupper.ScrapDate();
                if (_startDate == null)
                {
                    Log.Here().Error("Date scrap problem");
                    return null;
                }
                excelOutput = webScrupper.ScrapTable();
                if (excelOutput == null)
                {
                    Log.Here().Error("Table scrap problem");
                    return null;
                }
            }

            IDataConvertor dataConvertor = new DataConvertor();
            dataConvertor.Parser(excelOutput);

           return dataConvertor.GetTariffBySeasons();

        }

        private Dictionary<SeasonInfo, TariffSeason> CreateTariffsExcel()
        {
            Dictionary<SeasonInfo, TariffSeason> tariffs = null;
            using (ExcelReaderManager excelReader = new ExcelReaderManager())
            {
                excelReader.ReadExelReader();
                _startDate = excelReader.GetStartDate();
                if (_startDate == null)
                {
                    Log.Here().Error("Date scrap problem");
                    return null;
                }
                tariffs = excelReader.GetTariffs();
            }
            return tariffs;
        }

        public void Process()
        {
            Log.Here().Information($"Process start");
            var tariffTable = Helper.Instance.IsExcelStarted() ? CreateTariffsExcel() : CreateTariffsUrl();
            IActualDateManager dateEffectStarted = new ActualDateManager(_startDate.Value);
            IDBService dBService = new DBService();

            if (dateEffectStarted.CheckIfNeedDBUpdate())
            {
                if (!dBService.TariffClose(_startDate.Value))
                {
                    return;
                }
            }
            dBService.GetDatesFromDBAftreNow();
            dBService.InsertData(tariffTable);
            dateEffectStarted.SaveActualDate();
            Log.Here().Information($"Process end successful");
        }

    }
}
