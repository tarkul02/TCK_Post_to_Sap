using System;
using System.Data.SqlClient;
using System.Configuration;

namespace SAP_Batch_GR_TR.Class
{
    class GetAndUpdate_LogDataValidate_TR_to_Sap
    {
        internal void LogDataValidate_TR_to_Sap(string Slipno, string Datatype, string Type , string ValidateMessage)
        {
            LogDataValidate_TR_to_SapClass( Slipno,  Datatype,  Type);
        }
        public string LogDataValidate_TR_to_SapClass(string Slipno, string Datatype, string Type)
        {
            string Message = "";

            Message += Slipno.Length > 1 ? "Slipno ," : "".ToString().Trim();
            Message = Message.Substring(0, Message.Length - 1);
            string ValidateMessage = "Error : ( " + Message + ")".ToString().Trim();

            string Status = Message.Length > 0 ? "inprogress" : "Error".ToString().Trim();


            var sql = "INSERT INTO [Barcode].[dbo].[T_LogDatavalidate_TR_to_Sap] " +
                "(SlipNo ,ValidateMessage ,Type ,Status, CreateDate ,Datatype) " +
                "VALUES " +
                "(@SlipNo ,@ValidateMessage ,@Type,@Status,@CreateDate ,@Datatype)";

            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
            string connString = "";
            if (setting != null)
            {
                connString = setting.ConnectionString;
            }

            SqlConnection conn = new SqlConnection(connString);
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@SlipNo", Slipno);
                cmd.Parameters.AddWithValue("@ValidateMessage", ValidateMessage);
                cmd.Parameters.AddWithValue("@Type", Type);
                cmd.Parameters.AddWithValue("@Status", Status);
                cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                cmd.Parameters.AddWithValue("@Datatype", Datatype);
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                conn.Close();
            }
            return ValidateMessage;
        }
    }
}
