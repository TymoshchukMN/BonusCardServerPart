using System.Data;
using CardsHandlerServerPart.Data;
using CardsHandlerServerPart.Interfaces;
using Newtonsoft.Json;

namespace CardsHandlerServerPart
{
    public class GetAllCards : IProcessCard
    {
        public void ProcessCard(
            ref StreamProcessor streamProcessor, IDBProcessCard sqlInstance)
        {
            DataTable dataTable = sqlInstance.GetAllCards();
            streamProcessor.SendDataToUser(
                JsonConvert.SerializeObject(dataTable));
        }
    }
}