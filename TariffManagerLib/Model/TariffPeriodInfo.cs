using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TariffManagerLib.Model
{
    
    public class TariffPeriodInfo {
       
        public SeasonInfo SeasonInfo { get; set; }
        public DayType  DayType { get; set; }
        public TariffLevel Level { get; set; }
        public TimePeriod TimePeriod { get; set; }
        public decimal TariffSmall { get; set; }
        public decimal TariffFull { get; set; }        
    }

   
}
