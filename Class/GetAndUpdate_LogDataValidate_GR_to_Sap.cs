using System;
using System.Data.SqlClient;
using System.Configuration;

namespace SAP_Batch_GR_TR.Class
{
    class GetAndUpdate_LogDataValidate_GR_to_Sap
    {
        internal void  LogDataValidate_GR_to_Sap(String partno, int qty, String custid, String FacNo, String Plant, String store, int MvmntType, String postdate, String PostTime, String headertext, int Action, string Type , string ValidateMessage)
        {
            LogDataValidate_GR_to_SapClass(partno, qty, custid, FacNo, Plant, store, MvmntType, postdate, PostTime, headertext, Action, Type);
        }

        public string LogDataValidate_GR_to_SapClass(String partno, int qty, String custid, String FacNo, String Plant, String store, int MvmntType, String postdate, String PostTime, String headertext, int Action, string Type)
        {

            string Message = "";

            Message += partno.Length > 1 ? partno + "," : "".ToString().Trim();
            //Message += qty.Length > 1 ? "MatNo ," : "".ToString().Trim();
            Message += custid.Length > 1 ? custid + " ," : "".ToString().Trim();
            Message += store.Length > 1 ? store + "," : "".ToString().Trim();
            // Message += postdate.Length > 1 ? "CustID ," : "".ToString().Trim();
            // Message += headertext.Length > 1 ? "CustID ," : "".ToString().Trim();

            Message = Message.Substring(0, Message.Length - 1);
            string ValidateMessage = "\n( " + partno + ": " + Message + ")";
            string Status = Message.Length > 0 ? "inprogress" : "Error".ToString().Trim();


            var sql = "INSERT INTO [Barcode].[dbo].[T_LogDatavalidate_GR_to_Sap] " +
                "(MatNo, CustID, FacNo, Plant, SLoc, MvmntType, PostDate, PostTime, QRQty, HeaderText, Action ,Type ,Status, CreateDate ,ValidateMessage) " +
                "VALUES " +
                "(@MatNo,@CustID,@FacNo,@Plant,@SLoc,@MvmntType,@PostDate,@PostTime,@QRQty,@HeaderText,@Action,@Type,@Status,@CreateDate  ,@ValidateMessage)";

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
                cmd.Parameters.AddWithValue("@Status", Status);
                cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                cmd.Parameters.AddWithValue("@ValidateMessage", ValidateMessage);
                conn.Open();

                int result = cmd.ExecuteNonQuery();
                conn.Close();


            }
            return ValidateMessage;
        }
    }
}
