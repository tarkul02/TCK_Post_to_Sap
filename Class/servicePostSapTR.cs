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
        public void PostSapTRClass(string SlipNo, string DataType ,string Type)
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
                ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
                string connString = "";
                if (setting != null)
                {
                    connString = setting.ConnectionString;
                }

                SqlConnection conn = new SqlConnection(connString);

                DataTable getdata_tr_and_trredo = new DataTable();
                string sqlSelecttable = DataType == "12" ? "[Barcode_dev].[dbo].[v_sap_batch_tr]" : "[Barcode_dev].[dbo].[v_sap_batch_tr_redo]";
                string sql = "select t.* from " + sqlSelecttable + " t where t.SLIPNO = '" + SlipNo + "' and MAT_TYPE <> 'ZRM'";
                Class.Condb Condb = new Class.Condb();
                getdata_tr_and_trredo = Condb.GetQuery(sql);
            

                if (getdata_tr_and_trredo.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in getdata_tr_and_trredo.Rows)
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
                                ws_fn_head.PstngDate = Convert.ToDateTime(dataRow["POSTDATE"].ToString()).ToString("yyyyMMdd");
                                ws_fn_head.DocDate = Convert.ToDateTime(dataRow["POSTDATE"].ToString()).ToString("yyyyMMdd");
                                getpostdate = true;
                            }
                        }
                        tmp.Material = "";
                        tmp.Plant = dataRow["PlantFrom"].ToString();
                        tmp.StgeLoc = dataRow["StorageFrom"].ToString();
                        tmp.Batch = "DUMMYBATCH";
                        tmp.MoveType = "311";
                        tmp.EntryQnt = Convert.ToInt32(dataRow["MvmntQty"].ToString());
                        tmp.EntryUom = "Pcs";
                        tmp.ItemText = "";
                        tmp.GrRcpt = "";
                        tmp.UnloadPt = "";
                        tmp.FacNo = "";
                        tmp.RefDocYr = "";
                        tmp.RefDoc = "";
                        tmp.RefDocIt = "";
                        tmp.MovePlant = dataRow["PlantTo"].ToString();
                        tmp.MoveStloc = dataRow["StorageTo"].ToString();
                        tmp.MoveBatch = "DUMMYBATCH";
                        tmp.SoldTo = "";
                        tmp.Custid = "";
                        tmp.Kanban = dataRow["Kanban"].ToString();
                        tmp.Amount = "";
                        DetailToSap.Add(tmp);

                    }
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

                    string sqlLog_Gr = "INSERT INTO [Barcode_DEV].[dbo].[T_LOG_GR_STOCK] "
                    + "(Batch, EntryQnt, EntryUom, FacNo, Material, StgeLoc, MoveType, Plant, Custid, Kanban ,StockDate , UpdDate ,DocMat ,EMessage) " +
                    "VALUES "
                    + "(@Batch, @EntryQnt, @EntryUom, @FacNo, @Material, @StgeLoc, @MoveType, @Plant, @Custid, @Kanban, @StockDate, @UpdDate, @DocMat , @EMessage)";

                    DataTable insertDataLogGT = new DataTable();

                    string sqlErrorLog_Gr = "INSERT INTO [Barcode_DEV].[dbo].[T_LOG_STOCK_ERROR] "
                    + "(RefDocNo ,Batch, EntryQnt, EntryUom, FacNo, Material, StgeLoc, MoveType, Plant, Custid, Kanban ,StockDate , UpdDate  ,EMessage) " +
                    "VALUES "
                    + "(@RefDocNo ,@Batch, @EntryQnt, @EntryUom, @FacNo, @Material, @StgeLoc, @MoveType, @Plant, @Custid, @Kanban, @StockDate, @UpdDate , @EMessage)";

                    DataTable insertDataErrorLogGT = new DataTable();

                    string dataUpdateList = "UPDATE [Barcode_dev].[dbo].[T_barcode_trans] where SLIPNO = '" + SlipNo + "'";
                    DataTable UpdateList = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(dataUpdateList, conn))
                    {

                        if (ws_res.EMessage.Contains("was create"))
                        {
                            cmd.Parameters.AddWithValue("@REFDOCSAP", ws_res.EMessage);
                            cmd.Parameters.AddWithValue("@CONFIRM_DATE", DateTime.Now);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@REFDOCSAP", ws_res.EMessage);
                        }
                    }

                    if (ws_res.ItDetail.Count() > 0)
                    {
                        foreach (var item in ws_res.ItDetail)
                        {
                            if (string.IsNullOrEmpty(item.Error) && !string.IsNullOrEmpty(ws_res.EMaterailDoc.MatDoc))
                            {
                                using (SqlCommand cmd = new SqlCommand(sqlLog_Gr, conn))
                                {
                                    cmd.Parameters.AddWithValue("@Batch", item.Batch);
                                    cmd.Parameters.AddWithValue("@EntryQnt", (int)item.EntryQnt);
                                    cmd.Parameters.AddWithValue("@EntryUom", item.EntryUom);
                                    cmd.Parameters.AddWithValue("@FacNo", item.FacNo);
                                    cmd.Parameters.AddWithValue("@Material", SlipNo);
                                    cmd.Parameters.AddWithValue("@StgeLoc", item.StgeLoc + "|" + item.MoveStloc);
                                    cmd.Parameters.AddWithValue("@MoveType", item.MoveType);
                                    cmd.Parameters.AddWithValue("@Plant", item.Plant + "|" + item.MovePlant);
                                    cmd.Parameters.AddWithValue("@Custid", item.Custid);
                                    cmd.Parameters.AddWithValue("@Kanban", item.Kanban);
                                    cmd.Parameters.AddWithValue("@StockDate", Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")));
                                    cmd.Parameters.AddWithValue("@UpdDate", DateTime.Now);
                                    cmd.Parameters.AddWithValue("@DocMat", ws_res.EMaterailDoc.MatDoc + "|" + UserID);
                                    cmd.Parameters.AddWithValue("@EMessage", "TransferStockDataToSAP_311 : " + ws_res.EMaterailDoc.MatDoc + "|" + ws_res.EMaterailDoc.DocYear + "|" + ws_res.EMessage + "|" + item.Error);
                                    conn.Open();

                                    int resultseccess = cmd.ExecuteNonQuery();
                                    conn.Close();
                                }
                            }
                            else
                            {
                                if (item.Error != "")
                                {
                                    using (SqlCommand cmd = new SqlCommand(sqlErrorLog_Gr, conn))
                                    {
                                        cmd.Parameters.AddWithValue("@RefdocNo", RefdocNo + "|" + UserID);
                                        cmd.Parameters.AddWithValue("@Batch", item.Batch);
                                        cmd.Parameters.AddWithValue("@EntryQnt", (int)item.EntryQnt);
                                        cmd.Parameters.AddWithValue("@EntryUom", item.EntryUom);
                                        cmd.Parameters.AddWithValue("@FacNo", item.FacNo);
                                        cmd.Parameters.AddWithValue("@Material", SlipNo);
                                        cmd.Parameters.AddWithValue("@StgeLoc", item.StgeLoc + "|" + item.MoveStloc);
                                        cmd.Parameters.AddWithValue("@MoveType", item.MoveType);
                                        cmd.Parameters.AddWithValue("@Plant", item.Plant + "|" + item.MovePlant);
                                        cmd.Parameters.AddWithValue("@Custid", item.Custid);
                                        cmd.Parameters.AddWithValue("@Kanban", item.Kanban);
                                        cmd.Parameters.AddWithValue("@StockDate", Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")));
                                        cmd.Parameters.AddWithValue("@UpdDate", DateTime.Now);
                                        cmd.Parameters.AddWithValue("@DocMat", ws_res.EMaterailDoc.MatDoc + "|" + UserID);
                                        cmd.Parameters.AddWithValue("@EMessage", "TransferStockDataToSAP_311 : " + item.Error);
                                        conn.Open();

                                        int resultError = cmd.ExecuteNonQuery();
                                        conn.Close();
                                    }
                                }
                            }
                        }
                    }

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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //res.status = false;
                //res.message = "Web service status : False";
                //res.message3 = e.Message.ToString();
            }


        }
    }
}

