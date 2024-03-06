using System;
using System.Collections.Generic;
using System.Linq;
using SapApiGI.Class;
using SAP_Batch_GR_TR.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using PostSap_GR_TR.Models;

namespace PostSap_GR_TR.Class
{
    class ServicePostSapGI
    {
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
        public void PostSapGIClass(string PoAndDo, string DOandPO ,string getID)
        {

            var ws_service = new Z_CONFIRM_PICKING_GOODS_ISSUE_SRV();
            var ws_res = new ZConfirmPickingGoodsIssueResponse();
            var ws_fn_head = new Bapi2017GmHeadRet();
            var ws_fn_det = new ZsgmDetail1();
            var RefdocNo = "GI-" + DateTime.Now.ToString("yyMMddHHmm");
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
            string connString = "";
            if (setting != null)
            {
                connString = setting.ConnectionString;
            }

            SqlConnection conn = new SqlConnection(connString);
            Results res = new Results();

            List<ZsgmDetail1> Detail_GI = new List<ZsgmDetail1>();
            string DoNumber = "";
            string PoNumber = "";
            if (PoAndDo.Length > 0 && DOandPO == "DO")
            {
                DoNumber = PoAndDo;

            }
            else
            {
                PoNumber = PoAndDo;
            }

            ZsgmDetail1 temp = new ZsgmDetail1();
            temp.Batch = "DUMMYBATCH";
            temp.EntryQnt = Convert.ToInt32(null);
            temp.EntryUom = "";
            temp.FacNo = "";
            temp.Material = "";
            temp.StgeLoc = "";
            temp.MoveType = "";
            temp.Plant = "";
            temp.Custid = ""; //tmp.CUSTID;
            temp.Kanban = ""; //tmp.KANBANID;
            temp.IDoNumber = "";
            temp.IPoNumber = "";
            temp.IStgeLoc = "";
            Detail_GI.Add(temp);

            List<ZsgmDetail1> result = new List<ZsgmDetail1>();

            result = Detail_GI.GroupBy(l => l.Kanban)
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

            var ws_fn_partosap = new ZConfirmPickingGoodsIssue();
            ws_fn_partosap.IDoNumber = DoNumber;
            ws_fn_partosap.IPoNumber = PoNumber;
            ws_fn_partosap.ItDetail = result.ToArray();
            ws_fn_partosap.IStgeLoc = "";
            //ส่งไปให้ SAP
            ws_res = ws_service.ZConfirmPickingGoodsIssue(ws_fn_partosap);

            string dataUpdateList = "UPDATE [Barcode].[dbo].[T_barcode_trans] where ORDERNO = '" + PoAndDo + "'";
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

            var Log_Gr = new List<T_LOG_GR_STOCK>();
            var Log_Error = new List<T_LOG_STOCK_ERROR>();

            string sqlLog_Gi = "INSERT INTO [Barcode].[dbo].[T_LOG_GI_STOCK] "
            + "(Batch, EntryQnt, EntryUom, FacNo, Material, StgeLoc, MoveType, Plant, Custid, Kanban ,StockDate , UpdDate ,DocMat ,EMessage) " +
            "VALUES "
            + "(@Batch, @EntryQnt, @EntryUom, @FacNo, @Material, @StgeLoc, @MoveType, @Plant, @Custid, @Kanban, @StockDate, @UpdDate, @DocMat , @EMessage)";

            DataTable insertDataLogGT = new DataTable();

            string sqlErrorLog_Gr = "INSERT INTO [Barcode].[dbo].[T_LOG_STOCK_ERROR] "
            + "(RefDocNo ,Batch, EntryQnt, EntryUom, FacNo, Material, StgeLoc, MoveType, Plant, Custid, Kanban ,StockDate , UpdDate  ,EMessage) " +
            "VALUES "
            + "(@RefDocNo ,@Batch, @EntryQnt, @EntryUom, @FacNo, @Material, @StgeLoc, @MoveType, @Plant, @Custid, @Kanban, @StockDate, @UpdDate , @EMessage)";

            DataTable insertDataErrorLogGT = new DataTable();
            string UpdateStatusSap = "UPDATE [Barcode].[dbo].[T_LogDatavalidate_GI_to_Sap] SET SapStatus = @SapStatus , ConfirmDate = @ConfirmDate  where ID = '" + getID + "'";

            if (!string.IsNullOrEmpty(ws_res.EMaterailDoc.MatDoc))
            {
                using (SqlCommand cmd = new SqlCommand(UpdateStatusSap, conn))
                {
                    cmd.Parameters.AddWithValue("@SapStatus", 1);
                    cmd.Parameters.AddWithValue("@ConfirmDate", DateTime.Now);
                    conn.Open();
                    int resultsap = cmd.ExecuteNonQuery();
                    conn.Close();
                }

                using (SqlCommand cmd = new SqlCommand(sqlLog_Gi, conn))
                {
                    cmd.Parameters.AddWithValue("@Batch", "");
                    cmd.Parameters.AddWithValue("@EntryQnt", 0);
                    cmd.Parameters.AddWithValue("@EntryUom", "");
                    cmd.Parameters.AddWithValue("@FacNo", "");
                    cmd.Parameters.AddWithValue("@Material", PoAndDo);
                    cmd.Parameters.AddWithValue("@StgeLoc", "");
                    cmd.Parameters.AddWithValue("@MoveType", "");
                    cmd.Parameters.AddWithValue("@Plant", "");

                    cmd.Parameters.AddWithValue("@Custid", "");
                    cmd.Parameters.AddWithValue("@Kanban", "");
                    cmd.Parameters.AddWithValue("@StockDate", Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")));
                    cmd.Parameters.AddWithValue("@UpdDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@DocMat", ws_res.EMaterailDoc.MatDoc + "|IT");
                    cmd.Parameters.AddWithValue("@EMessage", "Z_CONFIRM_PICKING_GOODS_ISSUE : " + ws_res.EMessage);
                    conn.Open();

                    int resultseccess = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            else// case error
            {
                using (SqlCommand cmd = new SqlCommand(sqlErrorLog_Gr, conn))
                {
                    cmd.Parameters.AddWithValue("@RefdocNo", RefdocNo);
                    cmd.Parameters.AddWithValue("@Batch", "");
                    cmd.Parameters.AddWithValue("@EntryQnt", 0);
                    cmd.Parameters.AddWithValue("@EntryUom", "");
                    cmd.Parameters.AddWithValue("@FacNo", "");
                    cmd.Parameters.AddWithValue("@Material", PoAndDo);
                    cmd.Parameters.AddWithValue("@StgeLoc", "");
                    cmd.Parameters.AddWithValue("@MoveType", "");
                    cmd.Parameters.AddWithValue("@Plant", "");

                    cmd.Parameters.AddWithValue("@Custid", "");
                    cmd.Parameters.AddWithValue("@Kanban", "");
                    cmd.Parameters.AddWithValue("@StockDate", Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")));
                    cmd.Parameters.AddWithValue("@UpdDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@DocMat", ws_res.EMaterailDoc.MatDoc);
                    cmd.Parameters.AddWithValue("@EMessage", "Z_CONFIRM_PICKING_GOODS_ISSUE : " + ws_res.EMessage);
                    conn.Open();
                    int resultError = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
    }
}

