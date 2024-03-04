using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using CardsHandlerServerPart.Configs;
using CardsHandlerServerPart.Data;
using CardsHandlerServerPart.Enums;
using CardsHandlerServerPart.FabricElements;
using CardsHandlerServerPart.Interfaces;

namespace CardsHandlerServerPart
{
    internal class Starter
    {
        public static void Run()
        {
            CardsPool cardsPoll = CardsPool.GetInstance();

            MSSQLInstance sqlInstance = MSSQLInstance.GetInstance();
            sqlInstance.GetLastFreeValue(out int lastFreeVol);

            cardsPoll.FillPool(lastFreeVol);

            // Запуск сервера.
            StartServer();
        }

        /// <summary>
        /// Запуск сервера обработчика пула.
        /// </summary>
        public static void StartServer()
        {
            SqlSrvConfig sqlSrvConfig = BL.GetServerConfig();

            int port = sqlSrvConfig.Port;
            string sqlServerAddress = sqlSrvConfig.Server;

            // int port = 49001;
            // string sqlServerAddress = "127.0.0.1";
            IPAddress ipAddress = IPAddress.Parse(sqlServerAddress);
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

                StreamProcessor streamProcessor = new StreamProcessor(client);
                string dataReceived = streamProcessor.GetReceivedData();
                Console.WriteLine($"Получено от клиента: {dataReceived}");

                CardsOperationList cardOperation =
                    (CardsOperationList)Enum.Parse(
                        typeof(CardsOperationList),
                        dataReceived.Split(';')[0]);

                IDBProcessCard sqlInstance = MSSQLInstance.GetInstance();

                IProcessCard processCard = CommandFactory.GetCommand(cardOperation);

                processCard.ProcessCard(ref streamProcessor, sqlInstance);

                client.Close();
            });
        }
    }
}
