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
    public class CustomerController : ApiController
    {
        clsBusinessLayer bl = new clsBusinessLayer();
        string connectionString = clsEncryptDecrypt.Decrypt(ConfigurationManager.ConnectionStrings["dbconncection"].ConnectionString);

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
                            Longitude = row["Longtitude"].ToString(),
                            GSTIN = row["GSTIN"].ToString(),
                            RegisteredDate = row["RegistedDate"].ToString(),
                            ExpDate = row["ExpDate"].ToString(),
                            AmcDate = row["AmcDate"].ToString(),
                            ShineType = row["ShineType"].ToString(),
                            Usertype = row["Usertype"].ToString(),
                            NoofClient = row["NoofClient"].ToString(),
                            MobileApp = row["MobileApp"].ToString(),
                            ParentCompCode = row["ParentCompCode"].ToString(),
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
                            contactDT.Rows.Add(contact.Name, contact.MobileNo, contact.Owner_Staff);
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
                        listmaster.Longitude,
                        listmaster.GSTIN,
                        listmaster.RegisteredDate,
                        listmaster.ExpDate,
                        listmaster.AmcDate,
                        listmaster.ShineType,
                        listmaster.Usertype,
                        listmaster.NoofClient,
                        listmaster.MobileApp,
                        listmaster.ParentCompCode,
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

    }
}