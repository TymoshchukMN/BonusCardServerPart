using CardsHandlerServerPart.Data;
using CardsHandlerServerPart.Interfaces;
using Newtonsoft.Json;

namespace CardsHandlerServerPart
{
    public class GetBalanceCard : IProcessCard
    {
        public void ProcessCard(StreamProcessor streamProcessor)
        {
            IDBProcessCard pgDB = PostgresDB.GetInstance();
            int.TryParse(streamProcessor.GetReceivedData().Split(';')[1], out int cardN);

            ResultOperations resultOperation =
                pgDB.FindCardByCard(out Card card, cardN);

            if (resultOperation == ResultOperations.None)
            {
                streamProcessor.SendDataToUser(JsonConvert.SerializeObject(card));
            }
            else
            {
                streamProcessor.SendDataToUser(resultOperation.ToString());
            }
        }
    }
}