using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardsHandlerServerPart.Configs;
using CardsHandlerServerPart.Interfaces;
using CardsHandlerServerPart.JSON;
using Dapper;
using Newtonsoft.Json;

namespace CardsHandlerServerPart.Data
{
    internal class MSSQLInstance : IDisposable, IGetLastFreeValue, IDBProcessCard
    {
        #region FIELDS

        private const string ConfFilePathDB = @"\\172.16.112.40\share\TymoshchukMN\MSDBconfigFile.json";
        private const string ProcGetMinNumber = "GET_MIN_AVAILABLE_CARD_NUM";
        private static MSSQLInstance _instance;
        private readonly SqlConnection _connection;
        private string _connectionString;

        #endregion FIELDS

        #region CTORs

        private MSSQLInstance(
            string server,
            string userName,
            string dataBase,
            int port,
            string pass)
        {
            _connectionString =
                $"Server={server};Database={dataBase};User Id={userName};Password={pass};";

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

        ~MSSQLInstance()
        {
            _connection.Close();
        }

        #endregion CTORs

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

        public ResultOperations AddBonus(out Card card, int cardNum, int summ)
        {
            throw new NotImplementedException();
        }

        public ResultOperations Charge(out Card card, int cardNum, int summ)
        {
            throw new NotImplementedException();
        }

        public bool CheckIfCardExist(int cardNumber)
        {
            throw new NotImplementedException();
        }

        public bool CheckIfPhone(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public void CreateCard(Card card)
        {
            throw new NotImplementedException();
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

        public void GetLastFreeValue(out int lastFreeVol)
        {
            // минимальный номер карты
            const int minVol = 100000;

            int gottenMinCardNumber = _connection.QueryFirstOrDefault<int>(
                ProcGetMinNumber,
                commandType: CommandType.StoredProcedure);

            if (gottenMinCardNumber == 0)
            {
                lastFreeVol = minVol;
            }
            else
            {
                lastFreeVol = gottenMinCardNumber;
            }
        }
    }
}
