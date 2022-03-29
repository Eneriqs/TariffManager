using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TariffManagerLib.Model
{
    public class TariffSeason
    {
        public SeasonInfo SeasonInfo { get; set; }
        public Dictionary<DayType, List<TariffPeriodInfo>> DaysTariffInfo { get; set; }   
    }
}
