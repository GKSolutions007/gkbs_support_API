using GKBS_SUPPORT_API.BuisnessLayer;
using GKBS_SUPPORT_API.DALHelper;
using GKBS_SUPPORT_API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    public class TransactionController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        string connectionString = clsEncryptDecrypt.Decrypt(ConfigurationManager.ConnectionStrings["dbconncection"].ConnectionString);

        [HttpPost]
        [Route("api/Transaction/GetByCaseNo")]
        public IHttpActionResult GetByCaseNo(CaseRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.CaseNo))
                {
                    return Ok(new { success = false, message = "Case Number is required" });
                }

                DataTable dt = bl.BL_ExecuteParamSP("Usp_GetDailyActivityByCaseNo", request.CaseNo);

                if (dt != null && dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    var data = new Dictionary<string, object>();

                    foreach (DataColumn col in dt.Columns)
                    {
                        data[col.ColumnName] = row[col] == DBNull.Value ? "" : row[col];
                    }

                    return Ok(new { success = true, data = data });
                }
                else
                {
                    return Ok(new { success = false, message = "Case " + request.CaseNo + " not found." });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Server Error: " + ex.Message });
            }
        }


        [HttpPost]
        [Route("api/Transaction/getCustomerList")]
        public IHttpActionResult GetCustomerList()
        {
            try
            {
                DataTable dt = bl.BL_ExecuteParamSP("Usp_ListCustomerCodes");

                if (dt != null && dt.Rows.Count > 0)
                {
                    var customerList = dt.AsEnumerable().Select(row => new
                    {
                        ID = row["ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ID"]),
                        DisplayName = row["DisplayName"]?.ToString() ?? ""
                    }).ToList();

                    return Ok(new { success = true, data = customerList });
                }
                return Ok(new { success = false, message = "No customers found" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("api/Transaction/getSingleMasterByType")]
        public IHttpActionResult GetSingleMasterByType(MasterTypeRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Type))
                    return Ok(new { success = false, message = "Type is required" });

                DataTable dt = bl.BL_ExecuteParamSP("Usp_GetSingleMasterByType", request.Type);

                if (dt != null && dt.Rows.Count > 0)
                {
                    var list = dt.AsEnumerable().Select(row => new
                    {
                        ID = row["ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ID"]),
                        Name = row["Name"]?.ToString() ?? ""
                    }).ToList();

                    return Ok(new { success = true, data = list });
                }

                return Ok(new { success = true, data = new List<object>() });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("api/Transaction/getUserList")]
        public IHttpActionResult GetUserList()
        {
            try
            {
                DataTable dt = bl.BL_ExecuteSqlQuery(
                    "SELECT ID, UserName FROM tblUsers WHERE Active = 1 AND ID <> 1"
                );

                var list = dt.AsEnumerable().Select(row => new
                {
                    ID = row["ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ID"]),
                    UserName = row["UserName"]?.ToString() ?? ""
                }).ToList();

                return Ok(new { success = true, data = list });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Server Error: " + ex.Message });
            }
        }


        [HttpPost]
        [Route("api/Transaction/SaveDailyActivity")]
        public IHttpActionResult SaveDailyActivity(DailyActivityRequest request)
        {
            try
            {
                if (request == null)
                    return Ok(new { success = false, message = "Request data is required" });
                DataTable dt = bl.BL_ExecuteParamSP("Usp_SaveDailyActivity",
                    request.CaseType,
                    request.CaseNo,
                    request.CustomerID,
                    request.ActivityDate,
                    request.CallBy,
                    request.FromMobNo,
                    request.Mode,
                    request.Mobile_Phone,
                    request.Purpose,
                    request.SupportThrough,
                    request.RemarkIssue,
                    request.Action,
                    request.RemarkSolution,
                    request.AssignTo,
                    request.CBy,
                    request.Status,
                    request.OrgID.HasValue ? (object)request.OrgID.Value : DBNull.Value,
                    request.ToDoID.HasValue ? (object)request.ToDoID.Value : DBNull.Value,
                    request.FileName ?? ""
                );
                int newID = 0;
                if (dt != null && dt.Rows.Count > 0)
                    newID = Convert.ToInt32(dt.Rows[0]["NewID"]);
                return Ok(new { success = true, message = "Daily Activity saved successfully", newID });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("api/Transaction/GetCaseNo")]
        public IHttpActionResult GetCaseNo()
        {
            try
            {
                DataTable dt = bl.BL_ExecuteSqlQuery(
                    "SELECT CONCAT(CasePrefix, CaseNo) AS FullCaseNo FROM tblDocumentSeries WHERE ID = 1"
                );

                if (dt != null && dt.Rows.Count > 0)
                    return Ok(new { success = true, data = dt.Rows[0]["FullCaseNo"].ToString() });

                return Ok(new { success = false, data = "" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Server Error: " + ex.Message });
            }
        }


        [HttpPost]
        [Route("api/Transaction/getToDoWork")]
        public IHttpActionResult GetToDoWork()
        {
            try
            {
                DataTable dt = bl.BL_ExecuteSqlQuery(
                    "SELECT SNo, Description, DurationInHrs, AssignTo, Status, CBy, CDate FROM tblToDoList ORDER BY CDate DESC"
                );

                if (dt != null && dt.Rows.Count > 0)
                {
                    var list = dt.AsEnumerable().Select(row => new
                    {
                        SNo = row["SNo"] == DBNull.Value ? 0 : Convert.ToInt32(row["SNo"]),
                        Description = row["Description"]?.ToString() ?? "",
                        DurationInHrs = row["DurationInHrs"] == DBNull.Value ? 0 : Convert.ToDecimal(row["DurationInHrs"]),
                        AssignTo = row["AssignTo"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["AssignTo"]),
                        Status = row["Status"] == DBNull.Value ? 1 : Convert.ToInt32(row["Status"]),
                        CBy = row["CBy"] == DBNull.Value ? 0 : Convert.ToInt32(row["CBy"])
                    }).ToList();

                    return Ok(new { success = true, data = list });
                }

                return Ok(new { success = true, data = new List<object>() });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("api/Transaction/saveToDoWork")]
        public IHttpActionResult SaveToDoWork(ToDoWorkRequest request)
        {
            try
            {
                if (request == null)
                    return Ok(new { success = false, message = "Request is required" });
                DataTable dt = bl.BL_ExecuteParamSP("SaveToDoList",
                    request.SNo,
                    request.Description,
                    request.DurationInHrs,
                    request.AssignTo.HasValue ? (object)request.AssignTo.Value : DBNull.Value,
                    request.Status,
                    request.CBy
                );

                return Ok(new { success = true, message = "To Do Work saved successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Server Error: " + ex.Message });
            }
        }


        [HttpPost]
        [Route("api/Transaction/GetUserAssignedTask")]
        public IHttpActionResult GetUserAssignedTask(UserRequest request)
        {
            try
            {
                if (request == null || request.UserId <= 0)
                    return Ok(new { success = false, message = "User ID is required" });

                DataTable dt = bl.BL_ExecuteParamSP("GetUserAssignedTask", request.UserId);

                if (dt != null && dt.Rows.Count > 0)
                {
                    var list = dt.AsEnumerable().Select(row => new Dictionary<string, object>(
                        dt.Columns.Cast<DataColumn>().ToDictionary(
                            col => col.ColumnName,
                            col => row[col] == DBNull.Value ? (object)"" : row[col]
                        )
                    )).ToList();
                    return Ok(new { success = true, data = list });
                }
                return Ok(new { success = true, data = new List<object>() });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("api/Transaction/GetUserInProgress")]
        public IHttpActionResult GetUserInProgress(UserRequest request)
        {
            try
            {
                if (request == null || request.UserId <= 0)
                    return Ok(new { success = false, message = "User ID is required" });

                DataTable dt = bl.BL_ExecuteParamSP("GetUserAssignedProgress", request.UserId);

                if (dt != null && dt.Rows.Count > 0)
                {
                    var list = dt.AsEnumerable().Select(row => new Dictionary<string, object>(
                        dt.Columns.Cast<DataColumn>().ToDictionary(
                            col => col.ColumnName,
                            col => row[col] == DBNull.Value ? (object)"" : row[col]
                        )
                    )).ToList();
                    return Ok(new { success = true, data = list });
                }
                return Ok(new { success = true, data = new List<object>() });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        // For Task patch
        [HttpPost]
        [Route("api/Transaction/GetTaskById")]
        public IHttpActionResult GetTaskById(TaskRequest request)
        {
            try
            {
                DataTable dt = bl.BL_ExecuteSqlQuery(
                    "SELECT SNo, Description, DurationInHrs, AssignTo, Status FROM tblToDoList WHERE SNo = " + request.TaskId
                );

                if (dt != null && dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    var data = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                        data[col.ColumnName] = row[col] == DBNull.Value ? "" : row[col];
                    return Ok(new { success = true, data });
                }
                return Ok(new { success = false, message = "Task not found" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        // For In-Progress patch
        [HttpPost]
        [Route("api/Transaction/GetDailyActivityByOrgId")]
        public IHttpActionResult GetDailyActivityByOrgId(OrgRequest request)
        {
            try
            {
                DataTable dt = bl.BL_ExecuteParamSP("Usp_GetDailyActivityByOrgID", request.OrgId);

                if (dt != null && dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    var data = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                        data[col.ColumnName] = row[col] == DBNull.Value ? "" : row[col];
                    return Ok(new { success = true, data });
                }
                return Ok(new { success = false, message = "Record not found" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Server Error: " + ex.Message });
            }
        }


        // ── SAVE ATTACHMENT ──
        [HttpPost]
        [Route("api/Transaction/SaveAttachment")]
        public async Task<IHttpActionResult> SaveAttachment()
        {
            List<SaveMessage> list = new List<SaveMessage>();
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    return BadRequest("Unsupported media type.");

                var provider = new MultipartFormDataStreamProvider(Path.GetTempPath());
                await Request.Content.ReadAsMultipartAsync(provider);

                string caseNo = provider.FormData["CaseNo"] ?? "";
                string uid = provider.FormData["UID"] ?? "0";
                string dailyActivityID = provider.FormData["DailyActivityID"] ?? "0"; // ✅ NEW

                foreach (MultipartFileData file in provider.FileData)
                {
                    string originalName = file.Headers.ContentDisposition.FileName.Trim('"');
                    string fileExt = Path.GetExtension(originalName).Replace(".", "").ToLower();
                    byte[] fileBytes = File.ReadAllBytes(file.LocalFileName);
                    int fileSize = fileBytes.Length;

                    if (fileSize > 5 * 1024 * 1024)
                    {
                        File.Delete(file.LocalFileName);
                        return Ok(new List<SaveMessage> {
                    new SaveMessage { Status = "Error", Message = $"{originalName} exceeds 5MB limit." }
                });
                    }

                    bl.BL_ExecuteParamSP(
                        "uspManageDailyActivityAttachments",
                        "save", 0, int.Parse(dailyActivityID), caseNo,
                        originalName, fileExt, fileSize, fileBytes, uid
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
        [Route("api/Transaction/GetAttachments")]
        public IHttpActionResult GetAttachments([FromBody] dynamic body)
        {
            try
            {
                // ✅ Now receives DailyActivityID instead of CaseNo
                int dailyActivityID = int.Parse(body.DailyActivityID.ToString());

                DataTable DDT = bl.BL_ExecuteParamSP(
                    "uspManageDailyActivityAttachments",
                    "get", 0, dailyActivityID, null,
                    null, null, 0, null, 0
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

        // ── DOWNLOAD & DELETE stay exactly the same, no changes needed ──

    }
}