using System;
using System.Collections.Generic;
using System.Data;
using CardsHandlerServerPart.Interfaces;
using Dapper;
using Npgsql;

namespace CardsHandlerServerPart.Data
{
    internal class PostgresDB : IDisposable, IGetLastFreeValue, IDBProcessCard
    {
        #region FIELDS

        private static PostgresDB _instance;
        private readonly NpgsqlConnection _connection;
        private readonly string _server;
        private readonly string _dbName;
        private readonly int _port;
        private string _connectionString;

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

        public static PostgresDB GetInstance()
        {
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
                npgsqlCommand.CommandText = "Select max(cardnumber) as cardnumber from cards";
                data = npgsqlCommand.ExecuteReader();

                while (data.Read())
                {
                    lastFreeVol = (int)data["cardnumber"];
                }
            }

            data.Close();
        }

        #region COPIED

        /// <summary>
        /// Проверяем естьли карты с таким номером.
        /// </summary>
        /// <param name="cardNumber">
        /// Номер карты.
        /// </param>
        /// <returns>
        /// bool.
        /// </returns>
        public bool CheckIfCardExist(int cardNumber)
        {
            bool isExist = false;

            var queryArguments = new
            {
                card = cardNumber,
            };

            string sqlCommand = "SELECT COUNT(*) FROM cards WHERE cardnumber = @card";

            int count = _connection.QueryFirstOrDefault<int>(sqlCommand, queryArguments);

            if (count >= 1)
            {
                isExist = true;
            }

            return isExist;
        }

        /// <summary>
        /// Проверяем естьли карты с таким номером.
        /// </summary>
        /// <param name="phoneNumber">
        /// Номер карты.
        /// </param>
        /// <returns>
        /// bool.
        /// </returns>
        public bool CheckIfPhone(string phoneNumber)
        {
            bool isExist = false;

            var queryArguments = new
            {
                phone = phoneNumber,
            };

            string sqlCommand = "SELECT COUNT(*) FROM cards WHERE \"phoneNumber\" = @phone";

            int count = _connection.QueryFirstOrDefault<int>(sqlCommand, queryArguments);

            if (count >= 1)
            {
                isExist = true;
            }

            return isExist;
        }

