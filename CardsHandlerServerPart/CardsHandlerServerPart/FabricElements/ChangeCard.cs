using System;
using System.Data;
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
            const int IndexCardPosiition = 2;
            const int IndexSumPosition = 3;

            int.TryParse(
                dataReceived.Split(';')[IndexCardPosiition], out int cardNum);
            int.TryParse(
                dataReceived.Split(';')[IndexSumPosition], out int summ);

            DataTable dataTable = null;
            ResultOperations resultOperation = ResultOperations.None;

            switch (bonusOperations)
            {
                case BonusOperations.Add:
                    resultOperation =
                        sqlInstance.AddBonus(out dataTable, cardNum, summ);

                    break;
                case BonusOperations.Remove:

                    resultOperation =
                        sqlInstance.Charge(out dataTable, cardNum, summ);

                    break;
            }

            if (resultOperation == ResultOperations.None)
            {
                streamProcessor.SendDataToUser(
                  JsonConvert.SerializeObject(dataTable));
            }
            else
            {
                streamProcessor.SendDataToUser(
                    resultOperation.ToString());
            }
        }
    }
}