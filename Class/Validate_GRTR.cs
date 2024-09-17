using SAP_Batch_GR_TR.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostSap_GR_TR.Class
{
    class Validate_GRTR
    {
        string DBconfig = ConfigurationManager.AppSettings["Databaseconfig"];
        string checkError;
        public string GetAndUpdate_LogDataValidate_GR_to_Sap(String partno, int qty, String custid, String FacNo, String Plant, String store, int MvmntType, String postdate, String PostTime, String headertext, int Action, string Type)
        {
            string Message = "";
            string qtyType = qty.ToString();
            string checkdate = "";
            DateTime now = DateTime.Now;
            string datenow = now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            int checkYear = now.Year;
            string lowYear = (checkYear - 1).ToString();

            if (datenow.Substring(5, 10) == "-01-01")
            {
                checkdate += postdate.Length == 8 ? "" : "error";
                checkdate += postdate.Substring(0, 4) == lowYear ? "" : "error";
            }
            else
            {
                checkdate += postdate.Length == 8 ? "" : "error";
                checkdate += postdate.Substring(0, 4) == checkYear.ToString() ? "" : "error";
            }

            Message += partno.Length == 14 ? "" : "MatNo ,".ToString().Trim();
            Message += qtyType.Length < 4 ? "" : "QRQty ,".ToString().Trim();
            Message += custid.Length == 4 ? "" : "Custid ,".ToString().Trim();
            Message += store.Length < 5 ? "" : "store ,".ToString().Trim();
            Message += checkdate.Length > 0 ? "Postdate ," : "".ToString().Trim();
            Message += headertext.Length < 30 ? "" : "headertext ,".ToString().Trim();

            string ValidateMessage = "";
            if (Message != "")
            {
                Message = Message.Substring(0, Message.Length - 1);
                ValidateMessage = "Error : ( " + Message + ")";
            }
            else
            {
                ValidateMessage = "";
            }

            var sql = "INSERT INTO "+ DBconfig +".[T_LogDatavalidate_GR_to_Sap] " +
                "(MatNo, CustID, FacNo, Plant, SLoc, MvmntType, PostDate, PostTime, QRQty, HeaderText, Action ,Type , CreateDate ,ValidateMessage) " +
                "VALUES " +
                "(@MatNo,@CustID,@FacNo,@Plant,@SLoc,@MvmntType,@PostDate,@PostTime,@QRQty,@HeaderText,@Action,@Type,@CreateDate  ,@ValidateMessage)";

            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
            string connString = "";
            if (setting != null)
            {
                connString = setting.ConnectionString;
            }

            SqlConnection conn = new SqlConnection(connString);
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MatNo", partno);
                cmd.Parameters.AddWithValue("@CustID", custid);
                cmd.Parameters.AddWithValue("@FacNo", FacNo);
                cmd.Parameters.AddWithValue("@Plant", Plant);
                cmd.Parameters.AddWithValue("@SLoc", store);
                cmd.Parameters.AddWithValue("@MvmntType", MvmntType);
                cmd.Parameters.AddWithValue("@PostDate", postdate);
                cmd.Parameters.AddWithValue("@PostTime", PostTime);
                cmd.Parameters.AddWithValue("@QRQty", qty);
                cmd.Parameters.AddWithValue("@HeaderText", headertext);
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.Parameters.AddWithValue("@Type", Type);
                //cmd.Parameters.AddWithValue("@Status", Status);
                cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                cmd.Parameters.AddWithValue("@ValidateMessage", ValidateMessage);
                conn.Open();

                int result = cmd.ExecuteNonQuery();
                conn.Close();
            }
            _ = new DataTable();
            _ = new Class.ServicePostSapGR();
            Class.Condb Condb = new Class.Condb();
            string sqlgetID = "SELECT TOP (1) [ID] FROM "+ DBconfig +".[T_LogDatavalidate_GR_to_Sap] where MatNo = '" + partno + "' order by ID desc";
            var getID = Condb.GetQuery(sqlgetID);
            string lastID = getID.Rows[0]["ID"].ToString();
            return lastID;
        }
        public string GetAndUpdate_LogDataValidate_TR_to_Sap(string checkSlipno, string Datatype, string Type , string Plant, string StgeLoc, string EntryQnt, string MovePlant, string MoveStloc, string Kanban, string PostDate, string start_Time , string Mat_Type)
        {
            try
            {
                ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
                string connString = "";
                if (setting != null)
                {
                    connString = setting.ConnectionString;
                }

                SqlConnection conn = new SqlConnection(connString);

             
                Class.Condb Condb = new Class.Condb();
                checkError += "1";
             
                checkError += "2";
                string Message = "";
                //Message += checkSlipno.Length == 14 ? "" : "Slipno ,".ToString().Trim();
                Message += Datatype.Length == 2 ? "" : "Datatype ,".ToString().Trim();
                string ValidateMessage = "";
                var sql = "INSERT INTO " + DBconfig + ".[T_LogDatavalidate_TR_to_Sap] " +
                   "(PlantFrom, StorageFrom, PlantTo, StorageTo, Kanban, MvmntQty, SlipNo, Mat_Type, ValidateMessage, Type, CreateDate, Datatype) " +
                   "VALUES " +
                   "(@PlantFrom, @StorageFrom, @PlantTo, @StorageTo, @Kanban, @MvmntQty, @SlipNo, @Mat_Type, @ValidateMessage, @Type, @CreateDate, @Datatype);" +
                   "SELECT SCOPE_IDENTITY();";  // ดึง ID ของแถวที่เพิ่งถูกแทรก
                checkError += "3";
                if (Message != "")
                {
                    Message = Message.Substring(0, Message.Length - 1);
                    ValidateMessage = "Error : ( " + Message + ")";
                }
                else
                {
                    ValidateMessage = "";
                }
                //var result = "";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // ใส่ค่า Parameters
                    cmd.Parameters.AddWithValue("@PlantFrom", Plant);
                    cmd.Parameters.AddWithValue("@StorageFrom", StgeLoc);
                    cmd.Parameters.AddWithValue("@PlantTo", MovePlant);
                    cmd.Parameters.AddWithValue("@StorageTo", MoveStloc);
                    cmd.Parameters.AddWithValue("@Kanban", Kanban);
                    cmd.Parameters.AddWithValue("@MvmntQty", EntryQnt);
                    cmd.Parameters.AddWithValue("@SlipNo", checkSlipno);
                    cmd.Parameters.AddWithValue("@Mat_Type", Mat_Type);
                    cmd.Parameters.AddWithValue("@ValidateMessage", ValidateMessage);
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                    cmd.Parameters.AddWithValue("@Datatype", Datatype);

                    conn.Open();

                    // รับค่า ID ที่เพิ่งแทรกกลับมา
                    var result = cmd.ExecuteScalar();

                    conn.Close();

                    // แปลง result ให้เป็น int และส่งกลับ
                    return result.ToString();
                }

                //_ = new DataTable();
                //_ = new Class.ServicePostSapGR();
                //string sqlgetID = "SELECT TOP (1) [ID] FROM " + DBconfig + ".[T_LogDatavalidate_TR_to_Sap] where SlipNo = '" + checkSlipno + "' order by ID desc";
                //var getID = Condb.GetQuery(sqlgetID);
                //checkError += "6";
                //string lastID = getID.Rows[0]["ID"].ToString();
            }
            catch (Exception ex)
            {
                string Message = "Unexpected error Post_TR_to_Sap validate: " + ex.Message;
                GRTR_Post_sap checkError = new GRTR_Post_sap();
                checkError.CatchError("checkError :" + checkError +","+ Message );
                return "";
            }
        }
        public string GetAndUpdate_saveLogData_GI_to_Sap(string OrderNo, string checkPoAndDO, string Type ,string SLoc)
        {
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
            string connString = "";
            if (setting != null)
            {
                connString = setting.ConnectionString;
            }

            SqlConnection conn = new SqlConnection(connString);
            DataTable getdata_GI_and_GIredo = new DataTable();
            Class.Condb Condb = new Class.Condb();
            var sql = "INSERT INTO "+ DBconfig +".[T_LogDatavalidate_GI_to_Sap] " +
              "(OrderNo ,ValidateMessage ,Type, CreateDate ,Datatype , SLoc) " +
              "VALUES " +
              "(@OrderNo ,@ValidateMessage ,@Type,@CreateDate ,@Datatype ,@SLoc)";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@OrderNo", OrderNo);
                cmd.Parameters.AddWithValue("@Type", Type);
                cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                cmd.Parameters.AddWithValue("@Datatype", checkPoAndDO);
                cmd.Parameters.AddWithValue("@SLoc", SLoc);
                cmd.Parameters.AddWithValue("@ValidateMessage", "");
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                conn.Close();
            }
            _ = new DataTable();
            _ = new Class.ServicePostSapGR();
            string sqlgetID = "SELECT TOP (1) [ID] FROM "+ DBconfig +".[T_LogDatavalidate_GI_to_Sap] where OrderNo = '" + OrderNo + "' order by ID desc";
            var getID = Condb.GetQuery(sqlgetID);
            string lastID = getID.Rows[0]["ID"].ToString();
            return lastID;
        }
    }
}
