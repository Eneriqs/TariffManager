using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TariffManagerLib.Helpers;
using TariffManagerLib.Model;

namespace TariffManagerLib.Parser
{
    public class TariffLevelHandler : AbstractParserHandler
    {
        public override void Parse(string data, TariffRow tariffInfo)
        {
            Log.Here().Information($"Tariff level parse {data}");
            data = data.Trim();
            tariffInfo.TariffInfo.Level = Helper.Instance.GetValueFromEnumMember<TariffLevel>(data);
            Log.Here().Information($"Tariff level parse successful");
        }

        public override void Parse(string data)
        {
            Log.Here().Information($"Season parse {data}");
            data = data.Trim();

            if (string.IsNullOrEmpty(data))
            {
                Log.Here().Error($"Season parse problem");
            }
            Result = Helper.Instance.GetValueFromEnumMember<TariffLevel>(data);
            Log.Here().Information($"Parse successful {Result}");
        }
    }
}
