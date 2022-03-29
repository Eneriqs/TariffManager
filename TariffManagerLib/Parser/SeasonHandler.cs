using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TariffManagerLib.Helpers;
using TariffManagerLib.Model;

namespace TariffManagerLib.Parser
{
    internal class SeasonHandler : AbstractParserHandler
    {
        public override void Parse(string data, TariffRow tariffInfo)
        {
            Log.Here().Information($"Season parse {data}");
            List<Season> seasons = new List<Season>();
            data = data.Trim();
            data.Split('/');
            foreach(var value in data.Split('/'))
            {
                var season = Helper.Instance.GetValueFromEnumMember<Season>(value);
                seasons.Add(season);
            }
            tariffInfo.Seasons = seasons;
            Log.Here().Information($"Season parse successful");
        }
    }
}
