

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace TariffManagerLib.Model
{
   
    public class TimePeriod
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        
    }
    public class MonthPeriod
    {
        public MonthName? Start { get; set; }
        public MonthName? End { get; set; }

    }

    public class TariffRow
    {
        public TariffRow()
        {
           TariffInfo = new TariffInfo();
        }
        public List<Season> Seasons { get; set; }       
        public List<MonthPeriod> MonthPeriod { get; set; }    
        public TariffInfo TariffInfo { get; set; }


    }
}
