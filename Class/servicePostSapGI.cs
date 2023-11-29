using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z_CONFIRM_PICKING_GOODS_ISSUE.Class;
using SAP_Batch_GR_TR.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace SAP_Batch_GR_TR.Class
{
    class servicePostSapGI
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
        public void PostSapGIClass(string PoAndDo, string Type)
        {

            var ws_service = new Z_CONFIRM_PICKING_GOODS_ISSUE_SRV();
            var ws_res = new ZConfirmPickingGoodsIssueResponse();
            var ws_fn_head = new Bapi2017GmHeadRet();
            var ws_fn_det = new ZsgmDetail1();
            var RefdocNo = "GI-" + DateTime.Now.ToString("yyMMddHHmm");
            Results res = new Results();

            List<ZsgmDetail1> Detail_GI = new List<ZsgmDetail1>();
            string DoNumber = "";
            string PoNumber = "";
            if (PoAndDo.Length > 0 || Type == "DO")
            {
                 DoNumber = PoAndDo;
               
            }
            else {
                 PoNumber = PoAndDo;
            }

            ZsgmDetail1 temp = new ZsgmDetail1();
            temp.Batch = "DUMMYBATCH";
            temp.EntryQnt = 0;
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
            Console.WriteLine("post sap!");
            ws_res = ws_service.ZConfirmPickingGoodsIssue(ws_fn_partosap);


            Console.WriteLine("post sap successfully!");
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
            string connString = "";
            if (setting != null)
            {
                connString = setting.ConnectionString;
            }

            Console.WriteLine("Error: " + ws_res.EMessage);
            Console.WriteLine("Error: " + ws_res.EMaterailDoc.MatDoc);

            SqlConnection conn = new SqlConnection(connString);
            var Log_Gr = new List<T_LOG_GR_STOCK>();
            var Log_Error = new List<T_LOG_STOCK_ERROR>();

            string sqlLog_Gi = "INSERT INTO [Barcode_dev].[dbo].[T_LOG_GI_STOCK] "
            + "(Batch, EntryQnt, EntryUom, FacNo, Material, StgeLoc, MoveType, Plant, Custid, Kanban ,StockDate , UpdDate ,DocMat ,EMessage) " +
            "VALUES "
            + "(@Batch, @EntryQnt, @EntryUom, @FacNo, @Material, @StgeLoc, @MoveType, @Plant, @Custid, @Kanban, @StockDate, @UpdDate, @DocMat , @EMessage)";

            DataTable insertDataLogGT = new DataTable();

            string sqlErrorLog_Gr = "INSERT INTO [Barcode_DEV].[dbo].[T_LOG_STOCK_ERROR] "
            + "(RefDocNo ,Batch, EntryQnt, EntryUom, FacNo, Material, StgeLoc, MoveType, Plant, Custid, Kanban ,StockDate , UpdDate  ,EMessage) " +
            "VALUES "
            + "(@RefDocNo ,@Batch, @EntryQnt, @EntryUom, @FacNo, @Material, @StgeLoc, @MoveType, @Plant, @Custid, @Kanban, @StockDate, @UpdDate , @EMessage)";

            DataTable insertDataErrorLogGT = new DataTable();
            Console.WriteLine("start save db");

            if (!string.IsNullOrEmpty(ws_res.EMaterailDoc.MatDoc))
            {

                using (SqlCommand cmd = new SqlCommand(sqlLog_Gi, conn))
                {
                    Console.WriteLine("success");
                    Console.WriteLine("Save T_LOG_GR_STOCK successfully!");
                    cmd.Parameters.AddWithValue("@Batch", "");
                    cmd.Parameters.AddWithValue("@EntryQnt", 0);
                    cmd.Parameters.AddWithValue("@EntryUom", "");
                    cmd.Parameters.AddWithValue("@FacNo", "");
                    cmd.Parameters.AddWithValue("@Material", "");
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

                    int resultseccess = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            else// case error
            {
                Console.WriteLine("ERROR!");
                Console.WriteLine("Save T_LOG_STOCK_ERROR successfully!");
                using (SqlCommand cmd = new SqlCommand(sqlErrorLog_Gr, conn))
                {
                    cmd.Parameters.AddWithValue("@RefdocNo", RefdocNo);
                    cmd.Parameters.AddWithValue("@Batch","");
                    cmd.Parameters.AddWithValue("@EntryQnt", 0);
                    cmd.Parameters.AddWithValue("@EntryUom", "");
                    cmd.Parameters.AddWithValue("@FacNo", "");
                    cmd.Parameters.AddWithValue("@Material","");
                    cmd.Parameters.AddWithValue("@StgeLoc","");
                    cmd.Parameters.AddWithValue("@MoveType","");
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

