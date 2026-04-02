using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKBS_SUPPORT_API.Models
{
    public class DailyActivityModel
    {
    }
    public class CaseRequest
    {
        public string CaseNo { get; set; }
    }

    public class MasterTypeRequest
    {
        public string Type { get; set; }
    }

    public class DailyActivityRequest
    {
        public bool CaseType { get; set; }
        public string CaseNo { get; set; }
        public int CustomerID { get; set; }
        public string ActivityDate { get; set; }
        public string CallBy { get; set; }
        public string FromMobNo { get; set; }
        public int Mode { get; set; }
        public bool Mobile_Phone { get; set; }
        public int Purpose { get; set; }
        public int SupportThrough { get; set; }
        public string RemarkIssue { get; set; }
        public int Action { get; set; }
        public string RemarkSolution { get; set; }
        public int AssignTo { get; set; }
        public int CBy { get; set; }
        public int Status { get; set; }
        public int? OrgID { get; set; }
        public int? ToDoID { get; set; }
        public string FileName { get; set; }
    }

    public class ToDoWorkRequest
    {
        public int SNo { get; set; }
        public string Description { get; set; }
        public decimal DurationInHrs { get; set; }
        public int? AssignTo { get; set; }
        public int Status { get; set; }
        public int CBy { get; set; }

    }

    public class UserRequest
    {
        public int UserId { get; set; }
    }

    public class TaskRequest 
    { 
        public int TaskId { get; set; } 
    }

    public class OrgRequest 
    { 
        public int OrgId { get; set; } 
    }

}