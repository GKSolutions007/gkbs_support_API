using GKBS_SUPPORT_API.BuisnessLayer;
using GKBS_SUPPORT_API.Helpers;
using GKBS_SUPPORT_API.DALHelper;
using GKBS_SUPPORT_API.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
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
    public class CustomerController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        string connectionString = clsEncryptDecrypt.Decrypt(ConfigurationManager.ConnectionStrings["dbconncection"].ConnectionString);

        [HttpGet]
        [Route("api/customer/synccompany")]
        public IHttpActionResult GetCustomerList(string CompanyCode)
        {
            DataTable DDT = bl.BL_ExecuteParamSP("uspSyncwithGKS", CompanyCode);            
            return Ok(new { Status = DDT.Rows.Count == 0 ? "Success" :  "Error", Message = DDT.Rows.Count > 0 ? DDT.Rows[0][0].ToString() : "Synchronized Successfully" });
        }
            [HttpGet]
        [Route("api/customer/GetCustomerList")]
        public IHttpActionResult GetCustomerList(int userId = 0 )
        {
            try
            {
                DataTable DDT = bl.BL_ExecuteParamSP("uspgetcustomerlist");
                List<Customers> list = new List<Customers>();

                if (DDT != null && DDT.Rows.Count > 0)
                {
                    foreach (DataRow row in DDT.Rows)
                    {
                        list.Add(new Customers
                        {
                            ID = row["ID"].ToString(),
                            Customercode = row["Customercode"].ToString(),
                            Customername = row["Customername"].ToString(),
                            Address = row["Address"].ToString(),
                            Latitude = row["Latitude"].ToString(),                            
                            Longtitude = row["Longtitude"].ToString(),
                            GSTIN = row["GSTIN"].ToString(),
                            MobileNo = row["MobileNo"].ToString(),
                            EmailID = row["EmailID"].ToString(),
                            RegisteredDate = row["RegistedDate"].ToString(),
                            ExpDate = row["ExpDate"].ToString(),
                            WebExpDate = row["WebExpDate"].ToString(),
                            AmcDate = row["AmcDate"].ToString(),
                            ShineType = row["ShineType"].ToString(),
                            Usertype = row["Usertype"].ToString(),
                            NoofClient = row["NoofClient"].ToString(),
                            MobileApp = row["MobileApp"].ToString(),
                            ParentCompCode = row["ParentCompName"].ToString(),
                            Version = row["Version"].ToString(),
                            Active = row["Active"].ToString(),
                            CByName = row["CByName"].ToString(),
                            CDate = row["CDate"].ToString(),
                            MByName = row["MByName"].ToString(),
                            MDate = row["MDate"].ToString(),
                            ShineTypeValue = row["ShineTypeValue"].ToString(),
                            UsertypeValue = row["UserTypeValue"].ToString(),

                            

                        });
                    }
                    return Ok(list);
                }
                else
                {
                    return Ok(new List<Customers>());
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); ;

            }
        }


        [HttpPost]
        [Route("api/Customer/Save")]
        public IHttpActionResult Save(Customers listmaster)
        {
            List<SaveMessage> list = new List<SaveMessage>();
            try
            {
                if (listmaster != null)
                {

                    string actionType = (string.IsNullOrEmpty(listmaster.ID) || listmaster.ID == "0") ? "add" : "edit";


                    DataTable contactDT = new DataTable();
                    contactDT.Columns.Add("ID", typeof(int));
                    contactDT.Columns.Add("Name", typeof(string));
                    contactDT.Columns.Add("MobileNo", typeof(string));
                    contactDT.Columns.Add("Owner_Staff", typeof(string));

                    if (listmaster.ContactInfo != null)
                    {
                        foreach (var contact in listmaster.ContactInfo)
                        {
                            contactDT.Rows.Add(0, contact.Name, contact.MobileNo, contact.Owner_Staff);
                        }
                    }

                    DataTable DDT = bl.BL_ExecuteParamSP(
                        "uspManageCustomerMaster",
                        actionType,
                        listmaster.ID,
                        listmaster.Customercode,
                        listmaster.Customername,
                        listmaster.Address,
                        listmaster.Latitude,
                        listmaster.Longtitude,
                        listmaster.MobileNo,
                        listmaster.EmailID,
                        listmaster.GSTIN,
                        listmaster.RegisteredDate,
                        listmaster.ExpDate,
                        listmaster.WebExpDate,
                        listmaster.AmcDate,
                        listmaster.ShineType,
                        listmaster.Usertype,
                        listmaster.NoofClient,
                        listmaster.MobileApp,
                        !string.IsNullOrEmpty(listmaster.ParentCompCode) ? listmaster.ParentCompCode : "0",
                        listmaster.Version,
                        listmaster.Active,
                        listmaster.UID,
                        contactDT 
                    );


                    if (DDT != null && DDT.Rows.Count > 0)
                    {
                        int returnedID = Convert.ToInt32(DDT.Rows[0]["ID"]);
                        list.Add(new SaveMessage
                        {
                            Status = "Success",
                            Message = actionType == "add" ? "Customer added successfully." : "Customer updated successfully.",
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

        [HttpPost]
        [Route("api/Customer/SaveAttachment")]
        public async Task<IHttpActionResult> SaveAttachment()
        {
            List<SaveMessage> list = new List<SaveMessage>();
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    return BadRequest("Unsupported media type.");

                var provider = new MultipartFormDataStreamProvider(Path.GetTempPath());
                await Request.Content.ReadAsMultipartAsync(provider);

                string customerID = provider.FormData["CustomerID"] ?? "0";
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

                    DataTable DDT = bl.BL_ExecuteParamSP(
                        "uspManageCustomerAttachments",
                        "save",
                        0,
                        customerID,
                        originalName,
                        fileExt,
                        fileSize,
                        finalBytes,
                        uid
                    );

                    File.Delete(file.LocalFileName); // clean temp
                }

                list.Add(new SaveMessage { Status = "Success", Message = "File(s) uploaded successfully." });
            }
            catch (Exception ex)
            {
                list.Add(new SaveMessage { Status = "Error", Message = ex.Message });
            }
            return Ok(list);
        }

        // ✅ GET ATTACHMENT LIST
        [HttpPost]
        [Route("api/Customer/GetAttachments")]
        public IHttpActionResult GetAttachments([FromBody] dynamic body)
        {
            try
            {
                string customerID = body.CustomerID.ToString();
                DataTable DDT = bl.BL_ExecuteParamSP(
                    "uspManageCustomerAttachments",
                    "get", 0, customerID,
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

        [HttpPost]
        [Route("api/Customer/DownloadAttachment")]
        public IHttpActionResult DownloadAttachment([FromBody] dynamic body)
        {
            try
            {
                string id = body.ID.ToString();
                DataTable DDT = bl.BL_ExecuteParamSP(
                    "uspManageCustomerAttachments",
                    "download", id, 0,
                    null, null, 0, null, 0
                );

                if (DDT == null || DDT.Rows.Count == 0)
                    return NotFound();

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
                    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                response.Content.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(mime);

                return ResponseMessage(response);
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/Customer/DeleteAttachment")]
        public IHttpActionResult DeleteAttachment([FromBody] dynamic body)
        {
            try
            {
                string id = body.ID.ToString();
                bl.BL_ExecuteParamSP(
                    "uspManageCustomerAttachments",
                    "delete", id, 0,
                    null, null, 0, null, 0
                );
                return Ok(new { success = true, message = "File deleted." });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/Customer/EditAttachment")]
        public async Task<IHttpActionResult> EditAttachment()
        {
            List<SaveMessage> list = new List<SaveMessage>();
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    return BadRequest("Unsupported media type.");

                var provider = new MultipartFormDataStreamProvider(Path.GetTempPath());
                await Request.Content.ReadAsMultipartAsync(provider);

                string id = provider.FormData["ID"] ?? "0";
                string uid = provider.FormData["UID"] ?? "0";

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

                    DataTable DDT = bl.BL_ExecuteParamSP(
                            "uspManageCustomerAttachments",
                            "edit",
                            id,
                            0,
                            originalName,
                            fileExt,
                            fileSize,
                            fileBytes,
                            uid
                        );

                    File.Delete(file.LocalFileName);
                }

                list.Add(new SaveMessage { Status = "Success", Message = "File updated successfully." });
            }
            catch (Exception ex)
            {
                list.Add(new SaveMessage { Status = "Error", Message = ex.Message });
            }
            return Ok(list);
        }

        [HttpPost]
        [Route("api/Customer/GetContacts")]
        public IHttpActionResult GetContacts([FromBody] dynamic body)
        {
            try
            {
                string customerID = body.CustomerID.ToString();
                DataTable DDT = bl.BL_ExecuteParamSP(
                    "uspManageCustomerContacts",
                    "get", 0, customerID
                );

                var contacts = new List<object>();
                foreach (DataRow row in DDT.Rows)
                {
                    contacts.Add(new
                    {
                        ContactID = row["ContactID"],
                        Name = row["Name"],
                        MobileNo = row["MobileNo"],
                        Owner_Staff = row["Owner_Staff"]
                    });
                }

                return Ok(new { success = true, contacts = contacts });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        [Route("api/Customer/DeleteContact")]
        public IHttpActionResult DeleteContact([FromBody] dynamic body)
        {
            try
            {
                string id = body.ID.ToString();
                bl.BL_ExecuteParamSP(
                    "uspManageCustomerContacts",
                    "delete", id, 0
                );
                return Ok(new { success = true, message = "Contact deleted." });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/Customer/GetCustomerByID")]
        public IHttpActionResult GetCustomerByID(int ID)
        {
            try
            {
                DataSet DS = bl.BL_ExecuteParamSPDataset("uspGetCustomerByID", ID);

                if (DS == null || DS.Tables.Count == 0 || DS.Tables[0].Rows.Count == 0)
                    return Ok(new { success = false, message = "Customer not found" });

                DataRow row = DS.Tables[0].Rows[0];

                var customer = new
                {
                    ID = row["ID"].ToString(),
                    Customercode = row["Customercode"].ToString(),
                    Customername = row["Customername"].ToString(),
                    Address = row["Address"].ToString(),
                    Latitude = row["Latitude"].ToString(),
                    Longtitude = row["Longtitude"].ToString(),
                    GSTIN = row["GSTIN"].ToString(),
                    MobileNo = row["MobileNo"].ToString(),
                    EmailID = row["EmailID"].ToString(),
                    RegisteredDate = !string.IsNullOrEmpty(row["RegistedDate"].ToString()) ? Convert.ToDateTime(row["RegistedDate"]).ToString("yyyy-MM-dd") : null,
                    ExpDate = !string.IsNullOrEmpty(row["ExpDate"].ToString()) ? Convert.ToDateTime(row["ExpDate"]).ToString("yyyy-MM-dd") : null,
                    WebExpDate = !string.IsNullOrEmpty(row["WebExpDate"].ToString()) ? Convert.ToDateTime(row["WebExpDate"]).ToString("yyyy-MM-dd") : null,
                    AmcDate = !string.IsNullOrEmpty(row["AmcDate"].ToString()) ? Convert.ToDateTime(row["AmcDate"]).ToString("yyyy-MM-dd") : null,
                    ShineType = row["ShineType"].ToString(),
                    Usertype = row["Usertype"].ToString(),
                    NoofClient = row["NoofClient"].ToString(),
                    MobileApp = row["MobileApp"].ToString(),
                    ParentCompCode = row["ParentCompCode"].ToString(),
                    Version = row["Version"].ToString(),
                    Active = row["Active"].ToString()
                };

                var contacts = new List<object>();
                if (DS.Tables.Count > 1)
                {
                    foreach (DataRow cr in DS.Tables[1].Rows)
                    {
                        contacts.Add(new
                        {
                            ContactID = cr["ID"].ToString(),
                            Name = cr["Name"].ToString(),
                            MobileNo = cr["MobileNo"].ToString(),
                            Owner_Staff = cr["Owner_Staff"].ToString()
                        });
                    }
                }

                return Ok(new { success = true, customer = customer, contacts = contacts });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

    }
}