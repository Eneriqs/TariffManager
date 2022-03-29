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
    }
}
