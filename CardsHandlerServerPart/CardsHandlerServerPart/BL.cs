using System.Collections.Concurrent;
using System.IO;
using CardsHandlerServerPart.Configs;
using CardsHandlerServerPart.JSON;
using Newtonsoft.Json;

namespace CardsHandlerServerPart
{
    internal class BL
    {
        /// <summary>
        ///  Получить конфиг подключениея к серверу.
        /// </summary>
        /// <returns>конфиг подключениея к серверу</returns>
        public static SqlSrvConfig GetServerConfig()
        {
            const string ConfFilePathSRV = @"\\172.16.112.40\share\TymoshchukMN\SRVconfigFile.json";
            string srvConfigFile = File.ReadAllText(ConfFilePathSRV);
            SqlSrvConfig srvConfigJSON = JsonConvert.DeserializeObject<SqlSrvConfig>(srvConfigFile);

            return srvConfigJSON;
        }

        public static CreateCardConfig GetCreateCardConfig(string json)
        {
            string srvConfigFile = File.ReadAllText(json);
            CreateCardConfig srvConfigJSON = JsonConvert.DeserializeObject<CreateCardConfig>(srvConfigFile);

            return srvConfigJSON;
        }

        public static void FillPool(int startVol, ConcurrentQueue<int> pool)
        {
            const int PoolSixe = 10000;
            if (startVol == 0)
            {
                startVol = 100000;
            }
            else
            {
                for (int i = startVol + 1; i < PoolSixe + startVol; i++)
                {
                    pool.Enqueue(i);
                }
            }
        }
    }
}
