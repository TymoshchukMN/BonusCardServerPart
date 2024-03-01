using System;
using CardsHandlerServerPart.Enums;
using CardsHandlerServerPart.Interfaces;
using Newtonsoft.Json;

namespace CardsHandlerServerPart
{
    public class ChangeCard : IProcessCard
    {
        public void ProcessCard(
            ref StreamProcessor streamProcessor, IDBProcessCard sqlInstance)
        {
            string dataReceived = streamProcessor.GetReceivedData();

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
                        sqlInstance.AddBonus(out Card card, cardNum, summ);

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
                        sqlInstance.Charge(out card, cardNum, summ);
                    // sqlInstance.FindCardByCard(out card, cardNum);

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