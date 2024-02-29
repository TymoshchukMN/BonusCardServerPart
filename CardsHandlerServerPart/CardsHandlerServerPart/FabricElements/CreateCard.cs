using System;
using System.Threading;
using CardsHandlerServerPart.Data;
using CardsHandlerServerPart.Interfaces;
using Newtonsoft.Json;

namespace CardsHandlerServerPart
{
    public class CreateCard : IProcessCard
    {
        public void ProcessCard(
            ref StreamProcessor streamProcessor, IDBProcessCard sqlInstance)
        {
            CardsPool cardsPoll = CardsPool.GetInstance();

            // проверяем свободен ли обработчик пула,
            // если да - полчаем номер карты,
            // если нет - ждем произвольное время
            if (cardsPoll.GetPoolCard.TryDequeue(out int cardNumber))
            {
                Console.WriteLine($"Карта получена, номер {cardNumber}");
            }
            else
            {
                Console.WriteLine("Пулл занят. Получение номера карты...");
                do
                {
                    WaitRandomTime();
                }
                while (cardsPoll.GetPoolCard.TryDequeue(out cardNumber));
            }

            cardsPoll.CheckSizePool();
            string dataReceived = streamProcessor.GetReceivedData();

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
            sqlInstance.CreateCard(card);

            streamProcessor.SendDataToUser(JsonConvert.SerializeObject(card));
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