using TariffManagerLib.Helpers;
using TariffManagerLib.Model;

namespace TariffManagerLib.Parser
{
    public class MonthPeriodHandler : AbstractParserHandler
    {

        #region
        static List<string> _lstNotMonth = new List<string>();
      
        #endregion
        static MonthPeriodHandler() {
            Init();
        }
       
        private static void Init()
        {
            _lstNotMonth.Add("עד");
            _lstNotMonth.Add("\n");
            _lstNotMonth.Add("");
        }

        private MonthName? GetMonth(string monthName) {
            if (string.IsNullOrEmpty(monthName)) {
                Log.Here().Error($"Month parse problem");
                return null;
            }
            return Helper.Instance.GetValueFromEnumMember<MonthName>(monthName.Trim());
        }
        #region AbstractParserHandler
        public override void Parse(string data, TariffRow tariffInfo)
        {
            Log.Here().Information($"Month parse {data}");
            List<MonthPeriod> monthPeriods = new List<MonthPeriod>();            
            var period = data.Trim().Split(' ').Where(ds => !_lstNotMonth.Any(nt => Helper.Instance.Compare(nt, ds))).ToList();
            for (int i = 0; i < period?.Count(); i += 2)
            {
                var start = period[i];
                var end = period[i+1];

                MonthPeriod monthPeriod = new MonthPeriod();
                monthPeriod.Start =GetMonth(start);
                monthPeriod.End =GetMonth(end) ;
                monthPeriods.Add(monthPeriod);
            }
            tariffInfo.MonthPeriod = monthPeriods;
            Log.Here().Information($"Month parse successful");
        }

        public override void Parse(string data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
