using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKBS_SUPPORT_API.Models
{
    public class SingleMasterModel
    {
    }
    public class Users
    {
        public string Mode { get; set; }
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Mobilenumber { get; set; }
        public string EMailID { get; set; }
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public string PwdResetCount { get; set; }
        public string PwdResetTime { get; set; }
        public string LPin { get; set; }
        public string Active { get; set; }
        public bool active { get; set; }
        public string UserID { get; set; }
        public string BeatID { get; set; }
        public string SalesmanID { get; set; }
        public string BranchID { get; set; }
        public string CByName { get; set; }
        public string CBy { get; set; }
        public string MBy { get; set; }
        public string CDate { get; set; }
        public string UserImageData { get; set; }
        public string ResponseMessage { get; set; }
        public string EmployeeNo { get; set; }
        public string type { get; set; }
        public string MByName { get; set; }
        public string MDate { get; set; }

        public string UID { get; set; }
        public string token { get; set; }
    }

    public class SaveMessage
    {
        public string ID { get; set; }
        public string MsgID { get; set; }
        public string Message { get; set; }
        public string RowID { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }

    }

    public class CustomerContact
    {
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public string Owner_Staff { get; set; }
    }


    public class OTPRequest
    {
        public string OTPID { get; set; }
        public string OTP { get; set; }
    }

    public class Customers
    {
        public string ID { get; set; }
        public string Customercode { get; set; }
        public string Customername { get; set; }
        public string ExpDate { get; set; }
        public string WebExpDate { get; set; }
        public string AmcDate { get; set; }
        public string GSTIN { get; set; }
        public string MobileNo { get; set; }
        public string EmailID { get; set; }
        public string ShineTypeValue { get; set; }
        public string UsertypeValue { get; set; }
        public string NoofClient { get; set; }
        public string Active { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
        public string RegisteredDate { get; set; }
        public string ShineType { get; set; }
        public string Usertype { get; set; }
        public string MobileApp { get; set; }
        public string ParentCompCode { get; set; }
        public string Version { get; set; }
        public string CBy { get; set; }
        public string CByName { get; set; }
        public string CDate { get; set; }
        public string MBy { get; set; }
        public string MByName { get; set; }
        public string MDate { get; set; }
        public string UserTypeValue { get; set; }
        public string ActionUser { get; set; }
        public string ActionTime { get; set; }
        public string UID { get; set; }
        public string Type { get; set; }

        public List<CustomerContact> ContactInfo { get; set; }


    }
}