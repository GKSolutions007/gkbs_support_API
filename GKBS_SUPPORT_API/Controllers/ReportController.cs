using GKBS_SUPPORT_API.BuisnessLayer;
using GKBS_SUPPORT_API.DALHelper;
using GKBS_SUPPORT_API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace GKBS_SUPPORT_API.Controllers
{
    public class ReportController : ApiController
    {

        clsBusinessLayer bl = new clsBusinessLayer();
        string connectionString = clsEncryptDecrypt.Decrypt(ConfigurationManager.ConnectionStrings["dbconncection"].ConnectionString);

        [HttpPost]
        [Route("api/report/fetchColumnheaders")]
        public IHttpActionResult GetColumnHeaders(ColumnRequest request)
        {
            if (request == null)
            {
                return Ok(new { success = false, message = "Request payload is null" });
            }

            try
            {
                DataTable DDT = bl.BL_ExecuteParamSP("UspListColumnByRepordId", request.ReportID, request.TableID);

                List<ReportColumn> list = new List<ReportColumn>();

                if (DDT != null && DDT.Rows.Count > 0)
                {
                    foreach (DataRow row in DDT.Rows)
                    {
                        list.Add(new ReportColumn
                        {
                            ReportID = Convert.ToInt32(row["ReportID"]),
                            TableID = Convert.ToInt32(row["TableID"]),
                            ColumnID = Convert.ToInt32(row["ColumnID"]),
                            field = row["ColumnName"].ToString(),
                            header = row["DisplayColumnName"].ToString(),
                            width = row["Width"] != DBNull.Value ? Convert.ToInt32(row["Width"]) : 150,
                            Alignment = row["Alignment"] != DBNull.Value ? Convert.ToInt32(row["Alignment"]) : 1,
                            Visible = row["Visible"] != DBNull.Value ? Convert.ToInt32(row["Visible"]) : 1,
                            IsHiddenColumn = row["IsHiddenColumn"] != DBNull.Value ? Convert.ToInt32(row["IsHiddenColumn"]) : 0,
                            DisplayIndex = row["DisplayIndex"] != DBNull.Value ? Convert.ToInt32(row["DisplayIndex"]) : 0,
                            DataType = row["DataType"] != DBNull.Value ? row["DataType"].ToString() : "string"
                        });
                    }
                    return Ok(new { success = true, reportColumns = list });
                }
                else
                {
                    return Ok(new { success = false, message = "No column configuration found for Report " + request.ReportID });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "API Error", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/report/GetUserAssignedTask")]
        public IHttpActionResult GetUserAssignedTask(int userId)
        {
            try
            {
                DataTable dt = bl.BL_ExecuteParamSP("GetUserAssignedTask", userId);
                List<object> list = new List<object>();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(new
                        {
                            SNo = row["SNo"].ToString(),
                            Description = row["Description"].ToString(),
                            DurationInHrs = row["DurationInHrs"].ToString()
                        });
                    }
                    return Ok(list);
                }
                else
                {
                    return Ok(new List<object>());
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/report/GetUserAssignedProgress")]
        public IHttpActionResult GetUserAssignedProgress(int userId)
        {
            try
            {
                DataTable dt = bl.BL_ExecuteParamSP("GetUserAssignedProgress", userId);
                List<object> list = new List<object>();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(new
                        {
                            CaseNo = row["CaseNo"].ToString(),
                            CustomerName = row["CustomerName"].ToString(),
                            Date = row["Date"].ToString(),
                            PurposeName = row["PurposeName"].ToString()
                        });
                    }
                    return Ok(list);
                }
                else
                {
                    return Ok(new List<object>());
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        [Route("api/columnsettings/getcolumnsettings")]
        public IHttpActionResult GetGendralColumnData(string Mode, string FormID, string TableID, string FormorReport)
        {
            if (Mode == "1")
            {
                DataTable dtResult = bl.BL_ExecuteParamSP("uspGetGendralColumnSettings", Mode, FormID, TableID, FormorReport);
                string JSONCONV = JsonConvert.SerializeObject(dtResult);
                return Ok(JSONCONV);
            }
            if (Mode == "2")
            {
                List<ColumnSettingsDataModel> list = new List<ColumnSettingsDataModel>();
                DataTable dtResult = bl.BL_ExecuteParamSP("uspGetGendralColumnSettings", Mode, FormID, TableID, FormorReport);
                for (int i = 0; i < dtResult.Rows.Count; i++)
                {
                    //field	header	type	width	align	visible	EnableColumnMenu	ShowinColumnOption	Total	TotalYN	EnableSum	EnableAvg	precision	ClickPopup
                    list.Add(new ColumnSettingsDataModel()
                    {
                        field = dtResult.Rows[i]["ColumnName"].ToString(),
                        header = dtResult.Rows[i]["DisplayColumnName"].ToString(),
                        type = dtResult.Rows[i]["Field"].ToString(),
                        width = Convert.ToInt32(dtResult.Rows[i]["Width"].ToString()),
                        align = dtResult.Rows[i]["Alignment"].ToString() == "1" ? "left" : dtResult.Rows[i]["Alignment"].ToString() == "2" ? "right" : "center",
                        visible = dtResult.Rows[i]["Visible"].ToString() == "1" ? true : false,
                        EnableColumnMenu = dtResult.Rows[i]["EnableColumnMenu"].ToString() == "1" ? true : false,
                        ShowinColumnOption = dtResult.Rows[i]["ShowinColumnOption"].ToString() == "0" ? false : true,
                        Total = dtResult.Rows[i]["Total"].ToString() == "0" ? true : false,
                        TotalYN = dtResult.Rows[i]["TotalYN"].ToString(),
                        EnableSum = dtResult.Rows[i]["EnableSum"].ToString() == "1" ? true : false,
                        EnableAvg = dtResult.Rows[i]["EnableAvg"].ToString() == "1" ? true : false,
                        EnableCount = dtResult.Rows[i]["EnableCount"].ToString() == "1" ? true : false,
                        EnableUnique = dtResult.Rows[i]["EnableUnique"].ToString() == "1" ? true : false,
                        ClickPopup = dtResult.Rows[i]["ClickPopup"].ToString() == "1" ? true : false,
                        precision = dtResult.Rows[i]["precision"].ToString(),
                        PrintYN = dtResult.Rows[i]["PrintYN"].ToString() == "1" ? true : false,
                        Printwidth = Convert.ToInt32(dtResult.Rows[i]["PrintWidth"].ToString()),
                        PrintColumnName = dtResult.Rows[i]["PrintColumnName"].ToString(),
                    });
                }
                return Ok(list);
            }
            return Ok();
        }
        [HttpPost]
        [Route("api/columnsettings/Savecolumnsettings")]
        public IHttpActionResult saveGenColumnData(List<ColumnSettingsModel> ColumnSettingData)
        {
            if (ColumnSettingData != null)
            {
                var list = new List<object>();
                foreach (ColumnSettingsModel item in ColumnSettingData)
                {
                    bl.BL_ExecuteParamSP("uspSaveGendralColumnSettings", 1, item.FormID, item.TableID, item.ColumnID, item.FormorReport,
                      item.DisplayColumnName, item.Width, item.Visible, item.Alignment, item.DisplayIndex, item.TotalYN, item.EnableSum,
                      item.EnableAvg, item.EnableCount, item.EnableUnique, item.EnableColumnMenu, item.ShowinColumnOption, item.PrintYN ? 1 : 0, item.PrintColumnName,
                      !string.IsNullOrEmpty(item.Printwidth.ToString()) ? item.Printwidth : 0,item.FieldType);
                }
                List<ColumnSettingsDataModel> Columnlist = new List<ColumnSettingsDataModel>();
                DataTable dtResult = bl.BL_ExecuteParamSP("uspGetGendralColumnSettings", 2, ColumnSettingData[0].FormID, ColumnSettingData[0].TableID, ColumnSettingData[0].FormorReport);
                for (int i = 0; i < dtResult.Rows.Count; i++)
                {
                    //field	header	type	width	align	visible	EnableColumnMenu	ShowinColumnOption	Total	TotalYN	EnableSum	EnableAvg	precision	ClickPopup
                    Columnlist.Add(new ColumnSettingsDataModel()
                    {
                        field = dtResult.Rows[i]["ColumnName"].ToString(),
                        header = dtResult.Rows[i]["DisplayColumnName"].ToString(),
                        type = dtResult.Rows[i]["Field"].ToString(),
                        width = Convert.ToInt32(dtResult.Rows[i]["Width"].ToString()),
                        align = dtResult.Rows[i]["Alignment"].ToString() == "1" ? "left" : dtResult.Rows[i]["Alignment"].ToString() == "2" ? "right" : "center",
                        visible = dtResult.Rows[i]["Visible"].ToString() == "1" ? true : false,
                        EnableColumnMenu = dtResult.Rows[i]["EnableColumnMenu"].ToString() == "1" ? true : false,
                        ShowinColumnOption = dtResult.Rows[i]["ShowinColumnOption"].ToString() == "0" ? false : true,
                        Total = dtResult.Rows[i]["Total"].ToString() == "0" ? true : false,
                        TotalYN = dtResult.Rows[i]["TotalYN"].ToString(),
                        EnableSum = dtResult.Rows[i]["EnableSum"].ToString() == "1" ? true : false,
                        EnableAvg = dtResult.Rows[i]["EnableAvg"].ToString() == "1" ? true : false,
                        EnableCount = dtResult.Rows[i]["EnableCount"].ToString() == "1" ? true : false,
                        EnableUnique = dtResult.Rows[i]["EnableUnique"].ToString() == "1" ? true : false,
                        ClickPopup = dtResult.Rows[i]["ClickPopup"].ToString() == "1" ? true : false,
                        precision = dtResult.Rows[i]["precision"].ToString(),
                        PrintYN = dtResult.Rows[i]["PrintYN"].ToString() == "1" ? true : false,
                        Printwidth = Convert.ToInt32(dtResult.Rows[i]["PrintWidth"].ToString()),
                        PrintColumnName = dtResult.Rows[i]["PrintColumnName"].ToString(),
                    });
                }
                list.Add(new
                {
                    MsgID = "0",
                    Message = "Saved Successfully",
                    ColumnData = Columnlist
                });
                return Ok(list);
            }
            return Ok();
        }
        [HttpGet]
        [Route("api/columnsettings/updatecolumnwidth")]
        public IHttpActionResult updateGendralColumnwdith(string FormID, string TableID, string FormorReport, string ColumnID, string ColumnName, string Width)
        {
            bl.BL_ExecuteParamSP("uspSaveGendralColumnSettings", 2, FormID, TableID, ColumnID, FormorReport,
                ColumnName, Width);
            return Ok();
        }
    }
}