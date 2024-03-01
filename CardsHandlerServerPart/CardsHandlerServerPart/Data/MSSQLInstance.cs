using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using CardsHandlerServerPart.Interfaces;
using Dapper;
using Newtonsoft.Json;
using System.Linq;

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
        private const string ProcGetAllCards = "GET_ALL_CARS";
        private const string ProcGetCardByPhone = "FIND_CARD_BY_PHONE";
        private const string ProcGetALLCardByPhone = "FIND_ALL_CARD_BY_PHONE";

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

        public void CreateCard(ref Card card)
        {
            DateTime expirationDate = DateTime.Today.AddYears(1);
            var param = new DynamicParameters();
            param.Add("@cardnumber", card.Cardnumber, DbType.Int32, ParameterDirection.Input);
            param.Add("@expirationDate", expirationDate, DbType.Date, ParameterDirection.Input);
            param.Add("@ballance", card.Ballance, DbType.Int32, ParameterDirection.Input);
            param.Add("@firstName", card.FirstName, DbType.String, ParameterDirection.Input);
            param.Add("@middleName", card.MiddleName, DbType.String, ParameterDirection.Input);
            param.Add("@lastName", card.LastName, DbType.String, ParameterDirection.Input);
            param.Add("@phoneNumber",card.PhoneNumber, DbType.String, ParameterDirection.Input);
            param.Add("@@isActive", card.ActivityFlag, DbType.String, ParameterDirection.Input);

            _connection.Query<Card>(ProcCreateCard, param, commandType: CommandType.StoredProcedure);
            FindCardByCardNum(out card, card.Cardnumber);
        }

        public ResultOperations FindCardByPhone(out Card card, string number)
        {
            card = new Card();
            ResultOperations resultOperations = ResultOperations.None;
            var param = new DynamicParameters();
            param.Add("@phoneNum", number, DbType.String, ParameterDirection.Input);

            IEnumerable<Card> results = _connection.Query<Card>(
                ProcGetCardByPhone, param, commandType: CommandType.StoredProcedure);

            if (results.First().Cardnumber == 0)
            {
                resultOperations = ResultOperations.CardDoesnExist;
            }
            else
            {
                FillCard(ref card, ref results);
            }

            return resultOperations;
        }

        public ResultOperations FindCardByPhone(out DataTable dataTable, string number)
        {
            ResultOperations resultOperations = ResultOperations.None;
            var param = new DynamicParameters();

            param.Add("@phoneNum", number, DbType.String, ParameterDirection.Input);
            dataTable = new DataTable();

            IEnumerable<Card> results =
                _connection.Query<Card>(
                ProcGetALLCardByPhone, param, commandType: CommandType.StoredProcedure);

            if (results.First().PhoneNumber == null)
            {
                resultOperations = ResultOperations.PhoneDoesnEsixt;
            }
            else
            {
                FillCardsDataTable(dataTable, results);
            }

            return resultOperations;
        }

        public ResultOperations FindCardByCardNum(out Card card, int number)
        {
            card = new Card();
            ResultOperations resultOperations = ResultOperations.None;
            var param = new DynamicParameters();
            param.Add("@cardNum", number, DbType.Int32, ParameterDirection.Input);
            IEnumerable<Card> results = _connection.Query<Card>(
                ProcGetCard, param, commandType: CommandType.StoredProcedure);

            if (results.First().Cardnumber == 0)
            {
                resultOperations = ResultOperations.CardDoesnExist;
            }
            else
            {
                FillCard(ref card, ref results);
            }

            return resultOperations;
        }

        private static void FillCardsDataTable(DataTable dataTable, IEnumerable<Card> results)
        {
            dataTable.Columns.Add("Cardnumber", typeof(int));
            dataTable.Columns.Add("ExpirationDate", typeof(DateTime));
            dataTable.Columns.Add("Ballance", typeof(int));
            dataTable.Columns.Add("FirstName", typeof(string));
            dataTable.Columns.Add("MiddleName", typeof(string));
            dataTable.Columns.Add("LastName", typeof(string));
            dataTable.Columns.Add("PhoneNumber", typeof(string));

            foreach (var item in results)
            {
                dataTable.Rows.Add(
                    item.Cardnumber,
                    item.ExpirationDate,
                    item.Ballance,
                    item.FirstName,
                    item.MiddleName,
                    item.LastName,
                    item.PhoneNumber);
            }
        }

        public DataTable GetAllCards()
        {
            DataTable dataTable = new DataTable();

            IEnumerable<Card> results =
                _connection.Query<Card>(
                ProcGetAllCards, commandType: CommandType.StoredProcedure);

            FillCardsDataTable(dataTable, results);

            return dataTable;
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
                parameters.Add("@cardNum", cardNum, DbType.Int32, ParameterDirection.Input);
                card = _connection.QueryFirst<Card>(
                    ProcGetCard, parameters, commandType: CommandType.StoredProcedure);
            }

            return resultOperations;
        }

        public ResultOperations Charge(out Card card, int cardNum, int summ)
        {
            card = null;
            var param = new DynamicParameters();
            param.Add("@cardNumber", cardNum, DbType.Int32, ParameterDirection.Input);
            param.Add("@SUM", summ, DbType.Int32, ParameterDirection.Input);

            ResultOperations resultOperations =
                (ResultOperations)_connection.QueryFirstOrDefault<int>(ProcRemoveBonuses, param, commandType: CommandType.StoredProcedure);

            if (resultOperations == ResultOperations.None)
            {
                param = new DynamicParameters();
                param.Add("@cardNum", cardNum, DbType.Int32, ParameterDirection.Input);
                card = _connection.QueryFirst<Card>(
                    ProcGetCard, param, commandType: CommandType.StoredProcedure);
            }

            return resultOperations;
        }

        private static void FillCard(ref Card card, ref IEnumerable<Card> results)
        {
            foreach (var item in results)
            {
                card.Cardnumber = item.Cardnumber;
                card.ExpirationDate = item.ExpirationDate;
                card.Ballance = item.Ballance;
                card.FirstName = item.FirstName;
                card.MiddleName = item.MiddleName;
                card.LastName = item.LastName;
                card.PhoneNumber = item.PhoneNumber;
                card.ActivityFlag = item.ActivityFlag;
            }
        }
    }
}
