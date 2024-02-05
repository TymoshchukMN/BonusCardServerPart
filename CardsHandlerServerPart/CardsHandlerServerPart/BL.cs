using System.IO;
using CardsHandlerServerPart.Configs;
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
        public static ServerConfig GetServerConfig()
        {
            const string ConfFilePathDB = @"\\172.16.112.40\share\TymoshchukMN\DBconfigFile.json";
            string srvConfigFile = File.ReadAllText(ConfFilePathDB);
            ServerConfig srvConfigJSON = JsonConvert.DeserializeObject<ServerConfig>(srvConfigFile);

            return srvConfigJSON;
        }

    }
}
