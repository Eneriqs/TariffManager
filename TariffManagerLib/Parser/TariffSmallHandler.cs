using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TariffManagerLib.Helpers;
using TariffManagerLib.Model;

namespace TariffManagerLib.Parser
{
    internal class TariffSmallHandler : AbstractParserHandler
    {
        public override void Parse(string data, TariffRow tariffInfo)
        {
            Log.Here().Information($"Tariff small parse {data}");
            data = data.Trim();
            if (decimal.TryParse(data, out decimal tariff))
            {
                tariffInfo.TariffInfo.TariffSmall = tariff;
                Log.Here().Information($"Tariff small parse successful");
            }
            else
            {
                Log.Here().Information($"Tariff small is not valid");
            }
        }
    }
}
