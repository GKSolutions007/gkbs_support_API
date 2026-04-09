using GKBS_SUPPORT_API.BuisnessLayer;
using GKBS_SUPPORT_API.DALHelper;
using GKBS_SUPPORT_API.Helpers;
using GKBS_SUPPORT_API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GKBS_SUPPORT_API.Controllers
{
    public class UsersController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();        
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
                            CByName = row["CByName"].ToString(),
                            CDate = row["CDate"].ToString(),
                            MByName = row["MByName"].ToString(),
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

        [HttpGet]
        [Route("api/Users/GetUserByID")]
        public IHttpActionResult GetUserByID(int ID)
        {
            try
            {
                DataTable DDT = bl.BL_ExecuteParamSP("uspManageUsers", "userdata", ID);

                if (DDT == null || DDT.Rows.Count == 0)
                    return Ok(new { success = false, message = "User not found" });

                DataRow row = DDT.Rows[0];

                var user = new
                {
                    ID = row["ID"].ToString(),
                    UserID = row["UserID"].ToString(),
                    UserName = row["UserName"].ToString(),
                    Password = clsEncryptDecrypt.Decrypt(row["Password"].ToString()),
                    Mobilenumber = row["MobileNo"].ToString(),
                    EMailID = row["Email"].ToString(),
                    EmployeeNo = row["EmployeeNo"].ToString(),
                    Active = row["Active"].ToString()
                };

                return Ok(new { success = true, user = user });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        // ── SAVE ATTACHMENT ──
        [HttpPost]
        [Route("api/Users/SaveAttachment")]
        public async Task<IHttpActionResult> SaveAttachment()
        {
            List<SaveMessage> list = new List<SaveMessage>();
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    return BadRequest("Unsupported media type.");

                var provider = new MultipartFormDataStreamProvider(Path.GetTempPath());
                await Request.Content.ReadAsMultipartAsync(provider);

                string userID = provider.FormData["UserID"] ?? "0";
                string uid = provider.FormData["UID"] ?? "0";

                foreach (MultipartFileData file in provider.FileData)
                {
                    string originalName = file.Headers.ContentDisposition.FileName.Trim('"');
                    string fileExt = Path.GetExtension(originalName).Replace(".", "").ToLower();
                    byte[] rawBytes = File.ReadAllBytes(file.LocalFileName);
                    byte[] finalBytes = FileHelper.GetProcessedFileBytes(rawBytes, fileExt);
                    int fileSize = finalBytes.Length;

                    if (fileSize > 5 * 1024 * 1024)
                    {
                        File.Delete(file.LocalFileName);
                        return Ok(new List<SaveMessage> {
                    new SaveMessage { Status = "Error", Message = $"{originalName} exceeds 5MB limit." }
                });
                    }

                    bl.BL_ExecuteParamSP(
                        "uspManageUserAttachments",
                        "save", 0, userID,
                        originalName, fileExt, fileSize, finalBytes, uid
                    );

                    File.Delete(file.LocalFileName);
                }

                list.Add(new SaveMessage { Status = "Success", Message = "File(s) uploaded successfully." });
            }
            catch (Exception ex)
            {
                list.Add(new SaveMessage { Status = "Error", Message = ex.Message });
            }
            return Ok(list);
        }

        // ── GET ATTACHMENTS ──
        [HttpPost]
        [Route("api/Users/GetAttachments")]
        public IHttpActionResult GetAttachments([FromBody] dynamic body)
        {
            try
            {
                string userID = body.UserID.ToString();
                DataTable DDT = bl.BL_ExecuteParamSP(
                    "uspManageUserAttachments",
                    "get", 0, userID, null, null, 0, null, 0
                );

                var files = new List<object>();
                foreach (DataRow row in DDT.Rows)
                {
                    files.Add(new
                    {
                        ID = row["ID"],
                        FileName = row["FileName"],
                        FileType = row["FileType"],
                        FileSize = row["FileSize"],
                        UploadedOn = row["UploadedOn"]
                    });
                }

                return Ok(new { success = true, files = files });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        // ── DOWNLOAD ATTACHMENT ──
        [HttpPost]
        [Route("api/Users/DownloadAttachment")]
        public IHttpActionResult DownloadAttachment([FromBody] dynamic body)
        {
            try
            {
                string id = body.ID.ToString();
                DataTable DDT = bl.BL_ExecuteParamSP(
                    "uspManageUserAttachments",
                    "download", id, 0, null, null, 0, null, 0
                );

                if (DDT == null || DDT.Rows.Count == 0) return NotFound();

                byte[] fileData = (byte[])DDT.Rows[0]["FileData"];
                string fileName = DDT.Rows[0]["FileName"].ToString();
                string fileType = DDT.Rows[0]["FileType"].ToString();

                var mimeTypes = new Dictionary<string, string> {
            { "pdf",  "application/pdf" },
            { "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { "xls",  "application/vnd.ms-excel" },
            { "doc",  "application/msword" },
            { "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { "png",  "image/png" },
            { "jpg",  "image/jpeg" },
            { "jpeg", "image/jpeg" },
            { "txt",  "text/plain" }
        };

                string mime = mimeTypes.ContainsKey(fileType) ? mimeTypes[fileType] : "application/octet-stream";

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(fileData)
                };
                response.Content.Headers.ContentDisposition =
                    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = fileName };
                response.Content.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(mime);

                return ResponseMessage(response);
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        // ── DELETE ATTACHMENT ──
        [HttpPost]
        [Route("api/Users/DeleteAttachment")]
        public IHttpActionResult DeleteAttachment([FromBody] dynamic body)
        {
            try
            {
                string id = body.ID.ToString();
                bl.BL_ExecuteParamSP(
                    "uspManageUserAttachments",
                    "delete", id, 0, null, null, 0, null, 0
                );
                return Ok(new { success = true, message = "File deleted." });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

    }
}