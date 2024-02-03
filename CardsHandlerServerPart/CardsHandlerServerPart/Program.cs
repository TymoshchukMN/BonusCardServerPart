using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;

namespace CardsHandlerServerPart
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Starter.Run();
            //Starter.StartServer();

            Console.ReadKey();

        }

    }

}
