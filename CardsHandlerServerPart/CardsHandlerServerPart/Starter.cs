using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using CardsHandlerServerPart.Configs;
using CardsHandlerServerPart.Data;
using CardsHandlerServerPart.Enums;
using CardsHandlerServerPart.FabricElements;
using CardsHandlerServerPart.Interfaces;
using CardsHandlerServerPart.JSON;

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
            SrvConfig srvConfig = BL.GetServerConfig();

            int port = srvConfig.Port;
            string serverAddress = srvConfig.Server;

            IPAddress ipAddress = IPAddress.Parse(serverAddress);
            TcpListener listener = new TcpListener(ipAddress, port);
            listener.Start();
            Console.WriteLine("Сервер запущен...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();

                Task.Run(() => ProcessClientRequest(client));
            }
        }

        /// <summary>
        /// Ассинхронная обработка запросов от пользователя.
        /// </summary>
        /// <param name="client">TCP-подключение от клиента.</param>
        /// <returns>Ассинхронный метод.</returns>
        public static async Task ProcessClientRequest(TcpClient client)
        {
            await Task.Run(() =>
            {
                CardsPool cardsPoll = CardsPool.GetInstance();
                using (StreamProcessor streamProcessor = new StreamProcessor(client))
                {
                    string dataReceived = streamProcessor.GetReceivedData();
                    Console.WriteLine($"Получено от клиента: {dataReceived}");

                    CardsOperationList cardOperation =
                        (CardsOperationList)Enum.Parse(
                            typeof(CardsOperationList),
                            dataReceived.Split(';')[0]);

                    IDBProcessCard pgDB = PostgresDB.GetInstance();
                    IProcessCard processCard = CommandFactory.GetCommand(cardOperation);

                    processCard.ProcessCard(streamProcessor);
                }

                client.Close();
            });
        }
    }
}
