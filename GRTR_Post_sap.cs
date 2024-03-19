using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

using System.IO;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Xml.Linq;

namespace PostSap_GR_TR
{
    public partial class GRTR_Post_sap : Form
    {
        public GRTR_Post_sap()
        {
            InitializeComponent();
        }
        public async void GRTRPost_sap(object sender, EventArgs e)
        {
            GetAndUpdate_Batch_GR_TR_Log();
            Post_GR_to_Sap();
            Post_TR_to_Sap();
            //Post_GI_Sap();
            End_update();
            await GetErrorAndNotify();
            System.Environment.Exit(1);
        }
       
        string start_Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
        int checkruntime = 1;
        int checkcatchend = 0;


        // บันทึกรอบเวลาการส่งข้อมูล
        private void GetAndUpdate_Batch_GR_TR_Log()
        {
            try
            {
                Console.WriteLine("\nstart batch run time ");
                Console.WriteLine("#################################################### \n");

                ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
                string connString = "";
                if (setting != null)
                {
                    connString = setting.ConnectionString;
                }

                SqlConnection conn = new SqlConnection(connString);

                string sqlinsertRow = "INSERT INTO [Barcode].[dbo].[T_SAP_Batch_GR_TR_Log] (GR_NO, GR_Re_NO,TR_NO,TR_Re_NO,Start_Time,GI_NO,GI_Re_NO) VALUES (@GR_NO,@GR_Re_NO,@TR_NO,@TR_Re_NO,@Start_Time,@GI_NO,@GI_Re_NO)";
                using (SqlCommand cmd = new SqlCommand(sqlinsertRow, conn))
                {
                    cmd.Parameters.AddWithValue("@GR_NO", "");
                    cmd.Parameters.AddWithValue("@GR_Re_NO", "");
                    cmd.Parameters.AddWithValue("@TR_NO", "");
                    cmd.Parameters.AddWithValue("@TR_Re_NO", "");
                    cmd.Parameters.AddWithValue("@GI_NO", "");
                    cmd.Parameters.AddWithValue("@GI_Re_NO", "");
                    cmd.Parameters.AddWithValue("@Start_Time", start_Time);
                    conn.Open();
                    int result = cmd.ExecuteNonQuery();
                    conn.Close();
                }
                bool checkloop1 = true;
                bool checkloop2 = true;
                int countloop = 0;
                while ((checkloop1 == true || checkloop2 == true) && countloop <= 10)
                {
                    checkloop2 = CheckdataStart(checkloop1, checkloop2, countloop);
                    countloop++;
                    checkloop1 = false;
                }
            }
            catch (Exception ex)
            {
                string Message = "Unexpected error Batch_GR_TR_Log : " + ex.Message;
                CatchError(Message);
            }
        }

