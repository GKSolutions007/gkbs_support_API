using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKBS_SUPPORT_API.Models
{
    public class ToolsModel
    {

    }

    public class Appconfig
    {
        public string ID { get; set; }
        public string CustCode { get; set; }
        public string CustName { get; set; }
        public string DisplayName { get; set; }

        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string SmtpConfig { get; set; }

        public string CustomerID { get; set; }

    }
}