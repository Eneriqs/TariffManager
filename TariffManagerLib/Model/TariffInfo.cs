using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TariffManagerLib.Model
{
    public class TariffInfo
    {
        public TariffInfo()
        {
            TimeInfo = new Dictionary<DayType, List<TimePeriod>>();
        }
        public TariffLevel Level { get; set; }
        public Dictionary<DayType, List<TimePeriod>> TimeInfo { get; set; }
        public decimal TariffSmall { get; set; }
        public decimal TariffFull { get; set; }
    }
}
