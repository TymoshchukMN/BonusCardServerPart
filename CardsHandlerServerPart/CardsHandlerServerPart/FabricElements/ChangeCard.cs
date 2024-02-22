using System;
using System.Net.Sockets;
using CardsHandlerServerPart.Data;
using CardsHandlerServerPart.Enums;
using CardsHandlerServerPart.Interfaces;
using Newtonsoft.Json;

namespace CardsHandlerServerPart
{
    public class ChangeCard : IProcessCard
    {
        public void ProcessCard(StreamProcessor streamProcessor)
        {
            string dataReceived = streamProcessor.GetReceivedData();

            IDBProcessCard pgDB = PostgresDB.GetInstance();
            BonusOperations bonusOperations =
                (BonusOperations)Enum.Parse(
                    typeof(BonusOperations),
                    dataReceived.Split(';')[1]);

            int.TryParse(dataReceived.Split(';')[2], out int cardNum);
            int.TryParse(dataReceived.Split(';')[3], out int summ);

            switch (bonusOperations)
            {
                case BonusOperations.Add:
                    ResultOperations resultOperation =
                        pgDB.AddBonus(out Card card, cardNum, summ);

                    pgDB.FindCardByCard(out card, cardNum);

                    if (resultOperation == ResultOperations.None)
                    {
                        streamProcessor.SendDataToUser(
                            JsonConvert.SerializeObject(card));
                    }
                    else
                    {
                        streamProcessor.SendDataToUser(
                            resultOperation.ToString());
                    }

                    break;
                case BonusOperations.Remove:

                    resultOperation =
                        pgDB.Charge(out card, cardNum, summ);
                    pgDB.FindCardByCard(out card, cardNum);

                    if (resultOperation == ResultOperations.None)
                    {
                        streamProcessor.SendDataToUser(
                            JsonConvert.SerializeObject(card));
                    }
                    else
                    {
                        streamProcessor.SendDataToUser(
                            resultOperation.ToString());
                    }

                    break;
            }
        }
    }
}