using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TariffManagerLib.Model
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Season
    {
        [EnumMember(Value = "קיץ​​")]
        Summer,
        [EnumMember(Value = "חורף​")]
        Winter,
        [EnumMember(Value = "סתיו")]
        Autumn,
        [EnumMember(Value = "אביב")]
        Spring
    }
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MonthName
    {
        [EnumMember(Value = "​ינואר")]
        January=1,
        [EnumMember(Value = "​פברואר")]
        February=2,
        [EnumMember(Value = "מרץ")]
        March=3,
        [EnumMember(Value = "אפריל")]
        April=4,
        [EnumMember(Value = "מאי")]
        May=5,
        [EnumMember(Value = "יוני")]
        June=6,
        [EnumMember(Value = "יולי")]
        July=7,
        [EnumMember(Value = "אוגוסט")]
        August=8,
        [EnumMember(Value = "ספטמבר")]
        September=9,
        [EnumMember(Value = "אקטובר")]
        October=10,
        [EnumMember(Value = "נובמבר")]
        November=11,
        [EnumMember(Value = "דצמבר")]
        December=12
    }
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TariffLevel
    {
        [EnumMember(Value = "פסגה")]
        Top,
        [EnumMember(Value = "גבע")]
        Geva,
        [EnumMember(Value = "שפל")]
        Low
    }
     

    public enum DayType
    {
        WorkingDay = 1,
        Weekend1 = 2,
        Weekend2 = 3,
        Holiday = 4,
        SpecialDay = 5
    }

    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CommandName
    {
        [EnumMember(Value = "​העונה")]
        SeasonCommand,
        [EnumMember(Value = "​החודשים")]
        MonthPeriodCommand,
        [EnumMember(Value = "מקבצי השעות")]
        TariffLevelComand,
        [EnumMember(Value = "​שעות הצריכה בימים א'-ה'")]
        TimeInfoWorkingDayCommand,
        [EnumMember(Value = "שעות הצריכה בימי ו' ובערבי חג​")]
        TimeInfoWeekend1Command,
        [EnumMember(Value = "שעות הצריכה בשבתות ובחגים")]
        TimeInfoWeekend2Command,
        [EnumMember(Value = "מחיר&nbsp;לקוט\"ש באגורותלא כולל מע\"מ​")]
        TariffSmallCommand,
        [EnumMember(Value = "​מחיר לקוט\"ש באגורותכולל מע\"מ")]
        TariffFullCommand
    }
   
    public enum Command
    {
        Season,
        Month,
        TariffLevel,
        TimeInfo,
        TariffSmall,
        TariffFull,
        StartDate

    }
}
