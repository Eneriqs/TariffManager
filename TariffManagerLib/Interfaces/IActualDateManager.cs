using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TariffManagerLib.Interfaces
{
    public interface IActualDateManager
    {
        public void SaveActualDate();
        public bool CheckIfNeedDBUpdate();
    }
}
