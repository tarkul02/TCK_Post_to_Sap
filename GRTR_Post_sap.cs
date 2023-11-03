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
            Application.Exit();
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

        

        private void End_update()
        {
            var sql = "UPDATE [Barcode].[dbo].[T_SAP_Batch_GR_TR_Log] SET End_Time = @End_Time where Start_Time = '"+ start_Time + "'";
            
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
            string DataValidateMessage = "";
            string ValidateMessage = "Error :";


            sql = "select * from [Barcode].[dbo].[v_sap_batch_gr] where Action = 1";
            GRdata = GetQuery(sql);
            sql = "select * from [Barcode].[dbo].[v_sap_batch_gr_redo] where Action = 1";
            GRErrdata = GetQuery(sql);
            Class.GetAndUpdate_LogDataValidate_GR_to_Sap LogDataValidate_GR_to_Sap = new Class.GetAndUpdate_LogDataValidate_GR_to_Sap();
            if (GRdata.Rows.Count > 0)
            {

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

                    DataValidateMessage =  LogDataValidate_GR_to_Sap.LogDataValidate_GR_to_SapClass(partno, qty, custid, FacNo, Plant, store, MvmntType, postdate, PostTime, headertext, Action, Type);
                    ValidateMessage = ValidateMessage + DataValidateMessage;
                    //var res = ws.ADDSTOCKBYEXCEL(partno, qty, custid, store, postdate, headertext);

                }
                ValidateMessage = "aa";
                Class.LineNotify lineNotify = new Class.LineNotify();
                lineNotify.FNLineNotify(ValidateMessage);
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

                    //GetAndUpdate_LogDataValidate_GR_to_Sap(partno, qty, custid, FacNo, Plant, store, MvmntType, postdate, PostTime, headertext, Action, Type);
                    var res = ws.ADDSTOCKBYEXCEL(partno, qty, custid, store, postdate, headertext);

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
            string DataValidateMessage = "";
            string ValidateMessage = "Error :";

            var ws = new SapTransfer.post();

            sql = "select count(*) ,SLIPNO from [Barcode].[dbo].[v_sap_batch_tr] where Action = 1 GROUP BY SLIPNO";
            TRdata = GetQuery(sql);
            sql = "select count(*) ,SLIPNO from [Barcode].[dbo].[v_sap_batch_tr_redo] where Action = 1 GROUP BY SLIPNO";
            TRErrdata = GetQuery(sql);

            Class.GetAndUpdate_LogDataValidate_TR_to_Sap LogDataValidate_TR_to_Sap = new Class.GetAndUpdate_LogDataValidate_TR_to_Sap();
            if (TRdata.Rows.Count > 0)
            {
                foreach (DataRow item in TRdata.Rows)
                {
                    Slipno = "IT|" + item["SLIPNO"].ToString().Trim();
                    Datatype = "12";
                    Type = "TR";
                    DataValidateMessage = LogDataValidate_TR_to_Sap.LogDataValidate_TR_to_SapClass(Slipno, Datatype, Type);
                    ValidateMessage = ValidateMessage + DataValidateMessage;
                    //var res = ws.TransferStockDataToSAP_311(Slipno, Datatype);
                }
                Class.LineNotify lineNotify = new Class.LineNotify();
                lineNotify.FNLineNotify(ValidateMessage);
            }

            if (TRErrdata.Rows.Count > 0)
            {
                foreach (DataRow item in TRErrdata.Rows)
                {
                    Slipno = "IT|" + item["SLIPNO"].ToString().Trim();
                    Datatype = "13";
                    Type = "TR_redo";
                    //GetAndUpdate_LogDataValidate_TR_to_Sap(Slipno, Datatype , Type);
                    var res = ws.TransferStockDataToSAP_311(Slipno, Datatype);
                }
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
