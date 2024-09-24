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
        public ZGoodsmvtCreate1Response PostSapTRClass(string SlipNo, string DataType, string Type , string Plant, string StgeLoc, string EntryQnt , string MovePlant , string MoveStloc, string Kanban, string PostDate ,string start_Time , string Mat_Type)
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

            return ws_res;

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

