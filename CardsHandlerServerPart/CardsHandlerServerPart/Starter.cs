using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CardsHandlerServerPart.Configs;
using CardsHandlerServerPart.Data;
using CardsHandlerServerPart.Enums;
using CardsHandlerServerPart.Interfaces;
using CardsHandlerServerPart.JSON;
using Newtonsoft.Json;

namespace CardsHandlerServerPart
{
    internal class Starter
    {
        public static void Run()
        {
            CardsPool cardsPoll = CardsPool.GetInstance();
            DBConfigJSON dBConfig = BL.GetDBConfig();

            IGetLastFreeValue pgDB = PostgresDB.GetInstance(
               dBConfig.DBConfig.Server,
               dBConfig.DBConfig.UserName,
               dBConfig.DBConfig.DBname,
               dBConfig.DBConfig.Port);
            pgDB.GetLastFreeValue(out int lastFreeVol);
            cardsPoll.FillPool(lastFreeVol);

            // Запуск сервера.
            StartServer();
        }

        /// <summary>
        /// Запуск сервера обработчика пула.
        /// </summary>
        public static void StartServer()
        {
            // Получаем конфиг подключения к серверу.
            SrvConfig srvConfig = BL.GetServerConfig();

            int port = srvConfig.Port;
            string serverAddress = srvConfig.Server;

            IPAddress ipAddress = IPAddress.Parse(serverAddress);
            TcpListener listener = new TcpListener(ipAddress, port);
            CardsPool cardsPoll = CardsPool.GetInstance();
            listener.Start();
            Console.WriteLine("Сервер запущен...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();

                Task.Run(() => ProcessClientRequest(cardsPoll, client));
            }
        }

