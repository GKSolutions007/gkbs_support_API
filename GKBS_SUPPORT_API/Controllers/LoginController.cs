using GKBS_SUPPORT_API.BuisnessLayer;
using GKBS_SUPPORT_API.DALHelper;
using GKBS_SUPPORT_API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Windows.Forms;


namespace GKBS_SUPPORT_API.Controllers
{
    public class LoginController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        string connectionString = clsEncryptDecrypt.Decrypt(ConfigurationManager.ConnectionStrings["dbconncection"].ConnectionString);
        [HttpGet]
        [Route("api/login/get")]

        public IHttpActionResult GetloginData(string UserName, string Password)
        {
            DataTable DDT = bl.BL_ExecuteParamSP("uspManageUsers", "login", 0, UserName, clsEncryptDecrypt.Encrypt(Password));
            List<Users> list = new List<Users>();
            if (DDT.Rows.Count > 0)
            {
                list.Add(new Users
                {
                    UserID = DDT.Rows[0]["UserID"].ToString(),
                    UserName = DDT.Rows[0]["UserName"].ToString(),
                    Mobilenumber = DDT.Rows[0]["MobileNo"].ToString(),
                    EMailID = DDT.Rows[0]["Email"].ToString(),
                    ID = DDT.Rows[0]["ID"].ToString(),
                    EmployeeNo = DDT.Rows[0]["EmployeeNo"].ToString(), 
                });
                return Ok(list);
            }
            else 
            {
                
            }
            return Ok(list);
        }

        [HttpPost]
        [Route("api/signup/save")]
        public IHttpActionResult Savessignup(Users lstMaster)
        {
            if(lstMaster != null)
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand("uspManageUsers", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@type", lstMaster.type);
                sqlCommand.Parameters.AddWithValue("@ID", lstMaster.ID);
                sqlCommand.Parameters.AddWithValue("@UserID", lstMaster.UserID);
                sqlCommand.Parameters.AddWithValue("@Password", clsEncryptDecrypt.Encrypt(lstMaster.Password));
                sqlCommand.Parameters.AddWithValue("@UserName", lstMaster.UserName);
                sqlCommand.Parameters.AddWithValue("@Mobilenumber", lstMaster.Mobilenumber);
                sqlCommand.Parameters.AddWithValue("@EmailID", lstMaster.EMailID);
                sqlCommand.Parameters.AddWithValue("@EmployeeNo", (object)lstMaster.EmployeeNo ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@Active", lstMaster.Active);
                sqlCommand.Parameters.AddWithValue("@UID", 0);
                DataTable DDT = new DataTable();
                SqlDataAdapter SDA = new SqlDataAdapter(sqlCommand);
                SDA.Fill(DDT);
                sqlConnection.Close();
                List<SaveMessage> list = new List<SaveMessage>();
                if (DDT.Columns.Count == 1)
                {
                    list.Add(new SaveMessage()
                    {
                        ID = DDT.Rows[0][0].ToString(),
                        MsgID = "0",
                        Message = "Saved Successfully"
                    });
                }
                else
                {
                    // If more columns, it's likely returning an error message from the SP
                    list.Add(new SaveMessage()
                    {
                        ID = "0",
                        MsgID = "1",
                        Message = DDT.Rows[0][0].ToString()
                    });
                }
                return Ok(list);
            }
            return Ok();
        }


        [HttpPost]
        [Route("api/login/otp-generate")]
        public IHttpActionResult GenerateOTP(Users model)
        {
            DataTable dtUser = bl.BL_ExecuteParamSP("uspManageUsers", "forgotpassword", 0, model.UserID, null, null, null, model.EMailID);

            if (dtUser != null && dtUser.Rows.Count > 0)
            {
                string userName = dtUser.Rows[0]["UserName"].ToString();
                string otp = new Random().Next(100000, 999999).ToString();

                string mailBody = $"<b>Hii {userName}</b>,<br/><br/>Your OTP for password reset is: <h2>{otp}</h2>";
                bool isSent = bl.SendEmail("Password Reset OTP", mailBody, model.EMailID);

                if (isSent)
                {
                    DataTable dtOTP = bl.BL_ExecuteParamSP("uspManageOTP", 1, 0, "forgotpassword", otp);

                    if (dtOTP != null && dtOTP.Rows.Count > 0)
                    {
                        return Ok(new { success = true, message = "OTP Sent!", OTPID = dtOTP.Rows[0]["ID"] });
                    }
                }
                return Ok(new { success = false, message = "Failed to send email. Try again." });
            }
            return Ok(new { success = false, message = "User ID or Email not found." });
        }


        [HttpPost]
        [Route("api/login/otp-verify")]
        public IHttpActionResult VerifyOTP(OTPRequest data)
        {
            string id = data.OTPID;
            string otp = data.OTP;

            DataTable dt = bl.BL_ExecuteParamSP("uspManageOTP", 2, data.OTPID, null, data.OTP);

            if (dt != null && dt.Rows.Count > 0 && dt.Columns.Contains("MSG"))
            {
                return Ok(new { success = false, message = dt.Rows[0]["MSG"].ToString() });
            }

            return Ok(new { success = true, message = "OTP Verified Successfully." });
        }



        [HttpPost]
        [Route("api/login/resetpassword")]
        public IHttpActionResult ResetPassword(Users model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserID) ||
                string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.EMailID))
            {
                return Ok(new { success = false, message = "Invalid Parameters!" });
            }

            try
            {
                string encryptedPass = clsEncryptDecrypt.Encrypt(model.Password);

                DataTable dt = bl.BL_ExecuteParamSP(
                    "uspManageUsers",
                    "resetpassword",
                    0,
                    model.UserID,
                    encryptedPass,      
                    null,
                    null,
                    null
                );

                return Ok(new { success = true, message = "Password Reset Successfully." });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Something went wrong. Please try again later.", error = ex.Message });
            }
        }
    }
}