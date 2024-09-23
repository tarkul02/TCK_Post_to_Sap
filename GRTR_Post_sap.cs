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
using PostSap_GR_TR.Class;
using System.Linq;
using System.Data.Entity;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using SapApiGRAndTR.Class;
using PostSap_GR_TR.Models;

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

            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            GetAndUpdate_Batch_GR_TR_Log();
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            //Post_GR_to_Sap();
            //Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            Post_TR_to_Sap();
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            Post_GI_Sap();
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            //await GetErrorAndNotify();
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            await Task.Delay(3000);
            End_update();
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
        }


        string start_Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
        int checkruntime = 1;
        string DBconfig = ConfigurationManager.AppSettings["Databaseconfig"];
        bool setlog = false;
        bool setsuccesslog = false;
        bool seterrorlog = false;


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

                string sqlinsertRow = "INSERT INTO " + DBconfig + ".[T_SAP_Batch_GR_TR_Log] (GR_NO, GR_Re_NO,TR_NO,TR_Re_NO,Start_Time,GI_NO) VALUES (@GR_NO,@GR_Re_NO,@TR_NO,@TR_Re_NO,@Start_Time,@GI_NO)";


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

        private bool CheckdataStart(bool checkloop1, bool checkloop2, int countloop)
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
                //Class.Condb Condb = new Class.Condb();
                //DataTable dt = Condb.GetQuery(sql);

                // SqlCommand command = new SqlCommand(DBconfig +".[SP_2SAP_item_chk]", conn);

                //เช็คข้อมูล GR QTY
                SqlCommand command = new SqlCommand(DBconfig + ".[SP_2SAP_item_chk_check_QtyGR]", conn);

                command.CommandTimeout = 240;
                command.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                int checkdataOnprocess = Convert.ToInt32(dt.Rows[0]["GR_NO"]) + Convert.ToInt32(dt.Rows[0]["GR_Re_NO"]) + Convert.ToInt32(dt.Rows[0]["TR_NO"]) + Convert.ToInt32(dt.Rows[0]["TR_Re_NO"]) + Convert.ToInt32(dt.Rows[0]["GI_NO"]) + Convert.ToInt32(dt.Rows[0]["GI_Re_NO"]);

                if (checkdataOnprocess > 0)
                {
                    //เช็คข้อมูล GR QTY
                    string sql = "UPDATE  " + DBconfig + ".[T_SAP_Batch_GR_TR_Log] SET GR_NO = @GR_NO, GR_Re_NO = @GR_Re_NO ,GR_QTY = @GR_QTY, GR_RE_QTY = @GR_RE_QTY,TR_NO = @TR_NO,TR_Re_NO = @TR_Re_NO,GI_NO = @GI_NO,GI_Re_NO = @GI_Re_NO where start_Time = '" + start_Time + "'";
                    //string sql = "UPDATE  " + DBconfig + ".[T_SAP_Batch_GR_TR_Log] SET GR_NO = @GR_NO, GR_Re_NO = @GR_Re_NO,TR_NO = @TR_NO,TR_Re_NO = @TR_Re_NO,GI_NO = @GI_NO,GI_Re_NO = @GI_Re_NO where start_Time = '" + start_Time + "'";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@GR_NO", dt.Rows[0]["GR_NO"].ToString());
                        cmd.Parameters.AddWithValue("@GR_Re_NO", dt.Rows[0]["GR_Re_NO"].ToString());
                        cmd.Parameters.AddWithValue("@TR_NO", dt.Rows[0]["TR_NO"].ToString());
                        cmd.Parameters.AddWithValue("@TR_Re_NO", dt.Rows[0]["TR_Re_NO"].ToString());
                        cmd.Parameters.AddWithValue("@GI_NO", dt.Rows[0]["GI_NO"].ToString());
                        cmd.Parameters.AddWithValue("@GI_Re_NO", dt.Rows[0]["GI_Re_NO"].ToString());
                        //เช็คข้อมูล GR QTY
                        cmd.Parameters.AddWithValue("@GR_QTY", dt.Rows[0]["GR_QTY"].ToString());
                        cmd.Parameters.AddWithValue("@GR_RE_QTY", dt.Rows[0]["GR_RE_QTY"].ToString());
                        conn.Open();
                        int result = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    if (checkruntime > 1)
                    {
                        string Message = "Found data in round : " + checkruntime;
                        string dataUpdateList = "UPDATE " + DBconfig + ".[T_SAP_Batch_GR_TR_Log] SET EMessageError = @EMessageError  where start_Time = '" + start_Time + "'";

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
                    string dataUpdateList = "UPDATE " + DBconfig + ".[T_SAP_Batch_GR_TR_Log] SET EMessageError = @EMessageError  where start_Time = '" + start_Time + "'";
                    string ms = checkruntime > 1 ? "No data available Round " + checkruntime : "No data available";
                    using (SqlCommand cmd = new SqlCommand(dataUpdateList, conn))
                    {
                        cmd.Parameters.AddWithValue("@EMessageError", ms);
                        conn.Open();
                        int result = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    End_update();
                }
                return false;
            }
            catch (Exception ex)
            {
                string Message;

                Message = "select Data time out && recheck data : Round " + checkruntime;
                string dataUpdateList = "UPDATE " + DBconfig + ".[T_SAP_Batch_GR_TR_Log] SET EMessageError = @EMessageError  where start_Time = '" + start_Time + "'";

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
                string sqlGetGR = "select * from " + DBconfig + ".[v_sap_batch_gr] where Action = 1";
                string sqlGetGR_redo = "select * from " + DBconfig + ".[v_sap_batch_gr_redo] where Action = 1";

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
                string Message = "Unexpected error Post_GR_to_Sap checkrow : " + ex.Message;
                CatchError(Message);
            }
        }
        string checktable;
        string checkSlipNo = "";
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
                string sqlGetTR = "select * from " + DBconfig + ".[v_sap_batch_tr] where Action = 1 and MAT_TYPE <> 'ZRM' ORDER BY SLIPNO";
                string sqlGetTR_redo = "select  * from " + DBconfig + ".[v_sap_batch_tr_redo] where Action = 1 and MAT_TYPE <> 'ZRM' ORDER BY SLIPNO";
                DataTable TRdata = Condb.GetQuery(sqlGetTR);
                DataTable TRErrdata = Condb.GetQuery(sqlGetTR_redo);
                Class.ServicePostSapTR sendSapTR = new Class.ServicePostSapTR();

                var ws_fn_head = new ZsgmHeader();

                ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
                string connString = "";
                if (setting != null)
                {
                    connString = setting.ConnectionString;
                }


                // Create a DataTable
                DataTable dataTable = new DataTable();

                // Add columns to the DataTable without specifying types
                dataTable.Columns.Add("ID");
                dataTable.Columns.Add("SlipNo");
                dataTable.Columns.Add("Type");
                dataTable.Columns.Add("CreateDate");
                dataTable.Columns.Add("Datatype");
                dataTable.Columns.Add("ValidateMessage");
                dataTable.Columns.Add("PlantFrom");
                dataTable.Columns.Add("StorageFrom");
                dataTable.Columns.Add("PlantTo");
                dataTable.Columns.Add("StorageTo");
                dataTable.Columns.Add("PostDate");
                dataTable.Columns.Add("POSTTIME");
                dataTable.Columns.Add("Kanban");
                dataTable.Columns.Add("MvmntQty");
                dataTable.Columns.Add("Mat_Type");
                dataTable.Columns.Add("SapStatus");
                dataTable.Columns.Add("ConfirmDate");


                // Create a DataTable with the new name
                DataTable dataTable2 = new DataTable();

                // Add columns to the DataTable without specifying types
                dataTable2.Columns.Add("Id");
                dataTable2.Columns.Add("DocMat");
                dataTable2.Columns.Add("Batch");
                dataTable2.Columns.Add("EntryQnt");
                dataTable2.Columns.Add("EntryUom");
                dataTable2.Columns.Add("FacNo");
                dataTable2.Columns.Add("Material");
                dataTable2.Columns.Add("StgeLoc");
                dataTable2.Columns.Add("MoveType");
                dataTable2.Columns.Add("Plant");
                dataTable2.Columns.Add("Custid");
                dataTable2.Columns.Add("Kanban");
                dataTable2.Columns.Add("EMessage");
                dataTable2.Columns.Add("StockDate");
                dataTable2.Columns.Add("UpdDate");

                // Create a DataTable with the new name
                DataTable dataTable3 = new DataTable();

                // Add columns to the DataTable without specifying types
                dataTable3.Columns.Add("Id");
                dataTable3.Columns.Add("RefDocNo");
                dataTable3.Columns.Add("Batch");
                dataTable3.Columns.Add("EntryQnt");
                dataTable3.Columns.Add("EntryUom");
                dataTable3.Columns.Add("FacNo");
                dataTable3.Columns.Add("Material");
                dataTable3.Columns.Add("StgeLoc");
                dataTable3.Columns.Add("MoveType");
                dataTable3.Columns.Add("Plant");
                dataTable3.Columns.Add("Custid");
                dataTable3.Columns.Add("Kanban");
                dataTable3.Columns.Add("EMessage");
                dataTable3.Columns.Add("StockDate");
                dataTable3.Columns.Add("UpdDate");



                var ws_res = new ZGoodsmvtCreate1Response();


                int paramIndex = 0;

                if (TRdata.Rows.Count > 0)
                {
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    List<SqlParameter> parameters2 = new List<SqlParameter>();
                    List<SqlParameter> parameters3 = new List<SqlParameter>();

                    foreach (DataRow data in TRdata.Rows)
                    {
                        try
                        {
                            setlog = false;
                            setsuccesslog = false;
                            seterrorlog = false;
                            checkSlipNo = "IT|" + data["SLIPNO"].ToString().Trim();

                            string Slipno = "IT|" + data["SLIPNO"].ToString().Trim();
                            string Datatype = "12";
                            string Type = "TR";
                            string checkSlipno = data["SLIPNO"].ToString().Trim();

                            string Plant = data["PlantFrom"].ToString();
                            string StgeLoc = data["StorageFrom"].ToString();
                            string EntryQnt = Convert.ToInt32(data["MvmntQty"].ToString()).ToString();
                            string MovePlant = data["PlantTo"].ToString();
                            string MoveStloc = data["StorageTo"].ToString();
                            string Kanban = data["Kanban"].ToString();
                            string PostDate = data["POSTDATE"].ToString();
                            string Mat_Type = data["Mat_Type"].ToString();
                            var RefdocNo = "TR-" + DateTime.Now.ToString("yyMMddHHmm");
                            string UserID = "";
                            ws_fn_head.RefDocNo = RefdocNo;
                            if (Slipno.Contains("|"))
                            {
                                UserID = Slipno.Split('|')[0];
                                Slipno = Slipno.Split('|')[1];
                            }

                            string Message = "";
                            Message += Datatype.Length == 2 ? "" : "Datatype ,".ToString().Trim();
                            string ValidateMessage = Message != "" ? "Error : ( " + Message + ")" : "";

                            //ws_res = sendSapTR.PostSapTRClass(Slipno, Datatype, Type, Plant, StgeLoc, EntryQnt, MovePlant, MoveStloc, Kanban, PostDate, start_Time, Mat_Type);
                            
                            dataTable.Rows.Add(
                                                checkSlipno,
                                                Type,
                                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                                                Datatype,
                                                ValidateMessage,
                                                Plant,
                                                StgeLoc,
                                                MovePlant,
                                                MoveStloc,
                                                "",
                                                "",
                                                Kanban,
                                                EntryQnt,
                                                Mat_Type,
                                                "1",
                                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")
                                            );


                            //if (ws_res.ItDetail.Count() > 0)
                            //{

                            //    foreach (var item in ws_res.ItDetail)
                            //    {

                            //        if (string.IsNullOrEmpty(item.Error) && !string.IsNullOrEmpty(ws_res.EMaterailDoc.MatDoc))
                            //        {

                            //            dataTable.Rows.Add(
                            //                                checkSlipno,
                            //                                Type,
                            //                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                            //                                Datatype,
                            //                                ValidateMessage,
                            //                                Plant,
                            //                                StgeLoc,
                            //                                MovePlant,
                            //                                MoveStloc,
                            //                                "",
                            //                                "",
                            //                                Kanban,
                            //                                EntryQnt,
                            //                                Mat_Type,
                            //                                "1",
                            //                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")
                            //                            );

                            //            setlog = true;

                            //            dataTable2.Rows.Add(
                            //                                ws_res.EMaterailDoc.MatDoc + "|" + UserID,
                            //                                item.Batch,
                            //                                (int)item.EntryQnt,
                            //                                item.EntryUom,
                            //                                item.FacNo,
                            //                                checkSlipno,
                            //                                item.StgeLoc + "|" + item.MoveStloc,
                            //                                item.MoveType,
                            //                                item.Plant + "|" + item.MovePlant,
                            //                                item.Custid,
                            //                                item.Kanban,
                            //                                "TransferStockDataToSAP_311 : " + ws_res.EMaterailDoc.MatDoc + "|" + ws_res.EMaterailDoc.DocYear + "|" + ws_res.EMessage + "|" + item.Error,
                            //                                DateTime.Now.ToString("yyyy-MM-dd"),
                            //                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));

                            //            setsuccesslog = true;

                            //        }
                            //        else
                            //        {

                            //            if (item.Error != "")
                            //            {

                            //                dataTable.Rows.Add(
                            //                                   checkSlipno,
                            //                                   Type,
                            //                                   DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                            //                                   Datatype,
                            //                                   ValidateMessage,
                            //                                   Plant,
                            //                                   StgeLoc,
                            //                                   MovePlant,
                            //                                   MoveStloc,
                            //                                   "",
                            //                                   "",
                            //                                   Kanban,
                            //                                   EntryQnt,
                            //                                   Mat_Type,
                            //                                   "1",
                            //                                   DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")
                            //                                    );

                            //                setlog = true;

                            //                dataTable3.Rows.Add(
                            //                                    RefdocNo + "|" + UserID,
                            //                                    item.Batch,
                            //                                    (int)item.EntryQnt,
                            //                                    item.EntryUom,
                            //                                    item.FacNo,
                            //                                    Slipno,
                            //                                    item.StgeLoc + "|" + item.MoveStloc,
                            //                                    item.MoveType,
                            //                                    item.Plant + "|" + item.MovePlant,
                            //                                    item.Custid,
                            //                                    item.Kanban,
                            //                                    "TransferStockDataToSAP_311 : " + item.Error,
                            //                                    DateTime.Now.ToString("yyyy-MM-dd"),
                            //                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")
                            //                                     );

                            //                seterrorlog = true;
                            //            }
                            //        }
                            //    }
                            //}

                            paramIndex++;
                        }
                        catch (Exception ex)
                        {
                            string Message = checktable + "Unexpected error Post_TR_to_Sap : " + ex.Message;
                            if (setlog == false)
                            {
                                dataTable.Rows.Add(
                                                    checkSlipNo,
                                                    "",
                                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")
                                                );
                            }

                            if (setsuccesslog == false && setlog == true)
                            {
                                dataTable2.Rows.Add(
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    checkSlipNo,
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    DateTime.Now.ToString("yyyy-MM-dd"),
                                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                            }

                            if (seterrorlog == false && setlog == true)
                            {
                                dataTable3.Rows.Add(
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    checkSlipNo,
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    DateTime.Now.ToString("yyyy-MM-dd"),
                                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")
                                                );
                            }

                        }
                    }

               

                    Console.WriteLine("loop dobe :" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        conn.Open();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                        {
                            bulkCopy.DestinationTableName = DBconfig + ".[T_LogDatavalidate_TR_to_Sap]"; // Replace with the name of your target table
                            // Perform the bulk copy
                            bulkCopy.WriteToServer(dataTable);
                        }

                        //Console.WriteLine("t1" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                        //if (parameters2.ToArray().Length > 0)
                        //{
                        //    using (SqlCommand cmd = new SqlCommand(sqlLog_TR.ToString(), conn))
                        //    {
                        //        cmd.ExecuteNonQuery(); 
                        //    }
                        //}

                        //Console.WriteLine("t2" +DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                        //if (parameters3.ToArray().Length > 0)
                        //{
                        //    using (SqlCommand cmd = new SqlCommand(sqlLog_TR_Error.ToString(), conn))
                        //    {
                        //        cmd.ExecuteNonQuery(); 
                        //    }
                        //}
                        //Console.WriteLine("t3" +DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));

                        conn.Close(); 
                    }
                }
                //if (TRErrdata.Rows.Count > 0)
                //{
                   
                //    List<SqlParameter> parametersError = new List<SqlParameter>();
                //    List<SqlParameter> parameters2Error = new List<SqlParameter>();
                //    List<SqlParameter> parameters3Error = new List<SqlParameter>();

                //    paramIndex = 0;
                //    checkSlipNo = "";
                //    foreach (DataRow item in TRErrdata.Rows)
                //    {
                //        try
                //        {
                //            setlog = false;
                //            setsuccesslog = false;
                //            seterrorlog = false;
                //            checkSlipNo = "IT|" + item["SLIPNO"].ToString().Trim();

                //            string Slipno = "IT|" + item["SLIPNO"].ToString().Trim();
                //            string Datatype = "13";
                //            string Type = "TR_redo";
                //            string checkSlipno = item["SLIPNO"].ToString().Trim();

                //            string Plant = item["PlantFrom"].ToString();
                //            string StgeLoc = item["StorageFrom"].ToString();
                //            string EntryQnt = Convert.ToInt32(item["MvmntQty"].ToString()).ToString();
                //            string MovePlant = item["PlantTo"].ToString();
                //            string MoveStloc = item["StorageTo"].ToString();
                //            string Kanban = item["Kanban"].ToString();
                //            string PostDate = item["POSTDATE"].ToString();
                //            string Mat_Type = item["Mat_Type"].ToString();

                //            var RefdocNo = "TR-" + DateTime.Now.ToString("yyMMddHHmm");
                //            string UserID = "";
                //            ws_fn_head.RefDocNo = RefdocNo;
                //            if (Slipno.Contains("|"))
                //            {
                //                UserID = Slipno.Split('|')[0];
                //                Slipno = Slipno.Split('|')[1];
                //            }

                //            string Message = "";
                //            Message += Datatype.Length == 2 ? "" : "Datatype ,".ToString().Trim();
                //            string ValidateMessage = Message != "" ? "Error : ( " + Message + ")" : "";

                //            ws_res = sendSapTR.PostSapTRClass(Slipno, Datatype, Type, Plant, StgeLoc, EntryQnt, MovePlant, MoveStloc, Kanban, PostDate, start_Time, Mat_Type);


                //            if (ws_res.ItDetail.Count() > 0)
                //            {

                //                foreach (var item2 in ws_res.ItDetail)
                //                {

                //                     if (string.IsNullOrEmpty(item2.Error) && !string.IsNullOrEmpty(ws_res.EMaterailDoc.MatDoc))
                //                    {

                //                        sql_redo.Append("('" + Plant + "' , " +
                //                                    "'" + StgeLoc + "', " +
                //                                    "'" + MovePlant + "', " +
                //                                    "'" + MoveStloc + "', " +
                //                                    "'" + Kanban + "'," +
                //                                    "'" + EntryQnt + "', " +
                //                                    "'" + checkSlipno + "', " +
                //                                    "'" + Mat_Type + "', " +
                //                                    "'" + ValidateMessage + "', " +
                //                                    "'" + Type + "', " +
                //                                    "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "', " +
                //                                    "'" + Datatype + "', " +
                //                                    "'1', " +
                //                                    "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "' ),");

                //                        setlog = true;

                //                        sqlLog_TR_redo.Append("('" + item2.Batch + "' , " +
                //                                        "'" + (int)item2.EntryQnt + "', " +
                //                                        "'" + item2.EntryUom + "', " +
                //                                        "'" + item2.FacNo + "', " +
                //                                        "'" + checkSlipno + "'," +
                //                                        "'" + item2.StgeLoc + "|" + item2.MoveStloc + "', " +
                //                                        "'" + item2.MoveType + "', " +
                //                                        "'" + item2.Plant + "|" + item2.MovePlant + "', " +
                //                                        "'" + item2.Custid + "', " +
                //                                        "'" + item2.Kanban + "', " +
                //                                        "'" + DateTime.Now.ToString("yyyy-MM-dd") + "', " +
                //                                        "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "'," +
                //                                        "'" + ws_res.EMaterailDoc.MatDoc + "|" + UserID + "', " +
                //                                        "'" + "TransferStockDataToSAP_311 : " + ws_res.EMaterailDoc.MatDoc + "|" + ws_res.EMaterailDoc.DocYear + "|" + ws_res.EMessage + "|" + item2.Error + "' ),");

                //                        setsuccesslog = true;

                //                    }
                //                    else
                //                    {

                //                        if (item2.Error != "")
                //                        {

                //                            sql_redo.Append("('" + Plant + "' , " +
                //                                        "'" + StgeLoc + "', " +
                //                                        "'" + MovePlant + "', " +
                //                                        "'" + MoveStloc + "', " +
                //                                        "'" + Kanban + "'," +
                //                                        "'" + EntryQnt + "', " +
                //                                        "'" + checkSlipno + "', " +
                //                                        "'" + Mat_Type + "', " +
                //                                        "'" + ValidateMessage + "', " +
                //                                        "'" + Type + "', " +
                //                                        "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "', " +
                //                                        "'" + Datatype + "', " +
                //                                        "'1', " +
                //                                        "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "' ),");

                //                            setlog = true;

                //                            sqlLog_TR_Error_redo.Append("('" + RefdocNo + "|" + UserID + "' , " +
                //                                                    "'" + item2.Batch + "', " +
                //                                                    "'" + (int)item2.EntryQnt + "', " +
                //                                                    "'" + item2.EntryUom + "', " +
                //                                                    "'" + item2.FacNo + "'," +
                //                                                    "'" + Slipno + "', " +
                //                                                    "'" + item2.StgeLoc + "|" + item2.MoveStloc + "', " +
                //                                                    "'" + item2.MoveType +  "', " +
                //                                                    "'" + item2.Plant + "|" + item2.MovePlant + "', " +
                //                                                    "'" + item2.Custid + "', " +
                //                                                    "'" + item2.Kanban + "', " +
                //                                                    "'" + DateTime.Now.ToString("yyyy-MM-dd") + "', " +
                //                                                    "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "'," +
                //                                                    "'" + "TransferStockDataToSAP_311 : " + item2.Error + "' ),");

                //                            seterrorlog = true;
                //                        }
                //                    }
                //                }
                //            }

                //            paramIndex++;

                //        }
                //        catch (Exception ex)
                //        {
                //            string Message = checktable + "Unexpected error Post_TR_to_Sap : " + ex.Message;
                //            if (setlog == false)
                //            {
                //                sql_redo.Append("('' , '', '', '', '','', '" + checkSlipNo + "', '', '', '','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "', '', '1', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "' ),");
                //            }

                //            if (setsuccesslog == false && setlog == true)
                //            {
                //                sqlLog_TR_redo.Append("('' , '', '', '', '" + checkSlipNo + "', '', '', '', '', '','" + DateTime.Now.ToString("yyyy-MM-dd") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "','','' ),");
                //            }

                //            if (seterrorlog == false && setlog == true)
                //            {
                //                sqlLog_TR_Error_redo.Append("('', '', '', '', '','" + checkSlipNo + "', '','', '','', '','" + DateTime.Now.ToString("yyyy-MM-dd") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "','' ),");
                //            }
                //        }
                //    }

                //    sql_redo.Length -= 1;
                //    sql_redo.Append(";");

                //    sqlLog_TR_redo.Length -= 1;
                //    sqlLog_TR_redo.Append(";");

                //    sqlLog_TR_Error_redo.Length -= 1;
                //    sqlLog_TR_Error_redo.Append(";");

                //    using (SqlConnection conn = new SqlConnection(connString))
                //    {
                //        conn.Open(); 

                //        using (SqlCommand cmd = new SqlCommand(sql_redo.ToString(), conn))
                //        {
                //            cmd.ExecuteNonQuery();  
                //        }

                //        if (parameters2Error.ToArray().Length > 0)
                //        {
                //            using (SqlCommand cmd = new SqlCommand(sqlLog_TR_redo.ToString(), conn))
                //            {
                //                cmd.ExecuteNonQuery();  
                //            }
                //        }

                //        if (parameters3Error.ToArray().Length > 0)
                //        {

                //            using (SqlCommand cmd = new SqlCommand(sqlLog_TR_Error_redo.ToString(), conn))
                //            {
                //                cmd.ExecuteNonQuery();  
                //            }
                //        }

                //        conn.Close();  
                //    }
                //}
                //Console.WriteLine("      End Process TR \n");
            }
            catch (Exception ex)
            {
                string Message = checktable + "Unexpected error Post_TR_to_Sap : " + ex.Message;
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

                string sqlGetGI = "SELECT count(*) as countOrder, ORDERNO , From_To as SLoc FROM " + DBconfig + ".[v_sap_batch_gi] where Action = 1 group by  ORDERNO ,From_To ";
                string sqlGetGI_redo = "SELECT count(*) as countOrder, RefDocNo , ORDERNO ,StgeLoc as SLoc FROM " + DBconfig + ".[v_sap_batch_gi_redo] where Action = 1 group by RefDocNo ,ORDERNO ,StgeLoc ";

                DataTable GIdata = Condb.GetQuery(sqlGetGI);
                DataTable GIErrdata = Condb.GetQuery(sqlGetGI_redo);

                Class.ServicePostSapGI sendSapGI = new Class.ServicePostSapGI();
                if (GIdata.Rows.Count > 0)
                {
                    foreach (DataRow item in GIdata.Rows)
                    {
                        string OrderNo = item["ORDERNO"].ToString().Trim();
                        string PoAndDo = item["ORDERNO"].ToString().Trim();
                        string SLoc = item["SLoc"].ToString().Trim();
                        string Type = "GI";
                        string checkPoAndDO = OrderNo.Substring(0, 2);
                        checkPoAndDO = checkPoAndDO == "31" ? "DO" : "PO";
                        string DOandPO = checkPoAndDO;
                        Class.Validate_GRTR Validate_GRTR = new Class.Validate_GRTR();
                        var getID = Validate_GRTR.GetAndUpdate_saveLogData_GI_to_Sap(OrderNo, checkPoAndDO, Type, SLoc);
                        sendSapGI.PostSapGIClass(PoAndDo, DOandPO, getID, SLoc);
                    }
                }

                if (GIErrdata.Rows.Count > 0)
                {
                    foreach (DataRow item in GIErrdata.Rows)
                    {
                        string OrderNo = item["ORDERNO"].ToString().Trim();
                        string PoAndDo = item["ORDERNO"].ToString().Trim();
                        string SLoc = item["SLoc"].ToString().Trim();
                        string Type = "GI_redo";
                        string checkPoAndDO = OrderNo.Substring(0, 2);
                        checkPoAndDO = checkPoAndDO == "31" ? "DO" : "PO";
                        string DOandPO = checkPoAndDO;
                        Class.Validate_GRTR Validate_GRTR = new Class.Validate_GRTR();
                        var getID = Validate_GRTR.GetAndUpdate_saveLogData_GI_to_Sap(OrderNo, checkPoAndDO, Type, SLoc);
                        sendSapGI.PostSapGIClass(PoAndDo, DOandPO, getID, SLoc);
                    }
                }
                Console.WriteLine("      End Process GI \n");
                Console.WriteLine("      #################################################### \n");
            }
            catch (Exception ex)
            {
                string Message = "Unexpected error Post_GI_Sap checkrowGR :" + ex.Message;
                CatchError(Message);
            }
        }

        private void End_update()
        {
            try
            {
                var sql = "UPDATE " + DBconfig + ".[T_SAP_Batch_GR_TR_Log] SET End_Time = @End_Time where Start_Time = '" + start_Time + "'";

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
                System.Environment.Exit(1);
                Application.Exit();
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
                string sqlemailGR = "select  RefDocNo as DocNo , EMessage from " + DBconfig + ".[v_get_dataNotify_gr] where 1 = 1";
                string sqlemailTR = "select  RefDocNo as DocNo , EMessage from " + DBconfig + ".[v_get_dataNotify_tr] where 1 = 1";
                string sqlemailGI = "select  RefDocNo as DocNo , EMessage from " + DBconfig + ".[v_get_dataNotify_gi] where 1 = 1";

                string sqllineGR = "select count(*) totalSum from " + DBconfig + ".[v_get_dataNotify_gr] where Action = 1";
                string sqllineTR = "select count(*) totalSum From (select count(*) TR_Re_NO, SLIPNO, Action from " + DBconfig + ".[v_get_dataNotify_tr] where Action = 1 GROUP BY SLIPNO, Action)D1 ";
                string sqllineGI = "select count(*) totalSum From (select count(*) TR_Re_NO, ORDERNO, Action from " + DBconfig + ".[v_get_dataNotify_gi] where Action = 1 GROUP BY ORDERNO, Action)D1 ";

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
                string checkruntime = getTimeNotify();
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
            if ((hour == "08" && minute < 30) || (hour == "13" && minute < 30))
            {
                flag = "Y";
            }
            else
            {
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

            // ตรวจสอบและปิดการเชื่อมต่อหากเปิดอยู่
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            string dataUpdateList = "UPDATE " + DBconfig + ".[T_SAP_Batch_GR_TR_Log] SET EMessageError = @EMessageError  where start_Time = '" + start_Time + "'";
            using (SqlCommand cmd = new SqlCommand(dataUpdateList, conn))
            {
                cmd.Parameters.AddWithValue("@EMessageError", massage);
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                conn.Close();
            }

            End_update();

        }

    }
}