        private static async Task ProcessClientRequest(CardsPool cardsPoll, TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            byte[] responseData;
            Console.WriteLine($"Получено от клиента: {dataReceived}");

            CardsOperation cardOperation =
                (CardsOperation)Enum.Parse(typeof(CardsOperation), dataReceived.Split(';')[0]);

            IProcessCardsDB pgDB = PostgresDB.GetInstance();

            switch (cardOperation)
            {
                case CardsOperation.Create:

                    #region СОЗДАНИЕ КАРТЫ

                    int cardNumber;

                    // проверяем свободен ли обработчик пула,
                    // если да - полчаем номер карты,
                    // если нет - ждем произвольное время
                    if (!cardsPoll.IsBusy)
                    {
                        Console.WriteLine("Пулл свободен. Получение номера карты...");
                        cardNumber = cardsPoll.GetCarNumber();
                        Console.WriteLine($"Карта получена, номер {cardNumber}");
                    }
                    else
                    {
                        Console.WriteLine("Пулл занят. Получение номера карты...");
                        do
                        {
                            WaitRandomTime();
                        }
                        while (cardsPoll.IsBusy);

                        cardNumber = cardsPoll.GetCarNumber();
                    }

                    string phoneNumber = dataReceived.Split(';')[1];
                    string firstName = dataReceived.Split(';')[2];
                    string middleName = dataReceived.Split(';')[3];
                    string lastName = dataReceived.Split(';')[4];

                    Card card = new Card(
                                cardNumber,
                                phoneNumber,
                                firstName,
                                middleName,
                                lastName);

                    pgDB.CreateCard(card);

                    string json = JsonConvert.SerializeObject(card);
                    responseData = Encoding.UTF8.GetBytes(
                        json.ToString().ToCharArray());
                    stream.Write(responseData, 0, responseData.Length);

                    #endregion СОЗДАНИЕ КАРТЫ

                    break;
                case CardsOperation.Find:

                    #region ПОИСК

                    SearchType searchType =
                        (SearchType)Enum.Parse(
                            typeof(SearchType),
                            dataReceived.Split(';')[1]);

                    ResultOperations resultOperation;

                    switch (searchType)
                    {
                        case SearchType.ByPhone:
                            string phone = dataReceived.Split(';')[2];
                            resultOperation = pgDB.FindCardByPhone(out card, phone);

                            if (resultOperation == ResultOperations.None)
                            {
                                json = JsonConvert.SerializeObject(card);
                            }
                            else
                            {
                                json = resultOperation.ToString();
                            }

                            responseData = Encoding.UTF8.GetBytes(
                                   json.ToString().ToCharArray());
                            stream.Write(responseData, 0, responseData.Length);

                            break;
                        case SearchType.ByCard:

                            int.TryParse(dataReceived.Split(';')[2], out int cardNN);

                            resultOperation = pgDB.FindCardByCard(out card, cardNN);

                            if (resultOperation == ResultOperations.None)
                            {
                                json = JsonConvert.SerializeObject(card);
                            }
                            else
                            {
                                json = resultOperation.ToString();
                            }

                            responseData = Encoding.UTF8.GetBytes(
                                  json.ToString().ToCharArray());
                            stream.Write(responseData, 0, responseData.Length);

                            break;
                    }
                    #endregion ПОИСК

                    break;

                case CardsOperation.Change:

                    #region ИЗМЕНЕНИЕ

                    BonusOperations bonusOperations =
                        (BonusOperations)Enum.Parse(
                            typeof(BonusOperations),
                            dataReceived.Split(';')[1]);

                    int.TryParse(dataReceived.Split(';')[2], out int cardNum);
                    int.TryParse(dataReceived.Split(';')[3], out int summ);

                    switch (bonusOperations)
                    {
                        case BonusOperations.Add:
                            resultOperation =
                                pgDB.AddBonus(out card, cardNum, summ);

                            pgDB.FindCardByCard(out card, cardNum);
                            switch (resultOperation)
                            {
                                case ResultOperations.None:

                                    json = JsonConvert.SerializeObject(card);

                                    responseData = Encoding.UTF8.GetBytes(
                                    json.ToString().ToCharArray());
                                    stream.Write(responseData, 0, responseData.Length);
                                    break;

                                case ResultOperations.CardDoesnExist:

                                    json = resultOperation.ToString();
                                    responseData = Encoding.UTF8.GetBytes(
                                    json.ToString().ToCharArray());
                                    stream.Write(responseData, 0, responseData.Length);
                                    break;

                                case ResultOperations.CardExpired:

                                    json = resultOperation.ToString();
                                    responseData = Encoding.UTF8.GetBytes(
                                    json.ToString().ToCharArray());
                                    stream.Write(responseData, 0, responseData.Length);

                                    break;
                            }

                            break;
                        case BonusOperations.Remove:

                            resultOperation =
                                pgDB.Charge(out card, cardNum, summ);
                            pgDB.FindCardByCard(out card, cardNum);
                            switch (resultOperation)
                            {
                                case ResultOperations.None:

                                    json = JsonConvert.SerializeObject(card);

                                    responseData = Encoding.UTF8.GetBytes(
                                    json.ToString().ToCharArray());
                                    stream.Write(responseData, 0, responseData.Length);
                                    break;

                                case ResultOperations.CardDoesnExist:

                                    json = resultOperation.ToString();
                                    responseData = Encoding.UTF8.GetBytes(
                                    json.ToString().ToCharArray());
                                    stream.Write(responseData, 0, responseData.Length);
                                    break;

                                case ResultOperations.CardExpired:

                                    json = resultOperation.ToString();
                                    responseData = Encoding.UTF8.GetBytes(
                                    json.ToString().ToCharArray());
                                    stream.Write(responseData, 0, responseData.Length);
                                    break;
                                case ResultOperations.ChargeError:

                                    json = resultOperation.ToString();
                                    responseData = Encoding.UTF8.GetBytes(
                                    json.ToString().ToCharArray());
                                    stream.Write(responseData, 0, responseData.Length);

                                    break;
                            }

                            break;
                    }

                    #endregion ИЗМЕНЕНИЕ

                    break;

                case CardsOperation.SeeBalance:

                    #region ПРОСМОТР БАЛАНСА

                    int.TryParse(dataReceived.Split(';')[1], out int cardN);
                    resultOperation = pgDB.FindCardByCard(out card, cardN);

                    if (resultOperation == ResultOperations.None)
                    {
                        json = JsonConvert.SerializeObject(card);
                    }
                    else
                    {
                        json = resultOperation.ToString();
                    }

                    responseData = Encoding.UTF8.GetBytes(
                          json.ToString().ToCharArray());
                    stream.Write(responseData, 0, responseData.Length);

                    #endregion ПРОСМОТР БАЛАНСА

                    break;
            }

            client.Close();
        }

        /// <summary>
        /// Ждем произвольное количество милисекунд для повторного запроса.
        /// </summary>
        private static void WaitRandomTime()
        {
            Random random = new Random();
            Thread.Sleep(random.Next(15, 1000));
        }
    }
}
