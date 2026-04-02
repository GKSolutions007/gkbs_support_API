using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKBS_SUPPORT_API.Models
{
    public class ReportModel
    {
    }
    public class ColumnRequest
    {
        public int ReportID { get; set; }
        public int TableID { get; set; }
    }
    public class ReportParameters
    {
        public string ParameterID { get; set; }
        public string ReportID { get; set; }
        public string ParameterName { get; set; }
        public string ParameterType { get; set; }
        public string IsMandatory { get; set; }
        public string ParamOrder { get; set; }
        public string AutolistName { get; set; }
        public string ProcedureName { get; set; }
        public string SendFiltersDetail { get; set; }

        public List<ReportFilters> lstvFilters { get; set; }
    }
    public class ReportFilters
    {
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param4 { get; set; }
        public string Param3 { get; set; }
        public string Param5 { get; set; }
    }
    public class ReportColumn
    {
        public int ReportID { get; set; }
        public int TableID { get; set; }
        public int ColumnID { get; set; }

        // Grid-specific mapping names
        public string field { get; set; }
        public string header { get; set; }
        public int width { get; set; }

        // Visual and Behavior Settings
        public int Alignment { get; set; }
        public int Visible { get; set; }
        public int IsHiddenColumn { get; set; }
        public int DisplayIndex { get; set; }
        public string DataType { get; set; }
        public string Total { get; set; }
        public int TotalYN { get; set; }
    }
    public class ColumnSettingsDataModel
    {
        public string field { get; set; }
        public string header { get; set; }
        public string type { get; set; }
        public int width { get; set; }
        public string align { get; set; }
        public bool visible { get; set; }
        public bool EnableColumnMenu { get; set; }
        public bool ShowinColumnOption { get; set; }
        public bool Total { get; set; }
        public string TotalYN { get; set; }
        public bool EnableSum { get; set; }
        public bool EnableAvg { get; set; }
        public bool EnableCount { get; set; }
        public bool EnableUnique { get; set; }
        public string precision { get; set; }
        public bool ClickPopup { get; set; }
        public int Printwidth { get; set; }
        public bool PrintYN { get; set; }
        public string PrintColumnName { get; set; }
    }
    public class ColumnSettingsModel
    {
        public string Alignment { get; set; }
        public string ClickPopup { get; set; }
        public string ColumnID { get; set; }
        public string ColumnName { get; set; }
        public string FieldType { get; set; }
        public string DisplayColumnName { get; set; }
        public string DisplayIndex { get; set; }
        public string EnableAvg { get; set; }
        public string EnableColumnMenu { get; set; }
        public string EnableSum { get; set; }
        public string EnableCount { get; set; }
        public string EnableUnique { get; set; }
        public string FormID { get; set; }
        public string FormName { get; set; }
        public string FormorReport { get; set; }
        public string IsHiddenColumn { get; set; }
        public string ShowinColumnOption { get; set; }
        public string TableID { get; set; }
        public string Total { get; set; }
        public string TotalYN { get; set; }
        public string Visible { get; set; }
        public string Width { get; set; }
        public string precision { get; set; }
        public int Printwidth { get; set; }
        public bool PrintYN { get; set; }
        public string PrintColumnName { get; set; }

    }
}