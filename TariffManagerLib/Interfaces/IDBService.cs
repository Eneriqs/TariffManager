using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TariffManagerLib.Model;

namespace TariffManagerLib.Interfaces
{
    public interface IDBService
    {
        void InsertData(Dictionary<SeasonInfo, TariffSeason> data);
        void GetDatesFromDBAftreNow();
        bool TariffClose(DateTime timeOfUse);
    }    
}
