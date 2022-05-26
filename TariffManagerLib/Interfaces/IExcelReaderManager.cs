using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TariffManagerLib.Model;

namespace TariffManagerLib.Interfaces
{
    public interface IExcelReaderManager 
    {
        void ReadExelReader();
        DateTime? GetStartDate();
        Dictionary<SeasonInfo, TariffSeason> GetTariffs();

    }
}
