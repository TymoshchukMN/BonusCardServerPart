using System;
using System.Data;
using CardsHandlerServerPart.Enums;
using CardsHandlerServerPart.Interfaces;
using Newtonsoft.Json;

namespace CardsHandlerServerPart
{
    public class FindCard : IProcessCard
    {
        public void ProcessCard(
            ref StreamProcessor streamProcessor, IDBProcessCard sqlInstance)
        {
            string dataReceived = streamProcessor.GetReceivedData();
            const int IndexSearchParamPosiition = 2;
            SearchType searchType =
                (SearchType)Enum.Parse(
                    typeof(SearchType),
                    dataReceived.Split(';')[1]);

            ResultOperations resultOperation = ResultOperations.None;

            DataTable dataTable = null;
            switch (searchType)
            {
                case SearchType.ByPhone:
                    string phone =
                        dataReceived.Split(';')[IndexSearchParamPosiition];
                    resultOperation =
                        sqlInstance.FindCardByPhone(out dataTable, phone);

                    break;
                case SearchType.ByCard:

                    int.TryParse(
                        dataReceived.Split(';')[IndexSearchParamPosiition],
                        out int cardNN);

                    resultOperation =
                        sqlInstance.FindCardByCardNum(out dataTable, cardNN);

                    break;
            }

            if (resultOperation == ResultOperations.None)
            {
                streamProcessor.SendDataToUser(
                    JsonConvert.SerializeObject(dataTable));
            }
            else
            {
                streamProcessor.SendDataToUser(resultOperation.ToString());
            }
        }
    }
}