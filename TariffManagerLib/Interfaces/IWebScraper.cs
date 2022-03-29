using TariffManagerLib.Model;

namespace TariffManagerLib.Interfaces
{
    public interface IWebScraper
    {
        bool LoadURL(string path);
        DateTime? ScrapDate();
        public ExcelOutput ScrapTable();
    }
}
