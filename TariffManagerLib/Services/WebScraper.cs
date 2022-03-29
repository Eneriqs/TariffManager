using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using ClosedXML.Excel;
using System.Threading.Tasks;
using TariffManagerLib.Interfaces;
using System.Text.RegularExpressions;
using System.Configuration;
using TariffManagerLib.Model;
using TariffManagerLib.Helpers;

namespace TariffManagerLib.Services
{
    public class WebScraper : IWebScraper
    {
        #region Members   
        private HtmlDocument _document;
        private HtmlNode _nodeForm;
        public IExcelManager _excelManager;
        private static Serilog.ILogger Log => Serilog.Log.ForContext<WebScraper>();
        #region Const Forms parameters
        private const string FormName = "aspnetForm";
        private const string TableClassName = "ms-rteTable-default";
        private const string StartPeriodClassName = "ms-rteElement-H2";
     
        private const string pattern = "[0-1]?[0-9].[0-9]{2}.[0-9]{4}";
        #endregion

        #endregion
        #region Constructor
        public WebScraper(IExcelManager excelManager) { 
            _excelManager = excelManager; 
        }
        #endregion
        #region Private Methods

        private DateTime? GetDate(string input) {

            DateTime? date = null;
            try
            {
               var t =  Regex.Matches(input, pattern).FirstOrDefault();
                if (t != null)
                { 
                    date = Convert.ToDateTime(t.Value);
                }
            }
            catch (RegexMatchTimeoutException ex)
            {
                Log.Here().Error($"Date parsing problem. Input: {input}", ex);
            }
            return date;      

        }
        #endregion

        #region IWebScrupper

        public bool LoadURL(string path)
        {
            Log.Here().Information($"URL {path} load");
            
            using (WebClient client = new WebClient())
            {
                try
                {
                    MemoryStream ms = new MemoryStream(client.DownloadData(path));
                    _document = new HtmlDocument();
                    _document.Load(ms, Encoding.UTF8);
                }
                catch(Exception ex)
                {
                    Log.Here().Error($"Load URL {path} problem",ex);
                    return false;
                }               

            }
            Log.Here().Information($" URL load successful");
             _nodeForm = _document.GetElementbyId(FormName);
            if (_nodeForm == null) {
                Log.Here().Error($"Url parsing problem. Form name: {FormName} is not exits");
                return false;
            }
            return true;
        }

        public DateTime? ScrapDate() {
            Log.Here().Information($"Date scrap");
            HtmlNode nodePeriod = _nodeForm.SelectSingleNode($"//h2[@class=\"{StartPeriodClassName}\"]");
            if (nodePeriod == null) 
            {
                Log.Here().Information($"Date parsing problem. Node with class{ StartPeriodClassName} is not exits");
                return null;
            }
            DateTime? timeStarted = GetDate(nodePeriod.InnerText);
            if (timeStarted != null) {
                Log.Here().Information($"Date scrap  {timeStarted.Value} successful");
            }
            return timeStarted;
        }

        public ExcelOutput ScrapTable() {
            Log.Here().Information($"Table scrap");
            var table = _nodeForm.SelectSingleNode($"//table[@class='{TableClassName}']");
            if (table == null) {
                Log.Here().Information($"Table parsing problem. Node with class{ TableClassName} is not exits");
                return  null;
            }
           
            var tableRows = table
                         .Descendants("tr");
            bool isHeader = true;
            int iRow = 1;
            _excelManager.CreateWorksheets();
            foreach (var tableRow in tableRows)
            {  
                int iColumn = 0;
                if (isHeader)
                {
                    var rowHeader = tableRow
                          .Descendants("th");
                    foreach (var cell in rowHeader)
                    {
                        _excelManager.CreateCell(iRow, ++iColumn, cell.InnerText);
                    }
                    isHeader = false;                     
                }

                var rowCells = tableRow
                    .Descendants("td");
                iColumn = 0; 
               
                foreach (var cell in rowCells)
                {
                    _excelManager.CreateCell(iRow, ++iColumn, cell.InnerText);             
                }
                iRow++;
            }
            _excelManager.Save();
            var tariffs = _excelManager.GetTariffs();
            Log.Here().Information($"Table scrap success");
            return tariffs;
        }
        #endregion
    }
}
