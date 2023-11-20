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

using System.IO;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;
using DEV_Z_GOODSMVT_CREATE1.Class;

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
            Console.WriteLine("start GetAndUpdate_Batch_GR_TR_Log");
            GetAndUpdate_Batch_GR_TR_Log();
            Console.WriteLine("end GetAndUpdate_Batch_GR_TR_Log");
            Console.WriteLine("start Post_GR_to_Sap");
            Post_GR_to_Sap();
            Console.WriteLine("end Post_GR_to_Sap");
            Console.WriteLine("start Post_TR_to_Sap");
            Post_TR_to_Sap();
            Console.WriteLine("end Post_TR_to_Sap");
            Console.WriteLine("start End_update");
            End_update();
            Console.WriteLine("end End_update");
            Console.WriteLine("start GetErrorAndNotify");
            GetErrorAndNotify();
            Console.WriteLine("end GetErrorAndNotify");
            //Application.Exit();
        }

        string start_Time = "";

        // บันทึกรอบเวลาการส่งข้อมูล
        private void GetAndUpdate_Batch_GR_TR_Log()
        {

            var sql = " select isnull(A.GR_NO,0)GR_NO, isnull(B.GR_Re_NO,0)GR_Re_NO, isnull(C.TR_NO,0)TR_NO, isnull(D.TR_Re_NO,0)TR_Re_NO,FORMAT(getdate(), 'yyyy-MM-dd HH:mm:ss:fff') as Start_Time  From (select count(*) GR_NO, Action from [Barcode_DEV].[dbo].[v_sap_batch_gr] where Action = 1 group by Action) A " +
                "left join(select count(*) GR_Re_NO, Action from [Barcode_DEV].[dbo].[v_sap_batch_gr_redo] where Action = 1 group by Action) B ON A.Action = B.Action " +
                "left join(select count(*) TR_NO, Action From (select count(*) TR_NO, SLIPNO, Action from [Barcode_DEV].[dbo].[v_sap_batch_tr] where Action = 1 GROUP BY SLIPNO, Action) C1 GROUP BY C1.Action ) C ON B.Action = C.Action or A.Action = C.Action " +
                "left join(select count(*) TR_Re_NO, Action From (select count(*) TR_Re_NO, SLIPNO, Action from [Barcode_DEV].[dbo].[v_sap_batch_tr_redo] where Action = 1 GROUP BY SLIPNO, Action)D1 Group by D1.Action) D ON C.Action = D.Action or B.Action = D.Action or A.Action = D.Action";
            Console.WriteLine("call database");
            var dt = GetQuery(sql);
            Console.WriteLine("call database seccess");
            start_Time = dt.Rows[0]["Start_Time"].ToString();
            Console.WriteLine("call database seccess");
            sql = "INSERT INTO [Barcode_DEV].[dbo].[T_SAP_Batch_GR_TR_Log] (GR_NO, GR_Re_NO,TR_NO,TR_Re_NO,Start_Time) VALUES (@GR_NO,@GR_Re_NO,@TR_NO,@TR_Re_NO,@Start_Time)";
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
            var ws = new SAP_Batch_GR_TR.SapTransfer.post();

            sql = "select * from [Barcode_DEV].[dbo].[testGR] where Action = 1";
            GRdata = GetQuery(sql);
            sql = "select * from [Barcode_DEV].[dbo].[v_sap_batch_gr_redo] where Action = 1";
            GRErrdata = GetQuery(sql);

            if (GRdata.Rows.Count > 0)
            {
                foreach (DataRow item in GRdata.Rows)
                {
                    partno = "IT|" + item["MatNo"].ToString().Trim();
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

                    GetAndUpdate_LogDataValidate_GR_to_Sap(partno, qty, custid, FacNo, Plant, store, MvmntType, postdate, PostTime, headertext, Action, Type);
                    //var res = ws.ADDSTOCKBYEXCEL(partno, qty, custid, store, postdate, headertext);
                    //TestsendSap(partno, qty, custid, store, postdate, headertext);
                }
            }
            if (GRErrdata.Rows.Count > 0)
            {
                foreach (DataRow item in GRErrdata.Rows)
                {
                    partno = "IT|" + item["MatNo"].ToString().Trim();
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
            //sql = "select count(*) ,SLIPNO from [Barcode_DEV].[dbo].[v_sap_batch_tr] where Action = 1 GROUP BY SLIPNO";
            sql = "select * from [Barcode_DEV].[dbo].[testTR] where 1 = 1";
            TRdata = GetQuery(sql);
            sql = "select count(*) ,SLIPNO from [Barcode_DEV].[dbo].[v_sap_batch_tr_redo] where Action = 1 GROUP BY SLIPNO";
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
        private string GetAndUpdate_LogDataValidate_GR_to_Sap(String partno, int qty, String custid, String FacNo, String Plant, String store, int MvmntType, String postdate, String PostTime, String headertext, int Action, string Type)
        {
            string Message = "";
            string qtyType = qty.ToString();
            string checkdate = "";
            DateTime now = DateTime.Now;
            string datenow = now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            int checkYear = now.Year ;
            string lowYear = (checkYear - 1).ToString();
            string checkMonth = now.Month.ToString();
            string checkDay = now.Day.ToString();

            if (datenow.Substring(5, 10) == "-01-01")
            {
                checkdate += postdate.Length == 8 ? "" : "error";
                checkdate += postdate.Substring(0, 4) == lowYear ? "" : "error";
            }
            else {
                checkdate += postdate.Length == 8 ? "" : "error";
                checkdate += postdate.Substring(0, 4) == checkYear.ToString() ? "" : "error";
            }

            Message += partno.Length  == 17 ? "" : "partno ,".ToString().Trim();
            Message += qtyType.Length < 4 ? "" : "QRQty ,".ToString().Trim();
            Message += custid.Length == 4 ? "" : "Custid ,".ToString().Trim();
            Message += store.Length < 5 ? "" : "store ,".ToString().Trim();
            Message += checkdate.Length > 0 ? "Postdate ," : "".ToString().Trim();
            Message += headertext.Length == 18 ? "" : "headertext ,".ToString().Trim();

            string ValidateMessage = "";
            if (Message != "") {
                Message = Message.Substring(0, Message.Length - 1);
                ValidateMessage = "Error : ( "+ Message + ")";
            }
            else {
                 ValidateMessage = "";
            }

            var sql = "INSERT INTO [Barcode_DEV].[dbo].[T_LogDatavalidate_GR_to_Sap] " +
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
            Message += Slipno.Length == 17 ? "" : "Slipno ,".ToString().Trim();
            Message += Datatype.Length == 2 ? "" : "Datatype ,".ToString().Trim();
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

            var sql = "INSERT INTO [Barcode_DEV].[dbo].[T_LogDatavalidate_TR_to_Sap] " +
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

        private void GetErrorAndNotify()
        {
            DataTable GetDataErrorGR = new DataTable();
            DataTable GetDataErrorTR = new DataTable();
            DataTable GetDataErrorGRrow = new DataTable();
            DataTable GetDataErrorTRrow= new DataTable();
            string checkTime = DateTime.Now.ToString("HH:mm");
          
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
            string connString = "";
            if (setting != null)
            {
                connString = setting.ConnectionString;
            }
            string sql1 = "select id as No , RefDocNo as DocNo , EMessage from [Barcode_DEV].[dbo].[v_sap_batch_gr_redo] where Action = 1";
            GetDataErrorGR = GetQuery(sql1);
            string sql2 = "select id as No , RefDocNo as DocNo , EMessage  from [Barcode_DEV].[dbo].[v_sap_batch_tr_redo] where Action = 1";
            GetDataErrorTR = GetQuery(sql2);
            string sqllineGR = "select count(*) as totalSum  from [Barcode_DEV].[dbo].[v_sap_batch_gr_redo] where Action = 1";
            GetDataErrorGRrow = GetQuery(sqllineGR);
            string sqllineTR = "select count(*) as totalSum  from [Barcode_DEV].[dbo].[v_sap_batch_tr_redo] where Action = 1";
            GetDataErrorTRrow = GetQuery(sqllineTR);

            string checkdata1 = GetDataErrorGRrow.Rows[0]["totalSum"].ToString();
            string checkdata2 = GetDataErrorTRrow.Rows[0]["totalSum"].ToString();
            string MessagelistGR = int.Parse(checkdata1) > 0 ? "GR Error  " + checkdata1 + " Item" : "";
            string MessagelistTR = int.Parse(checkdata2) > 0 ? "TR Error  " + checkdata2 + " Item" : "";

            // if (checkTime == "08:00" || checkTime == "20:00")
            //{
            // start line notify 
            //if (int.Parse(checkdata1) > 0 || int.Parse(checkdata2) > 0) {
            string ValidateMessage = "Error  \nrun time =  " + checkTime + "\n" + MessagelistGR + "\n" + MessagelistTR;
                    Class.LineNotify lineNotify = new Class.LineNotify();
                    lineNotify.FNLineNotify(ValidateMessage);
            //}
            // end line notify
            // start cerate file and send mail
            if (GetDataErrorGR.Rows.Count == 0 || GetDataErrorTR.Rows.Count == 0)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                DateTime now = DateTime.Now;
                string filename = now.ToString("yyyy-MM-dd HH:mm:ss:fff");



                string[] words = filename.Split(' ');
                string[] text1 = words[0].Split('-');
                string[] text2 = words[1].Split(':');
                string lastfilename = text2[0] + text1[2] + text1[1] + text1[0];

                using (var package = new ExcelPackage())
                {

                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Data");

                    // Write column headers
                    if (GetDataErrorGR.Rows.Count == 0)
                    {
                        for (int i = 0; i < GetDataErrorGR.Columns.Count; i++)
                        {
                            worksheet.Cells[1, i + 1].Value = GetDataErrorGR.Columns[i].ColumnName;
                        }

                        // Write data to Excel file
                        for (int row = 0; row < GetDataErrorGR.Rows.Count; row++)
                        {
                            for (int column = 0; column < GetDataErrorGR.Columns.Count; column++)
                            {
                                worksheet.Cells[row + 2, column + 1].Value = GetDataErrorGR.Rows[row][column];
                            }
                        }

                        FileInfo excelFileGR = new FileInfo(@"C:\TKC\TCK_Post_to_Sap\temp\GR" + lastfilename + ".xlsx");
                        package.SaveAs(excelFileGR);
                    }

                    if (GetDataErrorTR.Rows.Count == 0)
                    {
                        for (int i = 0; i < GetDataErrorTR.Columns.Count; i++)
                        {
                            worksheet.Cells[1, i + 1].Value = GetDataErrorTR.Columns[i].ColumnName;
                        }

                        // Write data to Excel file
                        for (int row = 0; row < GetDataErrorTR.Rows.Count; row++)
                        {
                            for (int column = 0; column < GetDataErrorTR.Columns.Count; column++)
                            {
                                worksheet.Cells[row + 2, column + 1].Value = GetDataErrorTR.Rows[row][column];
                            }
                        }

                        FileInfo excelFileTR = new FileInfo(@"C:\TKC\TCK_Post_to_Sap\temp\TR" + lastfilename + ".xlsx");
                        package.SaveAs(excelFileTR);
                    }
                }
                //Console.WriteLine("strat Email sendmail!");
                //////// Email settings
                //string senderEmail = "prones_g@tkoito.co.th";
                //string receiverEmail = "tarkulbeer@gmail.com";
                ////string receiverEmail = "tniyom@tkoito.co.th";
                //string subject = "Excel File Attachment Error";
                //string body = "Please Check your data in Excel file attached. \n"+ MessagelistGR + "\n" + MessagelistTR;

                //Console.WriteLine("start Email send configuration!");
                //// Email configuration
                //MailMessage mail = new MailMessage(senderEmail, receiverEmail, subject, body);
                //SmtpClient client = new SmtpClient("172.18.1.2");
                //client.Port = 25; // Set the port according to your email provider
                //client.Credentials = new NetworkCredential("5000166", "cde3@wsxzaq1");
                //client.EnableSsl = false; // Enable SSL
                //Console.WriteLine("end Email send configuration!");
                //Console.WriteLine("start Email send Attachment!");
                ////Attach the Excel file
                //// Attachment attachment1 = new Attachment(@"C:\TKC\TCK_Post_to_Sap\temp\GR" + lastfilename + ".xlsx");
                //// Attachment attachment2 = new Attachment(@"C:\TKC\TCK_Post_to_Sap\temp\TR" + lastfilename + ".xlsx");
                //Attachment attachment1 = new Attachment(@"C:\testTCK\temp\GR1313112023.xlsx");
                //Attachment attachment2 = new Attachment(@"C:\testTCK\temp\TR1313112023.xlsx");
                //mail.Attachments.Add(attachment1);
                //mail.Attachments.Add(attachment2);
                //Console.WriteLine("end Email send successfully!");
                //// Send the email
                //try
                //{
                //    client.Send(mail);
                //    Console.WriteLine("Email sent successfully!");
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine("Error: " + ex.Message);
                //}
                //}
                // end cerate file and send mail
            }
        }

        private void End_update()
        {
            var sql = "UPDATE [Barcode_DEV].[dbo].[T_SAP_Batch_GR_TR_Log] SET End_Time = @End_Time where Start_Time = '" + start_Time + "'";

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

        public void TestsendSap(string partno, int qty, string custid, string store, string  postdate, string headertext)
        {

            List<ZsgmDetail1> result = new List<ZsgmDetail1>();
            List<ZsgmDetail1> Detail_GR = new List<ZsgmDetail1>();
            var ws_fn_head = new ZsgmHeader();
            var GmCode = new Bapi2017GmCode();

            ws_fn_head.BillOfLading = "";
            ws_fn_head.DocDate = DateTime.Now.ToString("yyyyMMdd");
            ws_fn_head.GrGiSlipNo = "";
            string UserID = "";
            if (headertext.Contains("|"))
            {
                UserID = headertext.Split('|')[0];
                headertext = headertext.Split('|')[1];
            }
            var RefdocNo = headertext;
            ws_fn_head.HeaderTxt = headertext;//"ADDSTOCKBYDEV";
                                              //ws_fn_head.PstngDate = "20220901";
            ws_fn_head.PstngDate = postdate.Replace("-", "");
            //ws_fn_head.PstngDate = DateTime.Now.ToString("yyyyMM01");
            //var Storage_GR = Master.T_LOCATION_SAP.ToList();
            GmCode.GmCode = "05";

            var Time = DateTime.Now.ToString("yyyy-MM-dd");
            var _Time = Time.Split('-');
            var year = _Time[0].Substring(_Time[0].Length - 1);
            //var Month = abcBase36(Convert.ToInt32(_Time[1]));
            //var Day = abcBase36(Convert.ToInt32(_Time[2]));

            //List<PRD_Z_GOODSMVT_CREATE1.ZsgmDetail1> Detail_GR = new List<PRD_Z_GOODSMVT_CREATE1.ZsgmDetail1>();

            //var StgLoc_GR = Storage_GR.Where(x => x.LOC_SAP_ID == Store).FirstOrDefault();

            ZsgmDetail1 temp = new ZsgmDetail1();
            temp.Batch = "DUMMYBATCH";
            temp.EntryQnt = Convert.ToDecimal(qty);
            temp.EntryUom = "Pcs";
            temp.FacNo = "F1";
            temp.Material = partno;
            temp.StgeLoc = store;
            temp.MoveType = "521";
            temp.Plant = "1100";
            temp.Custid = custid; //tmp.CUSTID;
            temp.Kanban = ""; //tmp.KANBANID;
            Detail_GR.Add(temp);

            //ZsgmDetail1 temp = new ZsgmDetail1();
            //temp.Batch = "";
            //temp.EntryQnt = Convert.ToDecimal(1);
            //temp.EntryUom = "";
            //temp.FacNo = "";
            //temp.Material = "";
            //temp.StgeLoc = "";
            //temp.MoveType = "";
            //temp.Plant = "";
            //temp.Custid = ""; //tmp.CUSTID;
            //temp.Kanban = ""; //tmp.KANBANID;
            //Detail_GR.Add(temp);


            result = Detail_GR.GroupBy(l => l.Kanban)
            .Select(cl => new ZsgmDetail1
            {
                Batch = cl.First().Batch,
                Material = cl.First().Material,
                EntryQnt = cl.Sum(c => c.EntryQnt),
                EntryUom = cl.First().EntryUom,
                FacNo = cl.First().FacNo,
                StgeLoc = cl.First().StgeLoc,
                MoveStloc = cl.First().MoveStloc,
                MoveType = cl.First().MoveType,
                Plant = cl.First().Plant,
                SoldTo = cl.First().SoldTo,
                Custid = cl.First().Custid,
                Kanban = cl.First().Kanban,
            }).ToList();

            var ws_service = new Z_GOODSMVT_CREATE1_SRV();
            var ws_fn_partosap = new ZGoodsmvtCreate1();
            var ws_res = new ZGoodsmvtCreate1Response();
            ws_fn_partosap.IsHeader = ws_fn_head;
            ws_fn_partosap.ItDetail = result.ToArray();
            ws_fn_partosap.IGoodsmvtCode = GmCode;;
            //ws_service.ZGoodsmvtCreate1(ws_fn_partosap);
            //Console.WriteLine("ws_res: " + ws_res);
        }

        private object abcBase36(int v)
        {
            throw new NotImplementedException();
        }
    }
}
