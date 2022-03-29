using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using TariffManagerLib.Helpers;
using TariffManagerLib.Interfaces;
using TariffManagerLib.Model;

namespace TariffManagerLib.Services
{
    public class DBService : IDBService
    {
        #region  Members
        private string _connectionString;
        private List<DateTime> _datesFromDBAfterNow;
        private int _maxTouIdNumber=0;
        private static Serilog.ILogger Log => Serilog.Log.ForContext<DBService>();
        #endregion
        #region Construnctor

        public DBService()
        {
            _connectionString = Helper.Instance.ReadSetting("ConnectionString"); 
        }
        #endregion

        #region Private Methods

        private bool TariffCreate(TariffSeason tariffSeason)
        {
            Log.Here().Information($"Tariff  create for {tariffSeason.SeasonInfo.Season}");
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;
                        try
                        {
                            foreach (List<TariffPeriodInfo> tariffs in tariffSeason.DaysTariffInfo.Values)
                            {

                                bool isFirst = true;
                                ++_maxTouIdNumber;
                                foreach (TariffPeriodInfo tariffPeriodInfo in tariffs.OrderBy(x => x.TimePeriod.Start))
                                {
                                    if (isFirst)
                                    {
                                        command.Parameters.Clear();
                                        command.CommandType = CommandType.Text;
                                        command.CommandText = " INSERT INTO tTimeOfUse ( ConsumerTypeId, DayTypeId, TouDate, TouId, TouSeason) " +
                                                              " VALUES( @ConsumerTypeId , @DayType , @TouDate , @TouId , @SeasonName) ";
                                        command.Parameters.Add("@ConsumerTypeId", SqlDbType.TinyInt).Value = 2;
                                        command.Parameters.Add("@DayType", SqlDbType.TinyInt).Value = (int)tariffPeriodInfo.DayType;
                                        command.Parameters.Add("@SeasonName", SqlDbType.NVarChar).Value = tariffSeason.SeasonInfo.Season;
                                        command.Parameters.Add("@TouDate", SqlDbType.SmallDateTime).Value = tariffSeason.SeasonInfo.LastDate;
                                        command.Parameters.Add("@TouId", SqlDbType.Int).Value = _maxTouIdNumber;
                                        command.ExecuteNonQuery();
                                        isFirst = false;
                                    }
                                    command.Parameters.Clear();
                                    command.CommandText = "spCreateTariff";
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.Add("@DayType", SqlDbType.TinyInt).Value = (int)tariffPeriodInfo.DayType;
                                    command.Parameters.Add("@StartPeriod", SqlDbType.Time).Value = tariffPeriodInfo.TimePeriod.Start.TimeOfDay;
                                    command.Parameters.Add("@TariffPrice", SqlDbType.Real).Value = tariffPeriodInfo.TariffSmall;
                                    command.Parameters.Add("@TariffName", SqlDbType.NVarChar).Value = Helper.Instance.GetAttributeOfType<EnumMemberAttribute>(tariffPeriodInfo.Level).Value;
                                    command.Parameters.Add("@TouId", SqlDbType.Int).Value = _maxTouIdNumber;
                                    command.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit();
                            Log.Here().Information($"Tariff  create for {tariffSeason.SeasonInfo.Season} successful");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Log.Here().Error($"Tariff create for {tariffSeason.SeasonInfo.Season} problem", ex);
                            return false;
                        }

                    }
                }
            }
            return true;
        }

        private void GetMaxTimeOfUse2Tariff()
        {
            Log.Here().Information("Max TouId  get");
            int maxNumber = 0;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("select max(TouId) FROM tTimeOfUse2Tariff ", connection))
                {
                    command.CommandType = CommandType.Text;
                    try
                    {
                        _maxTouIdNumber = (int)command.ExecuteScalar();
                        Log.Here().Information("Getting Max TouId successful ");

                    }
                    catch (Exception ex)
                    {
                        Log.Here().Error("Getting Max TouId problem", ex);
                    }

                }
            }
        }

        #endregion
        #region IDBService

        public void GetDatesFromDBAftreNow()
        {
            Log.Here().Information("Dates from DB get");
            _datesFromDBAfterNow = new List<DateTime>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("select  DISTINCT TouDate FROM tTimeOfUse WHERE TouDate >= getDate()", connection))
                {
                    command.CommandType = CommandType.Text;
                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var date = reader.GetDateTime(reader.GetOrdinal("TouDate"));
                                _datesFromDBAfterNow.Add(date);
                            }
                        }
                        Log.Here().Information("Getting dates from DB successful");

                    }
                    catch (Exception ex)
                    {
                        Log.Here().Error("Getting date from DB problem", ex);
                    }
                }
            }
           
        }

        public void InsertData(Dictionary<SeasonInfo, TariffSeason> data)
        { 
            Log.Here().Information("Data insert to DB");
            var dt = data.Where(dt => !_datesFromDBAfterNow.Any(after => after == dt.Key.LastDate));
            if(dt.Count() == 0)
            {
                Log.Here().Information("List to Insert data  to DB is empty");
                return;
            }
            GetMaxTimeOfUse2Tariff();
            if(_maxTouIdNumber == 0)
            {
                Log.Here().Error("Max number TouId is not valid");
                return;
            }

            foreach (var tariffSeason in dt.OrderBy(x => x.Key.LastDate))
            {
                if(!TariffCreate(tariffSeason.Value))
                {
                    Log.Here().Error($" Data to DB insert problem");
                    return ;
                }
                
            }
        }
     

        public bool TariffClose(DateTime timeOfUse)
        {
            Log.Here().Information("Closing date in DB");
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;
                        try
                        {
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.CommandText = "spUpdateTariff";
                                       
                                    command.Parameters.Add("@TimeOfUse", SqlDbType.SmallDateTime).Value = timeOfUse;

                            command.ExecuteNonQuery();  
                            transaction.Commit();
                            Log.Here().Information("Closing date in DB successful");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Log.Here().Error("Closing date in DB problem", ex);
                            return false;
                        }
                    }                
            }
            return true;
        }
    }
      
        #endregion
    }
}
