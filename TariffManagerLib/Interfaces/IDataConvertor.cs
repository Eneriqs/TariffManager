using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TariffManagerLib.Model;

namespace TariffManagerLib.Interfaces
{
    public interface IDataConvertor
    {
        void Parser(ExcelOutput excelOutput);
 
        Dictionary<SeasonInfo, TariffSeason> GetTariffBySeasons();
    }
}
