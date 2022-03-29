using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TariffManagerLib.Model
{
    public class SeasonInfo
    {
        public Season   Season { get; set; }
        public MonthPeriod MonthPeriod { get; set; }
        public DateTime? LastDate { get; set; }
    }
}
