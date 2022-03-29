using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TariffManagerLib.Model;

namespace TariffManagerLib.Interfaces
{
    public interface IExcelManager : IDisposable
    {
        void CreateWorksheets();
        void CreateCell(int row, int cell, string value);
        void Save();
        ExcelOutput GetTariffs();
    }
}
