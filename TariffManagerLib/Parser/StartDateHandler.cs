using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TariffManagerLib.Helpers;
using TariffManagerLib.Model;

namespace TariffManagerLib.Parser
{
    public class StartDateHandler : AbstractParserHandler
    {
        public override void Parse(string data, TariffRow tariffInfo)
        {
            throw new NotImplementedException();
        }

        public override void Parse(string data)
        {
            Log.Here().Information($"Start date parse {data}");
            data = data.Trim();
            DateTime dateStarted;
            if (DateTime.TryParse(data, out dateStarted))
            {
                Log.Here().Information($"The started date is valid: {dateStarted} ");
                Result = dateStarted;
            }
            else
            {
                Log.Here().Error($"Unable to parse the started date {data}");              
            }
        }
    }
}
