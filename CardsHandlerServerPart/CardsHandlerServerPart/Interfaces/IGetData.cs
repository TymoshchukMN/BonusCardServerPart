using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsHandlerServerPart.Interfaces
{
    internal interface IGetData
    {
        DataTable GetDate(DbConnection connection);
    }
}