        /// <summary>
        /// Создание карты.
        /// </summary>
        /// <param name="card">
        /// объект класса бонусной карты.
        /// </param>
        public void CreateCard(Card card)
        {
            DateTime expirationDate = DateTime.Today.AddMonths(12).Date;

            string sqlCommand = $"INSERT INTO public.cards(" +
                $"cardnumber, \"expirationDate\", ballance, " +
                $"\"firstName\", \"middleName\", \"lastName\", " +
                $"\"phoneNumber\")" +
                " VALUES(@cardNum, @expDate, @balance, @firstName, " +
                "@middleName, @lastName, @phone);";

            var queryArguments = new
            {
                expirationDateCol = "expirationDate",
                firstNameCol = "firstName",
                middleNameCol = "middleName",
                lastNameCol = "lastName",
                phoneNumberCol = "phoneNumber",
                cardNum = card.Cardnumber,
                expDate = expirationDate,
                balance = card.Ballance,
                firstName = card.FirstName,
                middleName = card.MiddleName,
                lastName = card.LastName,
                phone = card.PhoneNumber,
            };

            try
            {
                _connection.Execute(sqlCommand, queryArguments);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Поиск карты.
        /// </summary>
        /// <param name="card">Карта.</param>
        /// <param name="number">номер телефона/карты.</param>
        /// <returns>Объект карты.</returns>
        public ResultOperations FindCardByPhone(out Card card, string number)
        {
            ResultOperations resultOperations = ResultOperations.None;

            card = new Card();

            if (CheckIfPhone(number))
            {
                string sqlCommand = $"SELECT * FROM cards WHERE \"phoneNumber\" = @phone";

                IEnumerable<Card> results = _connection.Query<Card>(sqlCommand, new { phone = number });
                FillCard(ref card, ref results);
            }
            else
            {
                resultOperations = ResultOperations.PhoneDoesnEsixt;
            }

            return resultOperations;
        }

        /// <summary>
        /// Поиск карты.
        /// </summary>
        /// <param name="card">Карта.</param>
        /// <param name="number">карты.</param>
        /// <returns>Объект карты.</returns>
        public ResultOperations FindCardByCard(out Card card, int number)
        {
            ResultOperations resultOperations = ResultOperations.None;

            card = new Card();

            if (CheckIfCardExist(number))
            {
                string sqlCommand =
                    $"SELECT * FROM cards WHERE cardnumber = @cardNum";

                IEnumerable<Card> results =
                    _connection.Query<Card>(
                        sqlCommand,
                        new { cardNum = number });

                FillCard(ref card, ref results);
            }
            else
            {
                resultOperations = ResultOperations.CardDoesnExist;
            }

            return resultOperations;
        }

        /// <summary>
        /// Списание.
        /// </summary>
        /// <param name="card">объект карты.</param>
        /// <param name="cardNum">номер карты.</param>
        /// <param name="summ">Сумма к списанию.</param>
        /// <returns>ResultOperations.</returns>
        public ResultOperations Charge(out Card card, int cardNum, int summ)
        {
            ResultOperations resultOperations = ResultOperations.None;

            card = new Card();

            if (CheckIfCardExist(cardNum))
            {
                string sqlCommand =
                    $"SELECT * FROM cards WHERE cardnumber = @cardnumber";

                IEnumerable<Card> results =
                    _connection.Query<Card>(
                        sqlCommand,
                        new { cardNum = cardNum });

                FillCard(ref card, ref results);

                if (card.ExpirationDate < DateTime.Today)
                {
                    resultOperations = ResultOperations.CardExpired;
                }
                else
                {
                    if (card.Ballance - summ < 0)
                    {
                        resultOperations = ResultOperations.ChargeError;
                    }
                    else
                    {
                        int newVol = card.Ballance - summ;
                        UpdateBallance(card, cardNum, out results, newVol);

                        FillCard(ref card, ref results);
                    }
                }
            }
            else
            {
                resultOperations = ResultOperations.CardDoesnExist;
            }

            return resultOperations;
        }

        /// <summary>
        /// Начисление бонусов.
        /// </summary>
        /// <param name="card">объект карты.</param>
        /// <param name="cardNum">Номер карты.</param>
        /// <param name="summ">Сумма к списанию.</param>
        /// <returns>Результат.</returns>
        public ResultOperations AddBonus(out Card card, int cardNum, int summ)
        {
            ResultOperations resultOperations = ResultOperations.None;

            card = new Card();

            if (CheckIfCardExist(cardNum))
            {
                string sqlCommand =
                    $"SELECT * FROM cards WHERE cardnumber = @cardnumber";

                IEnumerable<Card> results =
                    _connection.Query<Card>(
                        sqlCommand,
                        new { cardNum = cardNum });

                FillCard(ref card, ref results);

                if (card.ExpirationDate < DateTime.Today)
                {
                    resultOperations = ResultOperations.CardExpired;
                }
                else
                {
                    int newVol = card.Ballance + summ;
                    UpdateBallance(card, cardNum, out results, newVol);
                    FillCard(ref card, ref results);
                }
            }
            else
            {
                resultOperations = ResultOperations.CardDoesnExist;
            }

            return resultOperations;
        }

        /// <summary>
        /// Получить все карты.
        /// </summary>
        /// <param name="dataTable">Таблица с данными.</param>
        /// <returns>Таблица с картами.</returns>
        public ResultOperations GetAllCards(out DataTable dataTable)
        {
            ResultOperations resultOperations = ResultOperations.None;

            dataTable = new DataTable();

            string sqlCommand =
                    $"SELECT * FROM cards;";

            IEnumerable<Card> results =
                _connection.Query<Card>(
                    sqlCommand);

            dataTable.Columns.Add("Cardnumber", typeof(int));
            dataTable.Columns.Add("ExpirationDate", typeof(DateTime));
            dataTable.Columns.Add("Ballance", typeof(int));
            dataTable.Columns.Add("FirstName", typeof(string));
            dataTable.Columns.Add("MiddleName", typeof(string));
            dataTable.Columns.Add("LastName", typeof(string));
            dataTable.Columns.Add("PhoneNumber", typeof(string));

            // Заполняем DataTable из IEnumerable<T>
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

            return resultOperations;
        }

        /// <summary>
        /// Получить все карты.
        /// </summary>
        /// <param name="dataTable">DataTable.</param>
        /// <returns>Таблица с картами.</returns>
        public ResultOperations GetExpiredCards(out DataTable dataTable)
        {
            ResultOperations resultOperations = ResultOperations.None;

            dataTable = new DataTable();

            string sqlCommand =
                    $"SELECT * FROM cards where \"ExpirationDate\" <  @date;";

            IEnumerable<Card> results =
                _connection.Query<Card>(
                    sqlCommand, new { date = DateTime.Today.Date });

            dataTable.Columns.Add("Cardnumber", typeof(int));
            dataTable.Columns.Add("ExpirationDate", typeof(DateTime));
            dataTable.Columns.Add("Ballance", typeof(int));
            dataTable.Columns.Add("FirstName", typeof(string));
            dataTable.Columns.Add("MiddleName", typeof(string));
            dataTable.Columns.Add("LastName", typeof(string));
            dataTable.Columns.Add("PhoneNumber", typeof(string));

            // Заполняем DataTable из IEnumerable<T>
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
            }
        }

        private void UpdateBallance(Card card, int cardNum, out IEnumerable<Card> results, int newVol)
        {
            string sqlCommand = "UPDATE cards SET ballance = @newBallance WHERE  cardnumber = @cardnumber;";

            _connection.Execute(sqlCommand, new
            {
                newBallance = newVol,
                cardnumber = cardNum,
            });

            sqlCommand =
            $"SELECT ballance FROM cards WHERE cardnumber = @cardnumber";

            results =
                _connection.Query<Card>(
                    sqlCommand,
                    new { cardNum = cardNum });
        }
        #endregion COPIED
    }
}
