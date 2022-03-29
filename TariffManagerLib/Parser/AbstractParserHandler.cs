using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TariffManagerLib.Model;

namespace TariffManagerLib.Parser
{
    public abstract class AbstractParserHandler        
    {
        #region Members
        public string CommandHeader { get; set; }
        protected static Serilog.ILogger Log => Serilog.Log.ForContext<AbstractParserHandler>();
        #endregion 
        public abstract void Parse(string data, TariffRow tariffInfo);
    }
}
