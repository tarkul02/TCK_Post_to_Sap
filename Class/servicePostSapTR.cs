using System;
using System.Collections.Generic;
using System.Linq;
using SapApiGRAndTR.Class;
using SAP_Batch_GR_TR.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using PostSap_GR_TR.Models;

namespace PostSap_GR_TR.Class
{
    class ServicePostSapTR
    {
        string DBconfig = ConfigurationManager.AppSettings["Databaseconfig"];
        string checkError;
        public void PostSapTRClass(string SlipNo, string DataType, string Type , string Plant, string StgeLoc, string EntryQnt , string MovePlant , string MoveStloc, string Kanban, string PostDate ,string start_Time , string Mat_Type)
        {
         
            var ws_service = new Z_GOODSMVT_CREATE1_SRV();
            var ws_res = new ZGoodsmvtCreate1Response();
            var ws_fn_head = new ZsgmHeader();
            var ws_fn_det = new ZsgmDetail1();
            var GmCode = new Bapi2017GmCode();
            var RefdocNo = "TR-" + DateTime.Now.ToString("yyMMddHHmm");
            Results res = new Results();
            var db = new T_LOCATION_SAP();
            List<ZsgmDetail1> DetailToSap = new List<ZsgmDetail1>();
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
            string connString = "";
            if (setting != null)
            {
                connString = setting.ConnectionString;
            }

            SqlConnection conn = new SqlConnection(connString);
            var getpostdate = false;
            string UserID = "";
            ws_fn_head.RefDocNo = RefdocNo;
            if (SlipNo.Contains("|"))
            {
                UserID = SlipNo.Split('|')[0];
                SlipNo = SlipNo.Split('|')[1];
            }


            int SlipNolength = SlipNo.Length;
            if (SlipNolength > 25)
            {
                ws_fn_head.HeaderTxt = SlipNo.Substring(0, 25);
            }
            else
            {
                ws_fn_head.HeaderTxt = SlipNo;
            }
            ws_fn_head.BillOfLading = "";
            ws_fn_head.GrGiSlipNo = "";
            GmCode.GmCode = "04";

            try
            {

                ZsgmDetail1 tmp = new ZsgmDetail1();
                if (getpostdate == false)
                {
                    if (DataType == "11")
                    {
                        ws_fn_head.PstngDate = DateTime.Now.ToString("yyyyMMdd");
                        ws_fn_head.DocDate = DateTime.Now.ToString("yyyyMMdd");
                    }
                    else
                    {
                        ws_fn_head.PstngDate = Convert.ToDateTime(PostDate.ToString()).ToString("yyyyMMdd");
                        ws_fn_head.DocDate = Convert.ToDateTime(PostDate.ToString()).ToString("yyyyMMdd");
                        getpostdate = true;
                    }
                }
                tmp.Material = "";
                tmp.Plant = Plant;
                tmp.StgeLoc = StgeLoc;
                tmp.Batch = "DUMMYBATCH";
                tmp.MoveType = "311";
                tmp.EntryQnt = Convert.ToInt32(EntryQnt.ToString());
                tmp.EntryUom = "Pcs";
                tmp.ItemText = "";
                tmp.GrRcpt = "";
                tmp.UnloadPt = "";
                tmp.FacNo = "";
                tmp.RefDocYr = "";
                tmp.RefDoc = "";
                tmp.RefDocIt = "";
                tmp.MovePlant = MovePlant;
                tmp.MoveStloc = MoveStloc;
                tmp.MoveBatch = "DUMMYBATCH";
                tmp.SoldTo = "";
                tmp.Custid = "";
                tmp.Kanban = Kanban;
                tmp.Amount = "";
                DetailToSap.Add(tmp);

                    
                List<ZsgmDetail1> result = new List<ZsgmDetail1>();
                result = DetailToSap.GroupBy(l => l.Kanban)
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
                                        MovePlant = cl.First().MovePlant,
                                        MoveBatch = cl.First().MoveBatch,
                                        Plant = cl.First().Plant,
                                        SoldTo = cl.First().SoldTo,
                                        Custid = cl.First().Custid,
                                        Kanban = cl.First().Kanban,
                                    }).OrderBy(l => l.Kanban).ToList();

                ZGoodsmvtCreate1 ws_fn_partosap = new ZGoodsmvtCreate1();
                ws_fn_partosap.IsHeader = ws_fn_head;
                ws_fn_partosap.ItDetail = result.ToArray();
                ws_fn_partosap.IGoodsmvtCode = GmCode;
                //ส่งไปให้ SAP
          
