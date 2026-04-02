using GKBS_SUPPORT_API.BuisnessLayer;
using GKBS_SUPPORT_API.DALHelper;
using GKBS_SUPPORT_API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GKBS_SUPPORT_API.Controllers
{
    public class ToolsController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        string connectionString = clsEncryptDecrypt.Decrypt(ConfigurationManager.ConnectionStrings["dbconncection"].ConnectionString);

        [HttpGet]
        [Route("api/AppConfig/GetCustomerCodes")]

        public IHttpActionResult GetAppconfig()
        {
            try
            {
                DataTable DDT = bl.BL_ExecuteParamSP("Usp_ListCustomerCodes");
                List<Appconfig> list = new List<Appconfig>();

                if (DDT != null && DDT.Rows.Count > 0)
                {
                    foreach (DataRow row in DDT.Rows)
                    {
                        list.Add(new Appconfig
                        {

                            ID = row["ID"].ToString(),
                            CustCode = row["Customercode"].ToString(),
                            CustName = row["Customername"].ToString(),
                            DisplayName = row["DisplayName"].ToString(),


                        });
                    }
                    return Ok(list);
                }
                else
                {
                    return Ok(new List<Appconfig>());
                }
;
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); ;

            }
        }


        [HttpPost]
        [Route("api/AppConfig/Save")]
        public IHttpActionResult Save(Appconfig listmaster)
        {
            List<SaveMessage> list = new List<SaveMessage>();
            try
            {
                if (listmaster != null)
                {
                    // Save each field dynamically like Node.js did
                    bl.BL_ExecuteParamSP("Usp_SaveAppConfig", "CustomerID", listmaster.CustomerID.ToString());
                    bl.BL_ExecuteParamSP("Usp_SaveAppConfig", "SMTPHost", listmaster.SmtpConfig);
                    bl.BL_ExecuteParamSP("Usp_SaveAppConfig", "EMail", listmaster.EmailAddress);

                    string encryptedPwd = clsEncryptDecrypt.Encrypt(listmaster.Password);
                    bl.BL_ExecuteParamSP("Usp_SaveAppConfig", "Password", encryptedPwd);

                    list.Add(new SaveMessage
                    {
                        Status = "Success",
                        Message = "Application configuration updated successfully.",
                        ID = listmaster.ID
                    });
                }
                else
                {
                    list.Add(new SaveMessage { Status = "Error", Message = "No data received." });
                }
            }
            catch (Exception ex)
            {
                list.Add(new SaveMessage { Status = "Error", Message = ex.Message });
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("api/AppConfig/Get")]
        public IHttpActionResult Get()
        {
            try
            {
                DataTable dt = bl.BL_ExecuteParamSP("Usp_GetAppConfig");

                var configData = new Dictionary<string, string>();

                foreach (DataRow row in dt.Rows)
                {
                    string appName = row["AppName"].ToString();
                    string appValue = row["AppValue"].ToString();

                    if (appName.ToLower().Contains("password"))
                    {
                        appValue = clsEncryptDecrypt.Decrypt(appValue);
                    }

                    configData[appName] = appValue;
                }

                return Ok(new { success = true, data = configData, message = "AppConfig loaded successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error Loading AppConfig", error = ex.Message });
            }
        }
    }
}