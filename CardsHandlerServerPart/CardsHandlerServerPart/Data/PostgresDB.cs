using System;
using System.Data;
using Npgsql;

namespace CardsHandlerServerPart.Data
{
    internal class PostgresDB : IDisposable
    {
        #region FIELDS

        private static PostgresDB _instance;
        private readonly NpgsqlConnection _connection;

        private string _connectionString;
        private string _server;
        private string _dbName;
        private int _port;

        #endregion FIELDS

        #region CTORs

        private PostgresDB(
            string server,
            string userName,
            string dataBase,
            int port)
        {
            _connectionString = string.Format(
                    $"Server={server};" +
                    $"Username={userName};" +
                    $"Database={dataBase};" +
                    $"Port={port};" +
                    $"Password={string.Empty}");
            _server = server;
            _dbName = dataBase;
            _port = port;

            _connection = new NpgsqlConnection(_connectionString);
            try
            {
                _connection.Open();
            }
            catch (Exception)
            {
                throw;
            }
        }

        ~PostgresDB()
        {
            _connection.Close();
        }

        #endregion CTORs

        public static PostgresDB GetInstance(
            string server,
            string userName,
            string dataBase,
            int port)
        {
            if (_instance == null)
            {
                _instance = new PostgresDB(
                    server,
                    userName,
                    dataBase,
                    port);
            }

            return _instance;
        }

        public void Dispose()
        {
            _connection.Close();
        }

        /// <summary>
        /// Получаем последний свободный номер карты.
        /// </summary>
        /// <param name="lastFreeVol">
        /// Последний номер карты.
        /// </param>
        public void GetLastFreeValue(out int lastFreeVol)
        {
            lastFreeVol = 0;
            NpgsqlCommand npgsqlCommand = _connection.CreateCommand();

            npgsqlCommand.CommandText = $"SELECT cardnumber FROM CARDS";

            NpgsqlDataReader data;
            data = npgsqlCommand.ExecuteReader();

            DataTable dataTable = new DataTable();
            dataTable.Load(data);

            if (dataTable.Rows.Count != 0)
            {
                data.Close();
                npgsqlCommand.CommandText = "Select max(cardnumber) from cards";
                data = npgsqlCommand.ExecuteReader();

                while (data.Read())
                {
                    lastFreeVol = (int)data["cardnumber"];
                }
            }

            data.Close();
        }
    }
}
