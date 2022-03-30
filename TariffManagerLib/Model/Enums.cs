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
       // [EnumMember(Value = "קיץ​​")]
        Summer,
       // [EnumMember(Value = "חורף​")]
        Winter,
        //[EnumMember(Value = "סתיו")]
        Autumn,
        //[EnumMember(Value = "אביב")]
        Spring
    }
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MonthName
    {
        //[EnumMember(Value = "​ינואר")]
        January=1,
        //[EnumMember(Value = "​פברואר")]
        February=2,
        //[EnumMember(Value = "מארס")]
        March=3,
        //[EnumMember(Value = "אפריל")]
        April=4,
        //[EnumMember(Value = "מאי")]
        May=5,
        //[EnumMember(Value = "יוני")]
        June=6,
        //[EnumMember(Value = "יולי")]
        July=7,
        //[EnumMember(Value = "אוגוסט")]
        August=8,
        //[EnumMember(Value = "ספטמבר")]
        September=9,
        //[EnumMember(Value = "אקטובר")]
        October=10,
        //[EnumMember(Value = "נובמבר")]
        November=11,
        //[EnumMember(Value = "דצמבר")]
        December=12
    }
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TariffLevel
    {
        [EnumMember(Value = "פסגה")]
        Summit,
        //[EnumMember(Value = "גבע")]
        Geva,
        //[EnumMember(Value = "שפל")]
        Depression
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
        [EnumMember(Value = "​This season")]
        SeasonCommand,
        [EnumMember(Value = "​The months")]
        MonthPeriodCommand,
        [EnumMember(Value = "From the time files")]
        TariffLevelComand,
        [EnumMember(Value = "Consumption hours on Sundays-Thursdays")]
        TimeInfoWorkingDayCommand,
        [EnumMember(Value = "Consumption hours on Fridays and on the eve of a holiday​")]
        TimeInfoWeekend1Command,
        [EnumMember(Value = "Consumption hours on Saturdays and holidays")]
        TimeInfoWeekend2Command,
        [EnumMember(Value = "Price per kWh in agorot , not including VAT")]
        TariffSmallCommand,
        [EnumMember(Value = "Price per kWh in agorot including VAT")]
        TariffFullCommand

    }
}
