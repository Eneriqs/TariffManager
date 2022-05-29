using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TariffManagerLib.Model;

namespace TariffManagerLib.Interfaces
{
    public interface ICommandProcess
    {
        void Parse(string commandName, string data, TariffRow tariffInfo);
        dynamic Parse(Command commandName, string data);
    }
}
