using CardsHandlerServerPart.Data;
using CardsHandlerServerPart.Interfaces;
using Newtonsoft.Json;

namespace CardsHandlerServerPart
{
    public class GetBalanceCard : IProcessCard
    {
        public void ProcessCard(
            ref StreamProcessor streamProcessor, IDBProcessCard sqlInstance)
        {
            const int IndexCardPosiition = 2;
            int.TryParse(
                streamProcessor.GetReceivedData().Split(';')[IndexCardPosiition],
                out int cardN);

            ResultOperations resultOperation =
                sqlInstance.FindCardByCard(out Card card, cardN);

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