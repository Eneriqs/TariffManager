using TariffManagerLib.Helpers;
using TariffManagerLib.Interfaces;
using TariffManagerLib.Model;

namespace TariffManagerLib.Services
{
    public class ProcessManager
    {
        #region
        private static Serilog.ILogger Log => Serilog.Log.ForContext<ProcessManager>();
        private const string _url  = "https://www-iec-co-il.translate.goog/businessclients/pages/highvoltage.aspx?_x_tr_sl=auto&_x_tr_tl=en&_x_tr_hl=ru&_x_tr_pto=wapp";
        #endregion
        public void Process()
        {
            Log.Here().Information($"Process start");
            ExcelOutput excelOutput;
            DateTime? dateFromURL = null;
            using (IExcelManager excelManager = new ExcelManager())
            {
                WebScraper webScrupper = new WebScraper(excelManager);
                if (!webScrupper.LoadURL(_url)) {
                    return;
                }
                dateFromURL = webScrupper.ScrapDate();
                if (dateFromURL == null)
                {
                    Log.Here().Error("Date scrap problem");
                    return;
                }
                excelOutput = webScrupper.ScrapTable();
                if (excelOutput == null) {
                    Log.Here().Error("Table scrap problem");
                    return;
                }
            }

            IDataConvertor dataConvertor = new DataConvertor();
            dataConvertor.Parser(excelOutput);              
            
            Dictionary<SeasonInfo, TariffSeason>  tariffTable = dataConvertor.GetTariffBySeasons();                       
            IActualDateManager dateEffectStarted = new ActualDateManager(dateFromURL.Value);
            IDBService dBService = new DBService();

            if (dateEffectStarted.CheckIfNeedDBUpdate())
            {
                if (!dBService.TariffClose(dateFromURL.Value))
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
