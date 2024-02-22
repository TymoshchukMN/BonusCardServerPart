﻿using System;
using CardsHandlerServerPart.Data;
using CardsHandlerServerPart.Enums;
using CardsHandlerServerPart.Interfaces;
using Newtonsoft.Json;

namespace CardsHandlerServerPart
{
    public class FindCard : IProcessCard
    {
        public void ProcessCard(StreamProcessor streamProcessor)
        {
            string dataReceived = streamProcessor.GetReceivedData();

            SearchType searchType =
                (SearchType)Enum.Parse(
                    typeof(SearchType),
                    dataReceived.Split(';')[1]);

            ResultOperations resultOperation = ResultOperations.None;

            IDBProcessCard pgDB = PostgresDB.GetInstance();
            string json = string.Empty;
            Card card = null;
            switch (searchType)
            {
                case SearchType.ByPhone:
                    string phone = dataReceived.Split(';')[2];
                    resultOperation = pgDB.FindCardByPhone(out card, phone);

                    break;
                case SearchType.ByCard:

                    int.TryParse(dataReceived.Split(';')[2], out int cardNN);
                    resultOperation = pgDB.FindCardByCard(out card, cardNN);

                    break;
            }

            if (resultOperation == ResultOperations.None)
            {
                streamProcessor.SendDataToUser(
                    JsonConvert.SerializeObject(card));
            }
            else
            {
                streamProcessor.SendDataToUser(resultOperation.ToString());
            }
        }
    }
}