using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TariffManagerLib.Helpers;
using TariffManagerLib.Model;

namespace TariffManagerLib.Parser
{
    public class MonthHandler : AbstractParserHandler
    {
        public override void Parse(string data, TariffRow tariffInfo)
        {
            throw new NotImplementedException();
        }

        public override void Parse(string data)
        {
            Log.Here().Information($"Month parse {data}");
            data = data.Trim();

            if (string.IsNullOrEmpty(data))
            {
                Log.Here().Error($"Month parse problem");             
            }
            Result = Helper.Instance.GetValueFromEnumMember<MonthName>(data);
            Log.Here().Information($"Parse successful {Result}");
        }
    }
}
