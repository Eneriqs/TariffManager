using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TariffManagerLib.Model
{
    public class ExcelOutput
    {
        public ExcelOutput() 
        {
           Data = new Dictionary<Tuple<int, int>, object>();
        }
        public Dictionary<Tuple<int, int>, object> Data { get; set; }
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }

}
}
