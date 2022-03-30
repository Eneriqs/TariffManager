using System.Diagnostics;
using System.Text;
using TariffManagerLib.Helpers;
using TariffManagerLib.Interfaces;
using TariffManagerLib.Model;

namespace TariffManagerLib.Services
{
    public class ProcessManager
    {
        #region
        private static Serilog.ILogger Log => Serilog.Log.ForContext<ProcessManager>();
        private string _edgePath = "";
        private string _filePath = "";
        // private const string _url  = "https://www-iec-co-il.translate.goog/businessclients/pages/highvoltage.aspx?_x_tr_sl=auto&_x_tr_tl=en&_x_tr_hl=ru&_x_tr_pto=wapp";
        #endregion

        private void Test()
        {
            _edgePath = Helper.Instance.ReadSetting("EdgePath");
            //SaveHTML();
            // ProcessStart();
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName =_edgePath ,
                        Arguments = "--headless --disable-gpu --dump-dom  \"https://www-iec-co-il.translate.goog/businessclients/pages/highvoltage.aspx?_x_tr_sl=auto&_x_tr_tl=en&_x_tr_hl=ru&_x_tr_pto=wapp\" ",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        //  CreateNoWindow = true,
                        // WorkingDirectory =@"C:\\Temp\1"
                    }
                };

                process.Start();
                StringBuilder builder = new StringBuilder();
                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();
                    builder.Append(line);
                }

                process.WaitForExit();

                Thread.Sleep(10000);
             
                //this code section write stringbuilder content to physical text file.
                using (StreamWriter swriter = new StreamWriter(_filePath))
                {
                    swriter.Write(builder.ToString());                   
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public void Process()
        {
            Log.Here().Information($"Process start");
           
            ExcelOutput excelOutput;
            DateTime? dateFromURL = null;
            _filePath = Helper.Instance.ReadSetting("FilePath"); ;
            string fileFullPath = Path.Combine(Directory.GetCurrentDirectory(),_filePath);
            Test();

            using (IExcelManager excelManager = new ExcelManager())
            {
                WebScraper webScrupper = new WebScraper(excelManager);
                string urlPath = fileFullPath.Replace(@"\",@"/");
                string url = $"file:///{urlPath}";
                if (!webScrupper.LoadURL(url)) {
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
