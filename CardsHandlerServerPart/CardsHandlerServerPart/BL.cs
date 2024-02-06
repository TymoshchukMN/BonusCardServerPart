using System.IO;
using CardsHandlerServerPart.Configs;
using CardsHandlerServerPart.JSON;
using Newtonsoft.Json;

namespace CardsHandlerServerPart
{
    internal class BL
    {
        /// <summary>
        /// Получить конфиг подключениея к БД.
        /// </summary>
        /// <returns>
        /// Конфиг подключение.
        /// </returns>
        public static DBConfigJSON GetDBConfig()
        {
            const string ConfFilePathDB = @"\\172.16.112.40\share\TymoshchukMN\DBconfigFile.json";
            string dbConfigFile = File.ReadAllText(ConfFilePathDB);
            DBConfigJSON dbConfigJSON = JsonConvert.DeserializeObject<DBConfigJSON>(dbConfigFile);

            return dbConfigJSON;
        }

        /// <summary>
        ///  Получить конфиг подключениея к серверу.
        /// </summary>
        /// <returns>конфиг подключениея к серверу</returns>
        public static SrvConfig GetServerConfig()
        {
            const string ConfFilePathSRV = @"\\172.16.112.40\share\TymoshchukMN\SRVconfigFile.json";
            string srvConfigFile = File.ReadAllText(ConfFilePathSRV);
            SrvConfig srvConfigJSON = JsonConvert.DeserializeObject<SrvConfig>(srvConfigFile);

            return srvConfigJSON;
        }

        public static CreateCardConfig GetCreateCardConfig(string json)
        {
            string srvConfigFile = File.ReadAllText(json);
            CreateCardConfig srvConfigJSON = JsonConvert.DeserializeObject<CreateCardConfig>(srvConfigFile);

            return srvConfigJSON;
        }
    }
}
