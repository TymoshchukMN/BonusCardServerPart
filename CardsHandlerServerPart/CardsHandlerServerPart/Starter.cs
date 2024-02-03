using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CardsHandlerServerPart.Data;

namespace CardsHandlerServerPart
{
    internal class Starter
    {
        public static void Run()
        {
            CardsPool cardsPoll = CardsPool.GetInstance();
            DBConfigJSON dBConfig = BL.GetDBConfig();

            PostgresDB pgDB = PostgresDB.GetInstance(
               dBConfig.DBConfig.Server,
               dBConfig.DBConfig.UserName,
               dBConfig.DBConfig.DBname,
               dBConfig.DBConfig.Port);

            pgDB.GetLastFreeValue(out int lastFreeVol);
            cardsPoll.FillPool(lastFreeVol);
            Console.WriteLine();
        }

        public static void StartServer()
        {
            const int port = 49001;
            const string ServerAddress = "127.0.0.1";
            const string RequestCard = "CardRequest";

            IPAddress ipAddress = IPAddress.Parse(ServerAddress);
            TcpListener listener = new TcpListener(ipAddress, port);
            CardsPool cardsPoll = CardsPool.GetInstance();
            listener.Start();
            Console.WriteLine("Сервер запущен...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                byte[] responseData;
                Console.WriteLine($"Получено от клиента: {dataReceived}");

                if (dataReceived == RequestCard)
                {
                    int cardNumber;

                    // проверяем свободен ли обработчик пула,
                    // если да - полчаем номер карты,
                    // если нет - ждем произвольное время
                    if (!cardsPoll.IsBusy)
                    {
                        cardNumber = cardsPoll.GetCarNumber();
                    }
                    else
                    {
                        do
                        {
                            WaitRandomTime();
                        }
                        while (cardsPoll.IsBusy);

                        cardNumber = cardsPoll.GetCarNumber();
                    }
                    Console.WriteLine();
                    responseData = Encoding.ASCII.GetBytes(
                        cardNumber.ToString().ToCharArray());
                    stream.Write(responseData, 0, responseData.Length);
                }

                client.Close();
            }
        }

        private static void WaitRandomTime()
        {
            Random random = new Random();
            Thread.Sleep(random.Next(15, 1000));
        }

    }
}