                ws_res = ws_service.ZGoodsmvtCreate1(ws_fn_partosap);
               
                BarcodeEntities UpdateBarcode = new BarcodeEntities();
                List<T_LOG_GR_STOCK> Log_Gr = new List<T_LOG_GR_STOCK>();
                List<T_LOG_STOCK_ERROR> Log_Error = new List<T_LOG_STOCK_ERROR>();
                    
                string sqlLog_Gr = "INSERT INTO "+ DBconfig +".[T_LOG_GR_STOCK] "
                + "(Batch, EntryQnt, EntryUom, FacNo, Material, StgeLoc, MoveType, Plant, Custid, Kanban ,StockDate , UpdDate ,DocMat ,EMessage) " +
                "VALUES "
                + "(@Batch, @EntryQnt, @EntryUom, @FacNo, @Material, @StgeLoc, @MoveType, @Plant, @Custid, @Kanban, @StockDate, @UpdDate, @DocMat , @EMessage)";

                DataTable insertDataLogGT = new DataTable();

                string sqlErrorLog_Gr = "INSERT INTO "+ DBconfig +".[T_LOG_STOCK_ERROR] "
                + "(RefDocNo ,Batch, EntryQnt, EntryUom, FacNo, Material, StgeLoc, MoveType, Plant, Custid, Kanban ,StockDate , UpdDate  ,EMessage) " +
                "VALUES "
                + "(@RefDocNo ,@Batch, @EntryQnt, @EntryUom, @FacNo, @Material, @StgeLoc, @MoveType, @Plant, @Custid, @Kanban, @StockDate, @UpdDate , @EMessage)";
             
                DataTable insertDataErrorLogGT = new DataTable();
                //string UpdateStatusSap = "UPDATE "+ DBconfig +".[T_LogDatavalidate_TR_to_Sap] SET SapStatus = @SapStatus , ConfirmDate = @ConfirmDate  where ID = '" + getID + "'";
                string dataUpdateList = "UPDATE "+ DBconfig +".[T_barcode_trans] set REFDOCSAP = @REFDOCSAP , CONFIRM_DATE = @CONFIRM_DATE where SLIPNO = '" + SlipNo + "'";
                DataTable UpdateList = new DataTable();


