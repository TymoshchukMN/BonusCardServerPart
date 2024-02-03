using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CardsHandlerServerPart.Data;

namespace CardsHandlerServerPart
{
    internal class Starter
    {
        public static void Run()
        {

            DBConfigJSON dBConfig = BL.GetDBConfig();

            PostgresDB pgDB = PostgresDB.GetInstance(
               dBConfig.DBConfig.Server,
               dBConfig.DBConfig.UserName,
               dBConfig.DBConfig.DBname,
               dBConfig.DBConfig.Port);

            pgDB.GetLastFreeValue(out int lastFreeVol);



        }
        public static void StartServer()
        {
            const int port = 49001;
            const string ServerAddress = "127.0.0.1";

            IPAddress ipAddress = IPAddress.Parse(ServerAddress);
            TcpListener listener = new TcpListener(ipAddress, port);

            listener.Start();
            Console.WriteLine("Сервер запущен...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Подключен новый клиент.");

                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Получено от клиента: {dataReceived}");

                string responseMessage = "Привет от сервера!";
                byte[] responseData = Encoding.ASCII.GetBytes(responseMessage);
                stream.Write(responseData, 0, responseData.Length);
                client.Close();
            }
        }

        
    }
}
