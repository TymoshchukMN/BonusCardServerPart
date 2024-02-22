using System.Data;
using CardsHandlerServerPart.Data;
using CardsHandlerServerPart.Interfaces;
using Newtonsoft.Json;

namespace CardsHandlerServerPart
{
    public class GetAllCards : IProcessCard
    {
        public void ProcessCard(ref StreamProcessor streamProcessor)
        {
            IDBProcessCard pgDB = PostgresDB.GetInstance();
            pgDB.GetAllCards(out DataTable dataTable);
            streamProcessor.SendDataToUser(JsonConvert.SerializeObject(dataTable));
        }
    }
}