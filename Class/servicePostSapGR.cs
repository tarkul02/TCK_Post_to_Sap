using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEV_Z_GOODSMVT_CREATE1.Class;
using SAP_Batch_GR_TR.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace SAP_Batch_GR_TR.Class
{
    class servicePostSapGR
    {
        public void PostSapGRClass(string partno, int Qty, string Custid, string Store, string postdate, string headerText)  //List<ZsgmDetail> zsgms, List<T_barcode_trans> t_Barcodes, string Kanban
        {

            //example from excel postdate = 2022-09-01
            try
            {
                partno = partno.Trim();
                BarcodeEntities db = new BarcodeEntities();

                T_LOCATION_SAP Master_T_LOCATION_SAP = new T_LOCATION_SAP();
                var ws_service = new Z_GOODSMVT_CREATE1_SRV();
                var ws_res = new ZGoodsmvtCreate1Response();
                var ws_fn_head = new ZsgmHeader();
                var ws_fn_det = new ZsgmDetail1();
                var GmCode = new Bapi2017GmCode();
                ws_fn_head.BillOfLading = "";
                ws_fn_head.DocDate = DateTime.Now.ToString("yyyyMMdd");
                ws_fn_head.GrGiSlipNo = "";
                string UserID = "";
                if (headerText.Contains("|"))
                {
                    UserID = headerText.Split('|')[0];
                    headerText = headerText.Split('|')[1];
                }

                var RefdocNo = headerText;
                ws_fn_head.HeaderTxt = headerText;//"ADDSTOCKBYDEV";
                //ws_fn_head.PstngDate = "20220901";
                ws_fn_head.PstngDate = postdate.Replace("-", "");
                //ws_fn_head.PstngDate = DateTime.Now.ToString("yyyyMM01");
             
                GmCode.GmCode = "05";

                var Time = DateTime.Now.ToString("yyyy-MM-dd");
                var _Time = Time.Split('-');
                var year = _Time[0].Substring(_Time[0].Length - 1);
                var Month = (Convert.ToInt32(_Time[1])).ToString();
                var Day = (Convert.ToInt32(_Time[2])).ToString();

                DataTable checkDatamaster = new DataTable();
                
                List<ZsgmDetail1> Detail_GR = new List<ZsgmDetail1>();

                ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
                string connString = "";
                if (setting != null)
                {
                    connString = setting.ConnectionString;
                }

                string sql = "SELECT TOP 1 * FROM [Barcode_dev].[dbo].[T_LOCATION_SAP] where LOC_SAP_ID ='" + Store + "'";
                SqlConnection conn = new SqlConnection(connString);
                Class.Condb Condb = new Class.Condb();
                checkDatamaster = Condb.GetQuery(sql);

                var Lot = Custid + year + Month + Day + "N00";
                ZsgmDetail1 temp = new ZsgmDetail1();
                temp.Batch = "DUMMYBATCH";
                temp.EntryQnt = Convert.ToDecimal(Qty);
                temp.EntryUom = "Pcs";
                temp.FacNo = checkDatamaster.Rows[0]["FAC"].ToString();
                temp.Material = partno;
                temp.StgeLoc = Store;
                temp.MoveType = "521";
                temp.Plant = checkDatamaster.Rows[0]["PLANT"].ToString();
                temp.Custid = Custid; //tmp.CUSTID;
                temp.Kanban = ""; //tmp.KANBANID;
                Detail_GR.Add(temp);


                List<ZsgmDetail1> result = new List<ZsgmDetail1>();

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

                var ws_fn_partosap = new ZGoodsmvtCreate1();
                ws_fn_partosap.IsHeader = ws_fn_head;
                ws_fn_partosap.ItDetail = result.ToArray();
                ws_fn_partosap.IGoodsmvtCode = GmCode;
                //ส่งไปให้ SAP
                //ws_res = ws_service.ZGoodsmvtCreate1(ws_fn_partosap);
                Console.WriteLine("post sap successfully!");
                var Log_Gr = new List<T_LOG_GR_STOCK>();
                var Log_Error = new List<T_LOG_STOCK_ERROR>();

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
                Console.WriteLine("start save db");
                if (ws_res.ItDetail.Count() > 0)
                {
                    foreach (var item in ws_res.ItDetail)
                    {
                        if (string.IsNullOrEmpty(item.Error) && !string.IsNullOrEmpty(ws_res.EMaterailDoc.MatDoc))
                        {

                            using (SqlCommand cmd = new SqlCommand(sqlLog_Gr, conn))
                            {
                                Console.WriteLine("success");
                                Console.WriteLine("Save T_LOG_GR_STOCK successfully!");
                                cmd.Parameters.AddWithValue("@Batch", item.Batch);
                                cmd.Parameters.AddWithValue("@EntryQnt", (int)item.EntryQnt);
                                cmd.Parameters.AddWithValue("@EntryUom", item.EntryUom);
                                cmd.Parameters.AddWithValue("@FacNo", item.FacNo);
                                cmd.Parameters.AddWithValue("@Material", item.Material);
                                if (string.IsNullOrEmpty(item.MoveStloc))
                                {
                                    cmd.Parameters.AddWithValue("@StgeLoc", item.StgeLoc);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@StgeLoc", item.StgeLoc + "|" + item.MoveStloc);
                                }
                                cmd.Parameters.AddWithValue("@MoveType", item.MoveType);

                                if (string.IsNullOrEmpty(item.MovePlant))
                                {
                                    cmd.Parameters.AddWithValue("@Plant", item.Plant);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@Plant", item.Plant + "|" + item.MovePlant);
                                }

                                cmd.Parameters.AddWithValue("@Custid", item.Custid);
                                cmd.Parameters.AddWithValue("@Kanban", item.Kanban);
                                cmd.Parameters.AddWithValue("@StockDate", Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")));
                                cmd.Parameters.AddWithValue("@UpdDate", DateTime.Now);
                                cmd.Parameters.AddWithValue("@DocMat", ws_res.EMaterailDoc.MatDoc + "|" + UserID);
                                cmd.Parameters.AddWithValue("@EMessage", "ADDSTOCKBYEXCEL : " + ws_res.EMessage);
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
                                cmd.Parameters.AddWithValue("@RefdocNo", RefdocNo + "|" + UserID);
                                cmd.Parameters.AddWithValue("@Batch", item.Batch);
                                cmd.Parameters.AddWithValue("@EntryQnt", (int)item.EntryQnt);
                                cmd.Parameters.AddWithValue("@EntryUom", item.EntryUom);
                                cmd.Parameters.AddWithValue("@FacNo", item.FacNo);
                                cmd.Parameters.AddWithValue("@Material", item.Material);
                                if (string.IsNullOrEmpty(item.MoveStloc))
                                {
                                    cmd.Parameters.AddWithValue("@StgeLoc", item.StgeLoc);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@StgeLoc", item.StgeLoc + "|" + item.MoveStloc);
                                }
                                cmd.Parameters.AddWithValue("@MoveType", item.MoveType);

                                if (string.IsNullOrEmpty(item.MovePlant))
                                {
                                    cmd.Parameters.AddWithValue("@Plant", item.Plant);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@Plant", item.Plant + "|" + item.MovePlant);
                                }

                                cmd.Parameters.AddWithValue("@Custid", item.Custid);
                                cmd.Parameters.AddWithValue("@Kanban", item.Kanban);
                                cmd.Parameters.AddWithValue("@StockDate", Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")));
                                cmd.Parameters.AddWithValue("@UpdDate", DateTime.Now);
                                cmd.Parameters.AddWithValue("@DocMat", ws_res.EMaterailDoc.MatDoc + "|" + UserID);
                                cmd.Parameters.AddWithValue("@EMessage", item.Error == "" ? "ADDSTOCKBYEXCEL : " + ws_res.EMessage : "ADDSTOCKBYEXCEL : " + item.Error);
                                conn.Open();

                                int resultError = cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
