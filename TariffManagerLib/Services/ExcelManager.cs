using ClosedXML.Excel;
using TariffManagerLib.Helpers;
using TariffManagerLib.Interfaces;
using TariffManagerLib.Model;

namespace TariffManagerLib.Services
{
    public class ExcelManager : IExcelManager
    {
        #region Members
        private IXLWorksheet _worksheet;
        private XLWorkbook _workbook;
        private string _pathExcel; 
        private static Serilog.ILogger Log => Serilog.Log.ForContext<ExcelManager>();
        #endregion

        #region Constructors
        public ExcelManager()
        {
            _pathExcel = Helper.Instance.ReadSetting("ExcelOutput");
        }

        #endregion
        #region IExcelManager
        public void CreateWorksheets() {
            Log.Here().Information("Create Excel Worksheets");
            _workbook = new XLWorkbook();
            _worksheet = _workbook.Worksheets.Add("Electric");
        }
        public void  CreateCell(int row, int cell, string value)
        {
            _worksheet.Cell(row, cell).Value = value;            
        }
       
        public void Save()
        {
            Log.Here().Information("Create Excel Worksheets");
            try 
            {
                Log.Here().Information($"Save Excel file {_pathExcel}");
                _workbook.SaveAs(_pathExcel);
                Log.Here().Information($"Excel file save successful");
            }
            catch(Exception ex)
            {
                Log.Here().Error($"Create Excel file {_pathExcel} problem", ex);
            }
                     
        }

        public ExcelOutput GetTariffs() {
            Log.Here().Information("Excel to list convert");

            ExcelOutput excelOutput = new ExcelOutput();
                var range = _worksheet.RangeUsed();
                for (int i = 0; i < range.RowCount() ; i++)
                {
                    for (int j = 0; j < range.ColumnCount() ; j++)
                    {
                    excelOutput.Data.Add(new Tuple<int, int>(i, j), _worksheet.Cell(i+1, j+1).Value);
                    }
                }
            excelOutput.ColumnCount = range.ColumnCount();
            excelOutput.RowCount = range.RowCount();
            Log.Here().Information("Excel to list convert successful");
            return excelOutput;
        }
            
        public void Dispose()
        {
            _workbook.Dispose();
            Log.Here().Information("Excel Manager dicpose successful");
        }
        #endregion

    }
}
