using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

    
namespace SAP_Batch_GR_TR
{
    public partial class GRTR_Post_sap : Form
    {

        public GRTR_Post_sap()
        {
            InitializeComponent();
        }
        private void GRTRPost_sap(object sender, EventArgs e)
        {
            
            GetAndUpdate_Batch_GR_TR_Log();
            Post_GR_to_Sap();
            Post_TR_to_Sap();
            End_update();
            GetErrorAndNotify();
        }

        string start_Time = "";

        // บันทึกรอบเวลาการส่งข้อมูล
        private void GetAndUpdate_Batch_GR_TR_Log()
        {
            var sql = " select isnull(A.GR_NO,0)GR_NO, isnull(B.GR_Re_NO,0)GR_Re_NO, isnull(C.TR_NO,0)TR_NO, isnull(D.TR_Re_NO,0)TR_Re_NO,FORMAT(getdate(), 'yyyy-MM-dd HH:mm:ss:fff') as Start_Time  From (select count(*) GR_NO, Action from [Barcode].[dbo].[v_sap_batch_gr] where Action = 1 group by Action) A " +
                "left join(select count(*) GR_Re_NO, Action from [Barcode].[dbo].[v_sap_batch_gr_redo] where Action = 1 group by Action) B ON A.Action = B.Action " +
                "left join(select count(*) TR_NO, Action From (select count(*) TR_NO, SLIPNO, Action from [Barcode].[dbo].[v_sap_batch_tr] where Action = 1 GROUP BY SLIPNO, Action) C1 GROUP BY C1.Action ) C ON B.Action = C.Action or A.Action = C.Action " +
                "left join(select count(*) TR_Re_NO, Action From (select count(*) TR_Re_NO, SLIPNO, Action from [Barcode].[dbo].[v_sap_batch_tr_redo] where Action = 1 GROUP BY SLIPNO, Action)D1 Group by D1.Action) D ON C.Action = D.Action or B.Action = D.Action or A.Action = D.Action";
            var dt = GetQuery(sql);
            start_Time = dt.Rows[0]["Start_Time"].ToString();
            sql = "INSERT INTO [Barcode].[dbo].[T_SAP_Batch_GR_TR_Log] (GR_NO, GR_Re_NO,TR_NO,TR_Re_NO,Start_Time) VALUES (@GR_NO,@GR_Re_NO,@TR_NO,@TR_Re_NO,@Start_Time)";
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
            string connString = "";
            if (setting != null)
            {
                connString = setting.ConnectionString;
            }

            SqlConnection conn = new SqlConnection(connString);
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@GR_NO", dt.Rows[0]["GR_NO"].ToString());
                cmd.Parameters.AddWithValue("@GR_Re_NO", dt.Rows[0]["GR_Re_NO"].ToString());
                cmd.Parameters.AddWithValue("@TR_NO", dt.Rows[0]["TR_NO"].ToString());
                cmd.Parameters.AddWithValue("@TR_Re_NO", dt.Rows[0]["TR_Re_NO"].ToString());
                cmd.Parameters.AddWithValue("@Start_Time", dt.Rows[0]["Start_Time"].ToString());
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                conn.Close();


            }
        }



       

        private string GetAndUpdate_LogDataValidate_GR_to_Sap(String partno, int qty, String custid, String FacNo, String Plant, String store, int MvmntType, String postdate, String PostTime, String headertext, int Action, string Type)
        {

            string Message = "";
            string qtyType = qty.GetType().ToString();
            Message += partno.Length > 19 ? partno+"," : "".ToString().Trim();
            Message += qtyType != "System.Int32" ? "QRQty Type ," : "".ToString().Trim(); ;
            Message += qty.ToString().Length > 3 ? "QRQty ," : "".ToString().Trim();
            Message += custid.Length > 13 ? "Custid ," : "".ToString().Trim();
            Message += store.Length > 5 ? store + "," : "".ToString().Trim();
            Message += postdate.Length > 9 ? "CustID ," : "".ToString().Trim();
            Message += headertext.Length > 8001 ? "CustID ," : "".ToString().Trim();

            string ValidateMessage = "";
            if (Message != "") {
                Message = Message.Substring(0, Message.Length - 1);
                ValidateMessage = "\n( " + "Error " + partno + ": " + Message + ")";
            }
            else {
                 ValidateMessage = "";
            }
            //string Status = Message.Length > 0 ? "inprogress" : "Error".ToString().Trim();


            var sql = "INSERT INTO [Barcode].[dbo].[T_LogDatavalidate_GR_to_Sap] " +
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
            return ValidateMessage;
        }

