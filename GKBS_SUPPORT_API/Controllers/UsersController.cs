using GKBS_SUPPORT_API.BuisnessLayer;
using GKBS_SUPPORT_API.DALHelper;
using GKBS_SUPPORT_API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GKBS_SUPPORT_API.Controllers
{
    public class UsersController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        string connectionString = clsEncryptDecrypt.Decrypt(ConfigurationManager.ConnectionStrings["dbconncection"].ConnectionString);

        [HttpGet]
        [Route("api/users/GetUserList")]

        public IHttpActionResult GetCustomerList()
        {
            try
            {
                DataTable DDT = bl.BL_ExecuteParamSP("uspgetuserslist");
                List<Users> list = new List<Users>();

                if (DDT != null && DDT.Rows.Count > 0)
                {
                    foreach (DataRow row in DDT.Rows)
                    {
                        list.Add(new Users
                        {
                            ID = row["ID"].ToString(),
                            UserID = row["UserID"].ToString(),
                            UserName = row["UserName"].ToString(),
                            Password = clsEncryptDecrypt.Decrypt(row["Password"].ToString()),
                            Mobilenumber = row["MobileNo"].ToString(),
                            EMailID = row["Email"].ToString(),
                            EmployeeNo = row["EmployeeNo"].ToString(),
                            Active = row["Active"].ToString(),
                            CBy = row["CBy"].ToString(),
                            CDate = row["CDate"].ToString(),
                            MBy = row["MBy"].ToString(),
                            MDate = row["MDate"].ToString(),
                        });
                    }
                    return Ok(list);
                }
                else
                {
                    return Ok(new List<Users>());
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); ;

            }
        }


        [HttpPost]
        [Route("api/Users/Save")]
        public IHttpActionResult Save(Users listmaster)
        {
            List<SaveMessage> list = new List<SaveMessage>();
            try
            {
                if (listmaster != null)
                {
                    string actionType = (string.IsNullOrEmpty(listmaster.ID) || listmaster.ID == "0") ? "add" : "edit";

                    DataTable DDT = bl.BL_ExecuteParamSP(
                        "uspManageUsers",
                        actionType,
                        listmaster.ID,
                        listmaster.UserID,
                        listmaster.Password = clsEncryptDecrypt.Encrypt(listmaster.Password),
                        listmaster.UserName,
                        listmaster.Mobilenumber,
                        listmaster.EMailID,
                        listmaster.EmployeeNo,
                        listmaster.active,
                        listmaster.CBy
                    );

                    if (DDT != null && DDT.Rows.Count > 0)
                    {
                        int returnedID = Convert.ToInt32(DDT.Rows[0]["ID"]);
                        list.Add(new SaveMessage
                        {
                            Status = "Success",
                            Message = actionType == "add" ? "User added successfully." : "User updated successfully.",
                            ID = returnedID.ToString()
                        });
                    }
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
    }
}