                if (ws_res.EMessage != null)
                {
                    using (SqlCommand cmd = new SqlCommand(dataUpdateList, conn))
                    {

                        if (ws_res.EMessage.Contains("was create"))
                        {
                            cmd.Parameters.AddWithValue("@REFDOCSAP", ws_res.EMessage);
                            cmd.Parameters.AddWithValue("@CONFIRM_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));

                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@REFDOCSAP", ws_res.EMessage);
                            cmd.Parameters.AddWithValue("@CONFIRM_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                        }
                        conn.Open();

                        int resultError = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                } else {
                    using (SqlCommand cmd = new SqlCommand(dataUpdateList, conn))
                    {
                        cmd.Parameters.AddWithValue("@REFDOCSAP", "");
                        cmd.Parameters.AddWithValue("@CONFIRM_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                        ExecuteSqlCommand(conn, cmd);
                    }
                }

                //if (ws_res.ItDetail.Count() > 0)
                //{
                    
                //    foreach (var item in ws_res.ItDetail)
                //    {

                //        if (string.IsNullOrEmpty(item.Error) && !string.IsNullOrEmpty(ws_res.EMaterailDoc.MatDoc))
                //        {


                //            //using (SqlCommand cmd = new SqlCommand(UpdateStatusSap, conn))
                //            //{
                //            //    cmd.Parameters.AddWithValue("@SapStatus", 1);
                //            //    cmd.Parameters.AddWithValue("@ConfirmDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                //            //    ExecuteSqlCommand(conn, cmd);
                //            //}

                //            using (SqlCommand cmd = new SqlCommand(sqlLog_Gr, conn))
                //            {
                //                cmd.Parameters.AddWithValue("@Batch", item.Batch);
                //                cmd.Parameters.AddWithValue("@EntryQnt", (int)item.EntryQnt);
                //                cmd.Parameters.AddWithValue("@EntryUom", item.EntryUom);
                //                cmd.Parameters.AddWithValue("@FacNo", item.FacNo);
                //                cmd.Parameters.AddWithValue("@Material", SlipNo);
                //                cmd.Parameters.AddWithValue("@StgeLoc", item.StgeLoc + "|" + item.MoveStloc);
                //                cmd.Parameters.AddWithValue("@MoveType", item.MoveType);
                //                cmd.Parameters.AddWithValue("@Plant", item.Plant + "|" + item.MovePlant);
                //                cmd.Parameters.AddWithValue("@Custid", item.Custid);
                //                cmd.Parameters.AddWithValue("@Kanban", item.Kanban);
                //                cmd.Parameters.AddWithValue("@StockDate", Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")));
                //                cmd.Parameters.AddWithValue("@UpdDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                //                cmd.Parameters.AddWithValue("@DocMat", ws_res.EMaterailDoc.MatDoc + "|" + UserID);
                //                cmd.Parameters.AddWithValue("@EMessage", "TransferStockDataToSAP_311 : " + ws_res.EMaterailDoc.MatDoc + "|" + ws_res.EMaterailDoc.DocYear + "|" + ws_res.EMessage + "|" + item.Error);
                //                ExecuteSqlCommand(conn, cmd);
                //            }
                //        }
                //        else
                //        {

                //            if (item.Error != "")
                //            {
                //                using (SqlCommand cmd = new SqlCommand(sqlErrorLog_Gr, conn))
                //                {
                //                    cmd.Parameters.AddWithValue("@RefdocNo", RefdocNo + "|" + UserID);
                //                    cmd.Parameters.AddWithValue("@Batch", item.Batch);
                //                    cmd.Parameters.AddWithValue("@EntryQnt", (int)item.EntryQnt);
                //                    cmd.Parameters.AddWithValue("@EntryUom", item.EntryUom);
                //                    cmd.Parameters.AddWithValue("@FacNo", item.FacNo);
                //                    cmd.Parameters.AddWithValue("@Material", SlipNo);
                //                    cmd.Parameters.AddWithValue("@StgeLoc", item.StgeLoc + "|" + item.MoveStloc);
                //                    cmd.Parameters.AddWithValue("@MoveType", item.MoveType);
                //                    cmd.Parameters.AddWithValue("@Plant", item.Plant + "|" + item.MovePlant);
                //                    cmd.Parameters.AddWithValue("@Custid", item.Custid);
                //                    cmd.Parameters.AddWithValue("@Kanban", item.Kanban);
                //                    cmd.Parameters.AddWithValue("@StockDate", Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")));
                //                    cmd.Parameters.AddWithValue("@UpdDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                //                    cmd.Parameters.AddWithValue("@DocMat", ws_res.EMaterailDoc.MatDoc + "|" + UserID);
                //                    cmd.Parameters.AddWithValue("@EMessage", "TransferStockDataToSAP_311 : " + item.Error);

                //                    ExecuteSqlCommand(conn , cmd);

                //                }
                //            }
                //        }
                //    }
                //}
              
                var Matdoc = "";
                var Errmsg = "";
                if (ws_res.EMaterailDoc.MatDoc == "")
                {
                    Matdoc = "No MatDoc";
                }
                else
                {
                    Matdoc = ws_res.EMaterailDoc.MatDoc;
                }
                foreach (var upd in ws_res.ItDetail)
                {
                    if (upd.Error != "")
                    {
                        Errmsg += upd.Kanban + ": " + upd.Error + "\n";
                    }
                }

                res.status = true;
                res.message = "Transfer : success ";
                res.message2 = "\nmatdoc :" + Matdoc;
                res.message3 = "\nError massage : \n" + Errmsg;
                
            }
            catch (Exception ex)
            {
                _ = new DataTable();
                _ = new Class.ServicePostSapGR();
                Class.Condb Condb = new Class.Condb();
              
                string dataUpdateList = "UPDATE " + DBconfig + ".[T_SAP_Batch_GR_TR_Log] SET EMessageErrorTR = @EMessageErrorTR  where start_Time = '" + start_Time + "'";
                using (SqlCommand cmd = new SqlCommand(dataUpdateList, conn))
                {
                    cmd.Parameters.AddWithValue("@EMessageErrorTR", "Errorlocaltion :" + checkError +", message : " + ex.Message);
                    conn.Open();
                    int result = cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }

        }

        public void ExecuteSqlCommand(SqlConnection conn, SqlCommand cmd)
        {
            // ตรวจสอบและปิดการเชื่อมต่อหากเปิดอยู่
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}