        private void GetAndUpdate_LogDataValidate_TR_to_Sap(string Slipno, string Datatype, string Type)
        {
            string Message = "";

            Message += Slipno.Length > 1 ? "Slipno ," : "".ToString().Trim();
            Message += Datatype.Length > 1 ? "Datatype ," : "".ToString().Trim();
            
            // string ValidateMessage = "Error : ( " + Message + ")".ToString().Trim();
            //string Status = Message.Length > 0 ? "inprogress" : "Error".ToString().Trim();
            string ValidateMessage = "";
            if (Message != "")
            {
                Message = Message.Substring(0, Message.Length - 1);
                ValidateMessage = "\n( " + "Error " + Slipno + ": " + Message + ")";
            }
            else
            {
                ValidateMessage = "";
            }

            var sql = "INSERT INTO [Barcode].[dbo].[T_LogDatavalidate_TR_to_Sap] " +
                "(SlipNo ,ValidateMessage ,Type, CreateDate ,Datatype) " +
                "VALUES " +
                "(@SlipNo ,@ValidateMessage ,@Type,@CreateDate ,@Datatype)";

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
                //cmd.Parameters.AddWithValue("@Status", Status);
                cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                cmd.Parameters.AddWithValue("@Datatype", Datatype);
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                conn.Close();
            }

        }
        private void Post_GR_to_Sap()
        {
            string sql = "";
            //GR
            string partno = "";
            int qty = 0;
            string custid = "";
            string FacNo = "";
            string Plant = "";
            string store = "";
            int MvmntType = 0;
            string postdate = "";
            string PostTime = "";
            string headertext = "";
            int Action = 0;
            string Type = "";
            string Status = "";
            DataTable GRdata = new DataTable();
            DataTable GRErrdata = new DataTable();
            var ws = new SapTransfer.post();



            sql = "select * from [Barcode].[dbo].[v_sap_batch_gr] where Action = 1";
            GRdata = GetQuery(sql);
            sql = "select * from [Barcode].[dbo].[v_sap_batch_gr_redo] where Action = 1";
            GRErrdata = GetQuery(sql);

            if (GRdata.Rows.Count > 0)
            {
                //string ValidateMessage = "Error :";
                foreach (DataRow item in GRdata.Rows)
                {


                    partno = item["MatNo"].ToString().Trim();
                    qty = Convert.ToInt32(item["QRQty"].ToString());
                    custid = item["CustID"].ToString().Trim();
                    FacNo = item["FacNo"].ToString().Trim();
                    Plant = item["Plant"].ToString().Trim();
                    store = item["SLoc"].ToString().Trim();
                    MvmntType = Convert.ToInt32(item["MvmntType"].ToString());
                    postdate = item["PostDate"].ToString().Trim();
                    PostTime = item["PostTime"].ToString().Trim();
                    headertext = "IT|" + item["HeaderText"].ToString().Trim();
                    Action = Convert.ToInt32(item["Action"].ToString());
                    Type = "GR".ToString().Trim();

                    string DataValidateMessage = GetAndUpdate_LogDataValidate_GR_to_Sap(partno, qty, custid, FacNo, Plant, store, MvmntType, postdate, PostTime, headertext, Action, Type);
                    //ValidateMessage = ValidateMessage + DataValidateMessage;
                    //var res = ws.ADDSTOCKBYEXCEL(partno, qty, custid, store, postdate, headertext);

                }

                //Class.LineNotify lineNotify = new Class.LineNotify();
                //lineNotify.FNLineNotify(ValidateMessage);
            }
            if (GRErrdata.Rows.Count > 0)
            {
                foreach (DataRow item in GRErrdata.Rows)
                {

                    partno = item["MatNo"].ToString().Trim();
                    qty = Convert.ToInt32(item["QRQty"].ToString());
                    custid = item["CustID"].ToString().Trim();
                    FacNo = item["FacNo"].ToString().Trim();
                    Plant = item["Plant"].ToString().Trim();
                    store = item["SLoc"].ToString().Trim();
                    MvmntType = Convert.ToInt32(item["MvmntType"].ToString());
                    postdate = item["PostDate"].ToString().Trim();
                    PostTime = item["PostTime"].ToString().Trim();
                    headertext = "IT|" + item["HeaderText"].ToString().Trim();
                    Action = Convert.ToInt32(item["Action"].ToString());
                    Type = "GR_redo".ToString().Trim();

                    GetAndUpdate_LogDataValidate_GR_to_Sap(partno, qty, custid, FacNo, Plant, store, MvmntType, postdate, PostTime, headertext, Action, Type);
                    //var res = ws.ADDSTOCKBYEXCEL(partno, qty, custid, store, postdate, headertext);

                }
            }



        }


        private void Post_TR_to_Sap()
        {
            string sql = "";
            string Slipno = "";
            string Datatype = "";
            string Type = "";
            DataTable TRdata = new DataTable();
            DataTable TRErrdata = new DataTable();

            var ws = new SapTransfer.post();

            sql = "select count(*) ,SLIPNO from [Barcode].[dbo].[v_sap_batch_tr] where Action = 1 GROUP BY SLIPNO";
            TRdata = GetQuery(sql);
            sql = "select count(*) ,SLIPNO from [Barcode].[dbo].[v_sap_batch_tr_redo] where Action = 1 GROUP BY SLIPNO";
            TRErrdata = GetQuery(sql);

            if (TRdata.Rows.Count > 0)
            {
                foreach (DataRow item in TRdata.Rows)
                {
                    Slipno = "IT|" + item["SLIPNO"].ToString().Trim();
                    Datatype = "12";
                    Type = "TR";
                    GetAndUpdate_LogDataValidate_TR_to_Sap(Slipno, Datatype, Type);
                    //var res = ws.TransferStockDataToSAP_311(Slipno, Datatype);
                }
            }

            if (TRErrdata.Rows.Count > 0)
            {
                foreach (DataRow item in TRErrdata.Rows)
                {
                    Slipno = "IT|" + item["SLIPNO"].ToString().Trim();
                    Datatype = "13";
                    Type = "TR_redo";
                    GetAndUpdate_LogDataValidate_TR_to_Sap(Slipno, Datatype, Type);
                    //var res = ws.TransferStockDataToSAP_311(Slipno, Datatype);
                }
            }
        }

        private void GetErrorAndNotify()
        {
            string sql = "";
            string Datatype = "";
            string Start_Time = "";
            string End_Time = "";
            string ErrorID = "";
            DataTable GetRoundData = new DataTable();
            DataTable GetRoundDataError = new DataTable();
            
            sql = "SELECT TOP 1 * FROM [Barcode].[dbo].[T_SAP_Batch_GR_TR_Log] order by ID desc";
            GetRoundData = GetQuery(sql);

            if (GetRoundData.Rows.Count > 0)
            {
                foreach (DataRow item in GetRoundData.Rows)
                {

                    Start_Time = item["Start_Time"].ToString().Trim(); ;
                    End_Time = item["End_Time"].ToString().Trim(); ;
                }
            }

            //sql = "SELECT * FROM [Barcode].[dbo].[T_LOG_STOCK_ERROR] WHERE UpdDate BETWEEN '"+ Start_Time + "'  AND '"+ End_Time+"'";
            sql = "SELECT * FROM [Barcode].[dbo].[T_LOG_STOCK_ERROR] WHERE UpdDate BETWEEN '2023-10-20 15:30:20.100'  AND '2023-10-20 15:30:20.100'";
            GetRoundDataError = GetQuery(sql);
           
            if (GetRoundDataError.Rows.Count > 0)
            {
                foreach (DataRow item in GetRoundDataError.Rows)
                {

                    ErrorID += item["ID"].ToString() + ", ";
                }
                if (ErrorID == "") {
                    string ValidateMessage =  "Completed : \nรอบ\n " + Start_Time + " \n " + End_Time ;
                    Class.LineNotify lineNotify = new Class.LineNotify();
                    lineNotify.FNLineNotify(ValidateMessage);
                }
                else {
                    string ValidateMessage = "Error : \nรอบ\n " + "2023-10-20 15:30:20.100" + " \n " + "2023-10-20 15:30:20.100" + "\n Error Item : ( " + ErrorID + " )";
                    //string ValidateMessage = "Error : \nรอบ\n " + Start_Time +" \n "+  End_Time + "\n Error Item : ( "+ ErrorID + " )";
                    Class.LineNotify lineNotify = new Class.LineNotify();
                    lineNotify.FNLineNotify(ValidateMessage);
                  
                }
            }
        }

        private void End_update()
        {
            var sql = "UPDATE [Barcode].[dbo].[T_SAP_Batch_GR_TR_Log] SET End_Time = @End_Time where Start_Time = '" + start_Time + "'";

            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
            string connString = "";
            if (setting != null)
            {
                connString = setting.ConnectionString;
            }

            SqlConnection conn = new SqlConnection(connString);
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@End_Time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                conn.Close();
            }

      
        }

        public DataTable GetQuery(string sql)
        {
            var dt = new DataTable();

            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
            string connString = "";
            if (setting != null)
            {
                connString = setting.ConnectionString;
            }

            SqlConnection conn = new SqlConnection(connString);
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                conn.Open();
                da.Fill(dt);
                conn.Close();
                da.Dispose();
            }

            return dt;
        }
       

    }
}
