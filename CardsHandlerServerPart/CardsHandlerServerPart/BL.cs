using System.IO;
using Newtonsoft.Json;

namespace CardsHandlerServerPart
{
    internal class BL
    {
        public static DBConfigJSON GetDBConfig()
        {
            const string ConfFilePathDB = @"\\172.16.112.40\share\TymoshchukMN\DBconfigFile.json";
            string dbConfigFile = File.ReadAllText(ConfFilePathDB);
            DBConfigJSON dbConfigJSON = JsonConvert.DeserializeObject<DBConfigJSON>(dbConfigFile);

            return dbConfigJSON;
        }
    }
}
