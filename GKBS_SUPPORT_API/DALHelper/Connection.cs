using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace GKBS_SUPPORT_API.DALHelper
{
    public class Connection
    {
        public static string GetConnectionString()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["dbconncection"].ConnectionString;
            return connectionString;
        }
    }
}