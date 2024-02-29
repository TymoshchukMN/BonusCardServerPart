using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using CardsHandlerServerPart.Interfaces;
using Dapper;
using Newtonsoft.Json;

namespace CardsHandlerServerPart.Data
{
    internal class MSSQLInstance : IDisposable, IGetLastFreeValue, IDBProcessCard
    {
        #region FIELDS

        private const string ConfFilePathDB = @"\\172.16.112.40\share\TymoshchukMN\MSDBconfigFile.json";
        private const string ProcGetMinNumber = "GET_MIN_AVAILABLE_CARD_NUM";
        private const string ProcCreateCard = "CREATE_USER";
        private const string ProcAddBonuses = "ADD_BONUSES";
        private const string ProcRemoveBonuses = "REMOVE_BONUSES";
        private const string ProcGetCard = "FIND_CARD_BY_CARDNUM";

        private static MSSQLInstance _instance;
        private readonly SqlConnection _connection;
        private string _connectionString;

        #endregion FIELDS

        #region CTORs

        private MSSQLInstance(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new SqlConnection(_connectionString);

            try
            {
                _connection.Open();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion CTORs

        #region SERVICE

        ~MSSQLInstance()
        {
            _connection.Close();
        }

        public static MSSQLInstance GetInstance()
        {
            if (_instance == null)
            {
                string dbConfigFile = File.ReadAllText(ConfFilePathDB);
                SqlConnectionStringBuilder builder =
                    new SqlConnectionStringBuilder();
                dynamic jsonObj = JsonConvert.DeserializeObject(dbConfigFile);
                builder.DataSource = jsonObj.Server;
                builder.InitialCatalog = jsonObj.Database;
                builder.UserID = jsonObj["User Id"];
                builder.Password = jsonObj.Password;

                _instance = new MSSQLInstance(builder.ConnectionString);
            }

            return _instance;
        }

        public void Dispose()
        {
            _connection.Close();
        }

        #endregion SERVICE

        public void CreateCard(Card card)
        {
            var param = new DynamicParameters();
            param.Add("@cardnumber", card.Cardnumber, DbType.Int32, ParameterDirection.Input);
            param.Add("@expirationDate", card.ExpirationDate, DbType.Date, ParameterDirection.Input);
            param.Add("@ballance", card.Ballance, DbType.Int32, ParameterDirection.Input);
            param.Add("@firstName", card.FirstName, DbType.String, ParameterDirection.Input);
            param.Add("@middleName", card.MiddleName, DbType.String, ParameterDirection.Input);
            param.Add("@lastName", card.LastName, DbType.String, ParameterDirection.Input);
            param.Add("@phoneNumber",card.PhoneNumber, DbType.String, ParameterDirection.Input);

            _connection.Query<Card>(ProcCreateCard, param, commandType: CommandType.StoredProcedure);

            Console.ReadKey();
        }

        public ResultOperations FindCardByCard(out Card card, int number)
        {
            throw new NotImplementedException();
        }

        public ResultOperations FindCardByPhone(out Card card, string number)
        {
            throw new NotImplementedException();
        }

        public ResultOperations GetAllCards(out DataTable dataTable)
        {
            throw new NotImplementedException();
        }

        public ResultOperations GetExpiredCards(out DataTable dataTable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получение последнего доступного номера карты.
        /// </summary>
        /// <param name="lastFreeVol"></param>
        public void GetLastFreeValue(out int lastFreeVol)
        {
            lastFreeVol = _connection.QueryFirstOrDefault<int>(
                ProcGetMinNumber,
                commandType: CommandType.StoredProcedure);
        }

        public ResultOperations AddBonus(out Card card, int cardNum, int summ)
        {
            card = null;

            var parameters = new DynamicParameters();
            parameters.Add("@cardNumber", cardNum, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@SUM", summ, DbType.Int32, ParameterDirection.Input);

            ResultOperations resultOperations =
                (ResultOperations)_connection.QueryFirstOrDefault<int>(
                    ProcAddBonuses, parameters, commandType: CommandType.StoredProcedure);

            if (resultOperations == ResultOperations.None)
            {
                parameters = new DynamicParameters();
                parameters.Add("@cardNumber", cardNum, DbType.Int32, ParameterDirection.Input);
                card = _connection.QueryFirst<Card>(
                    ProcGetCard, parameters, commandType: CommandType.StoredProcedure);
            }

            return resultOperations;
        }

        public ResultOperations Charge(out Card card, int cardNum, int summ)
        {
            throw new NotImplementedException();
        }
    }
}