        private bool CheckdataStart(bool checkloop1 , bool checkloop2 ,int countloop)
        {
            checkruntime = countloop;
            if (checkloop1 == false && checkloop2 == true) System.Threading.Thread.Sleep(10000);

            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
            string connString = "";
            if (setting != null)
            {
                connString = setting.ConnectionString;
            }
            SqlConnection conn = new SqlConnection(connString);

            try
            {
            //    var sql = "select isnull((select count(*) GR_NO from [Barcode].[dbo].[v_sap_batch_gr] where Action = 1 group by Action),0)GR_NO," +
            //       "isnull((select count(*) GR_Re_NO from[Barcode].[dbo].[v_sap_batch_gr_redo] where Action = 1 group by Action),0)GR_Re_NO," +
            //       "isnull((select count(*) TR_NO From(select count(*) TR_NO, SLIPNO, Action from[Barcode].[dbo].[v_sap_batch_tr] where Action = 1 GROUP BY SLIPNO, Action) C1 GROUP BY C1.Action),0)TR_NO," +
            //       "isnull((select count(*) TR_Re_NO From(select count(*) TR_Re_NO, SLIPNO, Action from[Barcode].[dbo].[v_sap_batch_tr_redo] where Action = 1 GROUP BY SLIPNO, Action)D1 Group by D1.Action),0)TR_Re_NO," +
            //       "isnull((select count(*) GI_NO From(select count(*) GI_NO, ORDERNO, Action from[Barcode].[dbo].[v_sap_batch_gi] where Action = 1 GROUP BY ORDERNO, Action) E1 GROUP BY E1.Action),0)GI_NO," +
            //       "isnull((select count(*) GI_Re_NO From(select count(*) GI_Re_NO, ORDERNO, Action from[Barcode].[dbo].[v_sap_batch_gi_redo] where Action = 1 GROUP BY ORDERNO, Action)D1 Group by D1.Action),0)GI_Re_NO";
            //Class.Condb Condb = new Class.Condb();
            //DataTable dt = Condb.GetQuery(sql);

            SqlCommand command = new SqlCommand("[Barcode].[dbo].[SP_2SAP_item_chk]", conn);
            command.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            int checkdataOnprocess = Convert.ToInt32(dt.Rows[0]["GR_NO"]) + Convert.ToInt32(dt.Rows[0]["GR_Re_NO"]) + Convert.ToInt32(dt.Rows[0]["TR_NO"]) + Convert.ToInt32(dt.Rows[0]["TR_Re_NO"]) + Convert.ToInt32(dt.Rows[0]["GI_NO"]) + Convert.ToInt32(dt.Rows[0]["GI_Re_NO"]);
                if (checkdataOnprocess > 0)
                {
                
                    string sql = "UPDATE  [Barcode].[dbo].[T_SAP_Batch_GR_TR_Log] SET GR_NO = @GR_NO, GR_Re_NO = @GR_Re_NO,TR_NO = @TR_NO,TR_Re_NO = @TR_Re_NO,GI_NO = @GI_NO,GI_Re_NO = @GI_Re_NO where start_Time = '" + start_Time + "'";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@GR_NO", dt.Rows[0]["GR_NO"].ToString());
                        cmd.Parameters.AddWithValue("@GR_Re_NO", dt.Rows[0]["GR_Re_NO"].ToString());
                        cmd.Parameters.AddWithValue("@TR_NO", dt.Rows[0]["TR_NO"].ToString());
                        cmd.Parameters.AddWithValue("@TR_Re_NO", dt.Rows[0]["TR_Re_NO"].ToString());
                        cmd.Parameters.AddWithValue("@GI_NO", dt.Rows[0]["GI_NO"].ToString());
                        cmd.Parameters.AddWithValue("@GI_Re_NO", dt.Rows[0]["GI_Re_NO"].ToString());
                        conn.Open();
                        int result = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    if (checkruntime > 1) {
                        string Message = "Found data in round : " + checkruntime;
                        string dataUpdateList = "UPDATE [Barcode].[dbo].[T_SAP_Batch_GR_TR_Log] SET EMessageError = @EMessageError  where start_Time = '" + start_Time + "'";

                        string ms = checkruntime > 0 ? "No data available Round " + checkruntime : "No data available";
                        using (SqlCommand cmd = new SqlCommand(dataUpdateList, conn))
                        {
                            cmd.Parameters.AddWithValue("@EMessageError", Message);
                            conn.Open();
                            int result = cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
                else
                {
                    string dataUpdateList = "UPDATE [Barcode].[dbo].[T_SAP_Batch_GR_TR_Log] SET EMessageError = @EMessageError  where start_Time = '" + start_Time + "'";

                    string ms = checkruntime > 1 ? "No data available Round " + checkruntime : "No data available";
                    using (SqlCommand cmd = new SqlCommand(dataUpdateList, conn))
                    {
                        cmd.Parameters.AddWithValue("@EMessageError", ms);
                        conn.Open();
                        int result = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    End_update();
                    System.Environment.Exit(1);
                }
                return false;
            }
            catch (Exception ex)
            {
                string Message ;
              
                    Message = "select Data time out && recheck data : Round "+ checkruntime;
                    string dataUpdateList = "UPDATE [Barcode].[dbo].[T_SAP_Batch_GR_TR_Log] SET EMessageError = @EMessageError  where start_Time = '" + start_Time + "'";

                    using (SqlCommand cmd = new SqlCommand(dataUpdateList, conn))
                    {
                        cmd.Parameters.AddWithValue("@EMessageError", Message);
                        conn.Open();
                        int result = cmd.ExecuteNonQuery();
                        conn.Close();
                    }

                if (checkruntime == 10)
                {
                    Message = "this function loop " + checkruntime + " times and not found data :" + ex.Message;
                    CatchError(Message);
                }
                return true;
            }
        }

        private void Post_GR_to_Sap()
            {
            try
            {
                Console.WriteLine("      Process GR");
                Console.WriteLine("      #################################################### \n");
                Console.WriteLine("      start Process GR");
                _ = new DataTable();
                _ = new Class.ServicePostSapGR();
                Class.Condb Condb = new Class.Condb();
                string sqlGetGR = "select * from [Barcode].[dbo].[v_sap_batch_gr] where Action = 1";
                string sqlGetGR_redo = "select * from [Barcode].[dbo].[v_sap_batch_gr_redo] where Action = 1";
                DataTable GRdata = Condb.GetQuery(sqlGetGR);
                DataTable GRErrdata = Condb.GetQuery(sqlGetGR_redo);
                Class.ServicePostSapGR sendSapGR = new Class.ServicePostSapGR();
                if (GRdata.Rows.Count > 0)
                {
                    foreach (DataRow item in GRdata.Rows)
                    {
                        string partno = item["MatNo"].ToString().Trim();
                        int qty = Convert.ToInt32(item["QRQty"].ToString());
                        string custid = item["CustID"].ToString().Trim();
                        string FacNo = item["FacNo"].ToString().Trim();
                        string Plant = item["Plant"].ToString().Trim();
                        string store = item["SLoc"].ToString().Trim();
                        int MvmntType = Convert.ToInt32(item["MvmntType"].ToString());
                        string postdate = item["PostDate"].ToString().Trim();
                        string PostTime = item["PostTime"].ToString().Trim();
                        string headertext = "IT|" + item["HeaderText"].ToString().Trim();
                        int Action = Convert.ToInt32(item["Action"].ToString());
                        string Type = "GR".ToString().Trim();
                        Class.Validate_GRTR Validate_GRTR = new Class.Validate_GRTR();
                        var getID = Validate_GRTR.GetAndUpdate_LogDataValidate_GR_to_Sap(partno, qty, custid, FacNo, Plant, store, MvmntType, postdate, PostTime, headertext, Action, Type);
                        sendSapGR.PostSapGRClass(partno, qty, custid, store, postdate, headertext, getID);
                    }
                }
                if (GRErrdata.Rows.Count > 0)
                {
                    foreach (DataRow item in GRErrdata.Rows)
                    {
                        string partno = item["MatNo"].ToString().Trim();
                        int qty = Convert.ToInt32(item["QRQty"].ToString());
                        string custid = item["CustID"].ToString().Trim();
                        string FacNo = item["FacNo"].ToString().Trim();
                        string Plant = item["Plant"].ToString().Trim();
                        string store = item["SLoc"].ToString().Trim();
                        int MvmntType = Convert.ToInt32(item["MvmntType"].ToString());
                        string postdate = item["PostDate"].ToString().Trim();
                        string PostTime = item["PostTime"].ToString().Trim();
                        string headertext = "IT|" + item["HeaderText"].ToString().Trim();
                        int Action = Convert.ToInt32(item["Action"].ToString());
                        string Type = "GR_redo".ToString().Trim();
                        Class.Validate_GRTR Validate_GRTR = new Class.Validate_GRTR();
                        var getID = Validate_GRTR.GetAndUpdate_LogDataValidate_GR_to_Sap(partno, qty, custid, FacNo, Plant, store, MvmntType, postdate, PostTime, headertext, Action, Type);
                        sendSapGR.PostSapGRClass(partno, qty, custid, store, postdate, headertext, getID);
                    }
                }
                Console.WriteLine("      End Process GR \n");
            }
            catch (Exception ex)
            {
                string Message = "Unexpected error Post_GR_to_Sap : " + ex.Message; ;
                CatchError(Message);
            }
        }

        private void Post_TR_to_Sap()
        {
            try
            {
                Console.WriteLine("      Process TR");
                Console.WriteLine("      #################################################### \n");
                Console.WriteLine("      Start Process TR");
                _ = new DataTable();
                _ = new Class.ServicePostSapTR();
                Class.Condb Condb = new Class.Condb();
                string sqlGetTR = "select count(*) ,SLIPNO from [Barcode].[dbo].[v_sap_batch_tr] where Action = 1 GROUP BY SLIPNO";
                DataTable TRdata = Condb.GetQuery(sqlGetTR);
                string sqlGetTR_redo = "select count(*) ,SLIPNO from [Barcode].[dbo].[v_sap_batch_tr_redo] where Action = 1 GROUP BY SLIPNO";
                DataTable TRErrdata = Condb.GetQuery(sqlGetTR_redo);
                Class.ServicePostSapTR sendSapTR = new Class.ServicePostSapTR();
                if (TRdata.Rows.Count > 0)
                {
                    foreach (DataRow item in TRdata.Rows)
                    {
                        string Slipno = "IT|" + item["SLIPNO"].ToString().Trim();
                        string Datatype = "12";
                        string Type = "TR";
                        string checkSlipno = item["SLIPNO"].ToString().Trim();
                        Class.Validate_GRTR Validate_GRTR = new Class.Validate_GRTR();
                        var getID = Validate_GRTR.GetAndUpdate_LogDataValidate_TR_to_Sap(checkSlipno, Datatype, Type);
                        sendSapTR.PostSapTRClass(Slipno, Datatype, getID);
                    }
                }

                if (TRErrdata.Rows.Count > 0)
                {
                    foreach (DataRow item in TRErrdata.Rows)
                    {
                        string Slipno = "IT|" + item["SLIPNO"].ToString().Trim();
                        string Datatype = "13";
                        string Type = "TR_redo";
                        string checkSlipno = item["SLIPNO"].ToString().Trim();
                        Class.Validate_GRTR Validate_GRTR = new Class.Validate_GRTR();
                        var getID = Validate_GRTR.GetAndUpdate_LogDataValidate_TR_to_Sap(checkSlipno, Datatype, Type);
                        sendSapTR.PostSapTRClass(Slipno, Datatype, getID);
                    }
                }
                Console.WriteLine("      End Process TR \n");
            }
            catch (Exception ex)
            {
                string Message = "Unexpected error Post_TR_to_Sap : " + ex.Message; ;
                CatchError(Message);
            }
        }
        private void Post_GI_Sap()
        {
            try
            {
                Console.WriteLine("      Process GI");
                Console.WriteLine("      #################################################### \n");
                Console.WriteLine("      Start Process GI");
                _ = new DataTable();
                _ = new Class.ServicePostSapGI();
                Class.Condb Condb = new Class.Condb();

                string sqlGetGI = "SELECT count(*) as countOrder, ORDERNO FROM [Barcode].[dbo].[v_sap_batch_gi] where Action = 1 group by ORDERNO";
                //string sqlGetGI = "SELECT * FROM [Barcode].[dbo].[testGI] where 1=1";
                DataTable GIdata = Condb.GetQuery(sqlGetGI);
                string sqlGetGI_redo = "SELECT count(*) as countOrder, RefDocNo , ORDERNO FROM [Barcode].[dbo].[v_sap_batch_gi_redo] where Action = 1 group by RefDocNo ,ORDERNO";
                DataTable GIErrdata = Condb.GetQuery(sqlGetGI_redo);
                Class.ServicePostSapGI sendSapGI = new Class.ServicePostSapGI();
                if (GIdata.Rows.Count > 0)
                {
                    foreach (DataRow item in GIdata.Rows)
                    {
                        string OrderNo = item["ORDERNO"].ToString().Trim();
                        string PoAndDo = item["ORDERNO"].ToString().Trim();
                        string Type = "GI";
                        string checkPoAndDO = OrderNo.Substring(0, 2);
                        checkPoAndDO = checkPoAndDO == "31" ? "DO" : "PO";
                        string DOandPO = checkPoAndDO;
                        Class.Validate_GRTR Validate_GRTR = new Class.Validate_GRTR();
                        var getID  = Validate_GRTR.GetAndUpdate_saveLogData_GI_to_Sap(OrderNo, checkPoAndDO, Type);
                        sendSapGI.PostSapGIClass(PoAndDo, DOandPO , getID);
                    }
                }

                if (GIErrdata.Rows.Count > 0)
                {
                    foreach (DataRow item in GIErrdata.Rows)
                    {
                        string OrderNo = item["ORDERNO"].ToString().Trim();
                        string PoAndDo = item["ORDERNO"].ToString().Trim();
                        string Type = "GI_redo";
                        string checkPoAndDO = OrderNo.Substring(0, 2);
                        checkPoAndDO = checkPoAndDO == "31" ? "DO" : "PO";
                        string DOandPO = checkPoAndDO;
                        Class.Validate_GRTR Validate_GRTR = new Class.Validate_GRTR();
                        var getID =  Validate_GRTR.GetAndUpdate_saveLogData_GI_to_Sap(OrderNo, checkPoAndDO, Type);
                        sendSapGI.PostSapGIClass(PoAndDo, DOandPO, getID);
                    }
                }
                Console.WriteLine("      End Process GI \n");
                Console.WriteLine("      #################################################### \n");
            }
            catch (Exception ex)
            {
                string Message = "Unexpected error Post_GI_Sap : " + ex.Message; ;
                CatchError(Message);
            }
        }

        private void End_update()
        {
            try
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
                Console.WriteLine("#################################################### ");
                Console.WriteLine("End batch run time");
                Console.WriteLine("successfully\n");
                Console.WriteLine("#################################################### \n");
            }
            catch (Exception ex)
            {
                string Message = "Unexpected error End_update : " + ex.Message; ;
                CatchError(Message);
            }
        }

        private async Task GetErrorAndNotify()
        {
            try
            {
               
                Console.WriteLine("Process Notify");
                Console.WriteLine("#################################################### \n");
                _ = new DataTable();
                string checkTime = DateTime.Now.ToString("HH:mm");
                Class.Condb Condb = new Class.Condb();
                ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
                if (setting != null)
                {
                    _ = setting.ConnectionString;
                }
                string sqlemailGR = "select  RefDocNo as DocNo , EMessage from [Barcode].[dbo].[v_get_dataNotify_gr] where 1 = 1";
                string sqlemailTR = "select  RefDocNo as DocNo , EMessage from [Barcode].[dbo].[v_get_dataNotify_tr] where 1 = 1";
                string sqlemailGI = "select  RefDocNo as DocNo , EMessage from [Barcode].[dbo].[v_get_dataNotify_gi] where 1 = 1";

                string sqllineGR = "select count(*) totalSum from [Barcode].[dbo].[v_get_dataNotify_gr] where Action = 1";
                string sqllineTR = "select count(*) totalSum From (select count(*) TR_Re_NO, SLIPNO, Action from[Barcode].[dbo].[v_get_dataNotify_tr] where Action = 1 GROUP BY SLIPNO, Action)D1 ";
                string sqllineGI = "select count(*) totalSum From (select count(*) TR_Re_NO, ORDERNO, Action from[Barcode].[dbo].[v_get_dataNotify_gi] where Action = 1 GROUP BY ORDERNO, Action)D1 ";

                DataTable GetDataErrorGR = Condb.GetQuery(sqlemailGR);
                DataTable GetDataErrorTR = Condb.GetQuery(sqlemailTR);
                DataTable GetDataErrorGI = Condb.GetQuery(sqlemailGI);
                DataTable GetDataErrorGRrow = Condb.GetQuery(sqllineGR);
                DataTable GetDataErrorTRrow = Condb.GetQuery(sqllineTR);
                DataTable GetDataErrorGIrow = Condb.GetQuery(sqllineGI);
                string checkdata1 = GetDataErrorGRrow.Rows.Count > 0 ? GetDataErrorGRrow.Rows[0]["totalSum"].ToString() : "";
                string checkdata2 = GetDataErrorTRrow.Rows.Count > 0 ? GetDataErrorTRrow.Rows[0]["totalSum"].ToString() : "";
                string checkdata3 = GetDataErrorGIrow.Rows.Count > 0 ? GetDataErrorGIrow.Rows[0]["totalSum"].ToString() : "";
                string MessagelistGR = int.Parse(checkdata1) > 0 ? "GR Error : " + checkdata1 + " Item" : "";
                string MessagelistTR = int.Parse(checkdata2) > 0 ? "TR Error : " + checkdata2 + " Item" : "";
                string MessagelistGI = int.Parse(checkdata3) > 0 ? "GI Error : " + checkdata3 + " Item" : "";
                Console.WriteLine("Start sent LineNotify ");
                string ValidateMessage = "Error  \nrun time =  " + checkTime + "\n" + MessagelistGR + "\n" + MessagelistTR + "\n" + MessagelistGI;

                Console.WriteLine("Start sent LineNotify ");
                // start line notify 
                
                if (int.Parse(checkdata1) > 0 || int.Parse(checkdata2) > 0 || int.Parse(checkdata3) > 0)
                {
                    Class.LineNotify lineNotify = new Class.LineNotify();
                    lineNotify.FNLineNotify(ValidateMessage);
                }
                await Task.Delay(3000);
                // end line notify
                string checkruntime =  getTimeNotify();
                if (checkruntime == "Y")
                {
                    
                    // start cerate file and send mail
                    if (GetDataErrorGR.Rows.Count > 0 || GetDataErrorTR.Rows.Count > 0 || GetDataErrorGI.Rows.Count > 0)
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        DateTime now = DateTime.Now;
                        string filename = now.ToString("yyyy-MM-dd HH:mm:ss:fff");

                        string[] words = filename.Split(' ');
                        string[] text1 = words[0].Split('-');
                        string[] text2 = words[1].Split(':');
                        string lastfilename = text1[0] + "_" + text1[1] + "_" + text1[2] + "_" + text2[0];
                        string Fordername = text1[0] + "_" + text1[1] + "_" + text1[2] + "_" + text2[0];
                        string folderPath = @"C:\testTKC\temp\" + Fordername;
                        using (var package = new ExcelPackage())
                        {

                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Data");

                            // Write column headers
                            if (GetDataErrorGR.Rows.Count > 0)
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

                                //string folderPath = @"C:\TKC\TCK_Post_to_Sap\temp\" + Fordername;
                                Directory.CreateDirectory(folderPath);
                                FileInfo excelFileGR = new FileInfo(folderPath + "\\GR" + lastfilename + ".xlsx");
                                package.SaveAs(excelFileGR);
                            }

                            if (GetDataErrorTR.Rows.Count > 0)
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


                                //string folderPath = @"C:\TKC\TCK_Post_to_Sap\temp\" + Fordername;
                                Directory.CreateDirectory(folderPath);
                                FileInfo excelFileGR = new FileInfo(folderPath + "\\TR" + lastfilename + ".xlsx");
                                package.SaveAs(excelFileGR);
                            }

                            if (GetDataErrorGI.Rows.Count > 0)
                            {
                                for (int i = 0; i < GetDataErrorGI.Columns.Count; i++)
                                {
                                    worksheet.Cells[1, i + 1].Value = GetDataErrorGI.Columns[i].ColumnName;
                                }

                                // Write data to Excel file
                                for (int row = 0; row < GetDataErrorGI.Rows.Count; row++)
                                {
                                    for (int column = 0; column < GetDataErrorGI.Columns.Count; column++)
                                    {
                                        worksheet.Cells[row + 2, column + 1].Value = GetDataErrorGI.Rows[row][column];
                                    }
                                }


                                //string folderPath = @"C:\TKC\TCK_Post_to_Sap\temp\" + Fordername;
                                Directory.CreateDirectory(folderPath);
                                FileInfo excelFileGR = new FileInfo(folderPath + "\\GI" + lastfilename + ".xlsx");
                                package.SaveAs(excelFileGR);
                            }
                        }
                        Console.WriteLine("Start sent Email \n");
                        ////// Email settings
                        ///
                        //string senderEmail = ConfigurationManager.AppSettings["SenderEmail"];
                        //string receiverEmail = ConfigurationManager.AppSettings["mailTO"];

                        //string subject = "Excel File Attachment Error";
                        //string body = "Please Check your data in Excel file attached. \n" + ValidateMessage;

                        // Email configuration

                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(ConfigurationManager.AppSettings["SenderEmail"]);
                        mail.To.Add(ConfigurationManager.AppSettings["mailTO"]);
                        mail.Subject = "Excel File Attachment Error";
                        mail.Body = "Please Check your data in Excel file attached. \n" + ValidateMessage;

                        SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["SmtpClient"]);
                        client.Port = 25; // Set the port according to your email provider
                        client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["CredentialsUser"], ConfigurationManager.AppSettings["CredentialsPass"]);
                        client.EnableSsl = false;


                        // Enable SSL
                        //Attach the Excel file
                        // Attachment attachment1 = new Attachment(@"C:\TKC\TCK_Post_to_Sap\temp\GR" + lastfilename + ".xlsx");
                        // Attachment attachment2 = new Attachment(@"C:\TKC\TCK_Post_to_Sap\temp\TR" + lastfilename + ".xlsx");
                        if (GetDataErrorGR.Rows.Count > 0)
                        {
                            Attachment attachment1 = new Attachment(folderPath + "\\GR" + lastfilename + ".xlsx");
                            mail.Attachments.Add(attachment1);
                        }
                        if (GetDataErrorTR.Rows.Count > 0)
                        {
                            Attachment attachment2 = new Attachment(folderPath + "\\TR" + lastfilename + ".xlsx");
                            mail.Attachments.Add(attachment2);
                        }
                        if (GetDataErrorGI.Rows.Count > 0)
                        {
                            Attachment attachment3 = new Attachment(folderPath + "\\GI" + lastfilename + ".xlsx");
                            mail.Attachments.Add(attachment3);
                        }
                        // Send the email
                        try
                        {
                            client.Send(mail);
                            Console.WriteLine("Email sent successfully!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                    }
                    // end cerate file and send mail
                }
            }
            catch (Exception ex)
            {
                string Message = "Unexpected error GetErrorAndNotify : " + ex.Message; ;
                CatchError(Message);
            }

        }
        public string getTimeNotify()
        {
            string checkTime = DateTime.Now.ToString("HH:mm");
            string[] timenow = checkTime.Split(':');
            string hour = timenow[0];
            int minute = Convert.ToInt32(timenow[1]);
            string flag = "Y";
            if ((hour == "08"  && minute < 30) || (hour == "13" && minute < 30)) {
                flag = "Y";
            }
            else {
                flag = "N";
            }
            return flag;

        }

        public void CatchError(string massage)
        {
            _ = new DataTable();
            _ = new Class.ServicePostSapGR();
            Class.Condb Condb = new Class.Condb();
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
            string connString = "";
            if (setting != null)
            {
                connString = setting.ConnectionString;
            }

            SqlConnection conn = new SqlConnection(connString);
            string dataUpdateList = "UPDATE [Barcode].[dbo].[T_SAP_Batch_GR_TR_Log] SET EMessageError = @EMessageError  where start_Time = '" + start_Time + "'";
            using (SqlCommand cmd = new SqlCommand(dataUpdateList, conn))
            {
                cmd.Parameters.AddWithValue("@EMessageError", massage);
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                conn.Close();
            }
           
            End_update();
            System.Environment.Exit(1);

        }

    }
}
