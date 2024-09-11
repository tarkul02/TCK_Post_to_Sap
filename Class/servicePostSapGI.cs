using System;
using SapApiGI.Class;
using SAP_Batch_GR_TR.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;


namespace PostSap_GR_TR.Class
{
    class ServicePostSapGI
    {
        string DBconfig = ConfigurationManager.AppSettings["Databaseconfig"];
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

        //เช็ค data to json
        public static string ConvertObjectArrayToString(object[] objects)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Object Array:");

            foreach (var obj in objects)
            {
                sb.AppendLine(InspectObject(obj));
            }

            return sb.ToString();
        }

        public static string InspectObject(object obj, int indentLevel = 0)
        {
            if (obj == null)
            {
                return "null";
            }

            var type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var sb = new StringBuilder();

            string indent = new string(' ', indentLevel * 2);
            sb.AppendLine($"{indent}Type: {type.Name}");

            sb.AppendLine($"{indent}Properties:");
            foreach (var prop in properties)
            {
                object value = prop.GetValue(obj);
                if (value != null && !IsSimpleType(value.GetType()))
                {
                    sb.AppendLine($"{indent}  {prop.Name}:");
                    sb.Append(InspectObject(value, indentLevel + 2));
                }
                else
                {
                    sb.AppendLine($"{indent}  {prop.Name}: {value}");
                }
            }

            sb.AppendLine($"{indent}Fields:");
            foreach (var field in fields)
            {
                object value = field.GetValue(obj);
                if (value != null && !IsSimpleType(value.GetType()))
                {
                    sb.AppendLine($"{indent}  {field.Name}:");
                    sb.Append(InspectObject(value, indentLevel + 2));
                }
                else
                {
                    sb.AppendLine($"{indent}  {field.Name}: {value}");
                }
            }

            return sb.ToString();
        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive ||
                   type.IsEnum ||
                   type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid);
        }

        //end เช็ค data to json

        public void PostSapGIClass(string PoAndDo, string DOandPO, string getID, string SLoc)
        {

            var ws_service = new Z_CONFIRM_PICKING_GOODS_ISSUE_SRV();
            var ws_res = new ZConfirmPickingGoodsIssueResponse();
            // var ws_fn_head = new Bapi2017GmHeadRet();
            var RefdocNo = "GI-" + DateTime.Now.ToString("yyMMddHHmm");
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["BarcodeEntities"];
            string connString = "";
            if (setting != null)
            {
                connString = setting.ConnectionString;
            }

            SqlConnection conn = new SqlConnection(connString);
            Results res = new Results();

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

            var ws_fn_partosap = new ZConfirmPickingGoodsIssue();
            ws_fn_partosap.IDoNumber = DoNumber;
            ws_fn_partosap.IPoNumber = PoNumber;
            ws_fn_partosap.IStgeLoc = SLoc;

            //ส่งไปให้ SAP
         
            ws_res = ws_service.ZConfirmPickingGoodsIssue(ws_fn_partosap);

            string eMaterailDoc = ConvertObjectArrayToString(ws_res.eMaterailDoc);

            //test data to json
            //Console.WriteLine("eMaterailDocresultsap: " + eMaterailDoc);
            //Console.WriteLine("EMessage: " + ws_res.EMessage);

            string dataUpdateList = "UPDATE " + DBconfig + ".[T_barcode_trans] set REFDOCSAP = @REFDOCSAP , CONFIRM_DATE = @CONFIRM_DATE ,CONFIRM_DOC = @CONFIRM_DOC  where ORDERNO = '" + PoAndDo + "' and MENUID = 'DO13'";

            if (ws_res.eMaterailDoc.Count() > 0)
            {
                using (SqlCommand cmd = new SqlCommand(dataUpdateList, conn))
                {
                    if (!string.IsNullOrEmpty(ws_res.eMaterailDoc[0].MatDoc))
                    {
                        cmd.Parameters.AddWithValue("@REFDOCSAP", ws_res.EMessage);
                        cmd.Parameters.AddWithValue("@CONFIRM_DOC", "");
                        //cmd.Parameters.AddWithValue("@CONFIRM_DOC", ws_res.eMaterailDoc[0].DocYear + "|" + ws_res.eMaterailDoc[0].MatDoc);
                        cmd.Parameters.AddWithValue("@CONFIRM_DATE", DateTime.Now);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@REFDOCSAP", ws_res.EMessage);
                        cmd.Parameters.AddWithValue("@CONFIRM_DOC", "");
                        //cmd.Parameters.AddWithValue("@CONFIRM_DOC", ws_res.eMaterailDoc[0].DocYear + "|" + ws_res.eMaterailDoc[0].MatDoc);
                        cmd.Parameters.AddWithValue("@CONFIRM_DATE", DateTime.Now);
                    }
                    conn.Open();
                    int resultseccess = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            else {
                using (SqlCommand cmd = new SqlCommand(dataUpdateList, conn))
                {
                    cmd.Parameters.AddWithValue("@REFDOCSAP", ws_res.EMessage);
                    cmd.Parameters.AddWithValue("@CONFIRM_DOC", "");
                    //cmd.Parameters.AddWithValue("@CONFIRM_DOC", ws_res.eMaterailDoc[0].DocYear + "|" + ws_res.eMaterailDoc[0].MatDoc);
                    cmd.Parameters.AddWithValue("@CONFIRM_DATE", DateTime.Now);
                    
                    conn.Open();
                    int resultseccess = cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }

            string sqlLog_Gi = "INSERT INTO " + DBconfig + ".[T_LOG_GI_STOCK] "
            + "(Batch, EntryQnt, EntryUom, FacNo, Material, StgeLoc, MoveType, Plant, Custid, Kanban ,StockDate , UpdDate ,DocMat ,EMessage,DoNo) " +
            "VALUES "
            + "(@Batch, @EntryQnt, @EntryUom, @FacNo, @Material, @StgeLoc, @MoveType, @Plant, @Custid, @Kanban, @StockDate, @UpdDate, @DocMat , @EMessage ,@DoNo)";


            string sqlErrorLog_Gi = "INSERT INTO " + DBconfig + ".[T_LOG_STOCK_ERROR] "
            + "(RefDocNo ,Batch, EntryQnt, EntryUom, FacNo, Material, StgeLoc, MoveType, Plant, Custid, Kanban ,StockDate , UpdDate  ,EMessage) " +
            "VALUES "
            + "(@RefDocNo ,@Batch, @EntryQnt, @EntryUom, @FacNo, @Material, @StgeLoc, @MoveType, @Plant, @Custid, @Kanban, @StockDate, @UpdDate , @EMessage)";

            string UpdateStatusSap = "UPDATE " + DBconfig + ".[T_LogDatavalidate_GI_to_Sap] SET SapStatus = @SapStatus , ConfirmDate = @ConfirmDate  where ID = '" + getID + "'";

            Console.WriteLine("ws_res.EMessage :" + ws_res.EMessage);
            string[] datalast = null;
            if (ws_res.EMessage.Contains("saved")) {
                string txtClean = ws_res.EMessage.Replace(" ", "");
                int lengthStart = txtClean.IndexOf("Delivery");
                int lengthEnd = txtClean.IndexOf("has");
                int start = lengthStart + 8;
                int end = lengthEnd - start;
                string data = txtClean.Substring(start, end);
                datalast = data.Split(',');
            }
           
            
            Console.WriteLine("datalast :" + datalast);

            int index = 0;
            if (ws_res.eMaterailDoc.Count() > 0)
            {
                foreach (var doc in ws_res.eMaterailDoc)
                {
                    Console.WriteLine("doc :" + doc.DoNo);

                    index++;
                    //if (!string.IsNullOrEmpty(doc.MatDoc))
                    //{
                   
                    //    using (SqlCommand cmd = new SqlCommand(UpdateStatusSap, conn))
                    //    {
                    //        cmd.Parameters.AddWithValue("@SapStatus", 1);
                    //        cmd.Parameters.AddWithValue("@ConfirmDate", DateTime.Now);
                    //        conn.Open();
                    //        int resultsap = cmd.ExecuteNonQuery();
                    //        conn.Close();
                    //    }
                    //    using (SqlCommand cmd = new SqlCommand(sqlLog_Gi, conn))
                    //    {
                    //        cmd.Parameters.AddWithValue("@Batch", "");
                    //        cmd.Parameters.AddWithValue("@EntryQnt", 0);
                    //        cmd.Parameters.AddWithValue("@EntryUom", "");
                    //        cmd.Parameters.AddWithValue("@FacNo", "");
                    //        cmd.Parameters.AddWithValue("@Material", PoAndDo);
                    //        cmd.Parameters.AddWithValue("@StgeLoc", SLoc);
                    //        cmd.Parameters.AddWithValue("@MoveType", "");
                    //        cmd.Parameters.AddWithValue("@Plant", "");

                    //        cmd.Parameters.AddWithValue("@Custid", "");
                    //        cmd.Parameters.AddWithValue("@Kanban", "");
                    //        cmd.Parameters.AddWithValue("@StockDate", Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")));
                    //        cmd.Parameters.AddWithValue("@UpdDate", DateTime.Now);
                    //        cmd.Parameters.AddWithValue("@DocMat", doc.MatDoc + "|IT");
                    //        cmd.Parameters.AddWithValue("@EMessage", "Z_CONFIRM_PICKING_GOODS_ISSUE : " + ws_res.EMessage);
                    //        cmd.Parameters.AddWithValue("@DoNo", doc.DoNo);
                    //        conn.Open();

                    //        int resultseccess = cmd.ExecuteNonQuery();
                    //        conn.Close();
                    //    }
                    //    saved = 1;
                    //}
                    //else// case error
                    //{
                    //    using (SqlCommand cmd = new SqlCommand(sqlErrorLog_Gi, conn))
                    //    {
                    //        cmd.Parameters.AddWithValue("@RefdocNo", RefdocNo);
                    //        cmd.Parameters.AddWithValue("@Batch", "");
                    //        cmd.Parameters.AddWithValue("@EntryQnt", 0);
                    //        cmd.Parameters.AddWithValue("@EntryUom", "");
                    //        cmd.Parameters.AddWithValue("@FacNo", "");
                    //        cmd.Parameters.AddWithValue("@Material", PoAndDo);
                    //        cmd.Parameters.AddWithValue("@StgeLoc", SLoc);
                    //        cmd.Parameters.AddWithValue("@MoveType", "");
                    //        cmd.Parameters.AddWithValue("@Plant", "");

                    //        cmd.Parameters.AddWithValue("@Custid", "");
                    //        cmd.Parameters.AddWithValue("@Kanban", "");
                    //        cmd.Parameters.AddWithValue("@StockDate", Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")));
                    //        cmd.Parameters.AddWithValue("@UpdDate", DateTime.Now);
                    //        cmd.Parameters.AddWithValue("@DocMat", doc.MatDoc);
                    //        cmd.Parameters.AddWithValue("@EMessage", "Z_CONFIRM_PICKING_GOODS_ISSUE : " + ws_res.EMessage);
                    //        conn.Open();
                    //        int resultError = cmd.ExecuteNonQuery();
                    //        conn.Close();
                    //    }
                    //    saved = 1;
                    //}

                  
                    if (ws_res.EMessage.Contains("saved") && index <= datalast.Length)
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
                            cmd.Parameters.AddWithValue("@StgeLoc", SLoc);
                            cmd.Parameters.AddWithValue("@MoveType", "");
                            cmd.Parameters.AddWithValue("@Plant", "");

                            cmd.Parameters.AddWithValue("@Custid", "");
                            cmd.Parameters.AddWithValue("@Kanban", "");
                            cmd.Parameters.AddWithValue("@StockDate", Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")));
                            cmd.Parameters.AddWithValue("@UpdDate", DateTime.Now);
                            cmd.Parameters.AddWithValue("@DocMat", doc.MatDoc + "|IT");
                            cmd.Parameters.AddWithValue("@EMessage", "Z_CONFIRM_PICKING_GOODS_ISSUE : " + ws_res.EMessage);
                            cmd.Parameters.AddWithValue("@DoNo", doc.DoNo);
                            conn.Open();

                            int resultseccess = cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlErrorLog_Gi, conn))
                        {
                            cmd.Parameters.AddWithValue("@RefdocNo", RefdocNo);
                            cmd.Parameters.AddWithValue("@Batch", "");
                            cmd.Parameters.AddWithValue("@EntryQnt", 0);
                            cmd.Parameters.AddWithValue("@EntryUom", "");
                            cmd.Parameters.AddWithValue("@FacNo", "");
                            cmd.Parameters.AddWithValue("@Material", PoAndDo);
                            cmd.Parameters.AddWithValue("@StgeLoc", SLoc);
                            cmd.Parameters.AddWithValue("@MoveType", "");
                            cmd.Parameters.AddWithValue("@Plant", "");

                            cmd.Parameters.AddWithValue("@Custid", "");
                            cmd.Parameters.AddWithValue("@Kanban", "");
                            cmd.Parameters.AddWithValue("@StockDate", Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")));
                            cmd.Parameters.AddWithValue("@UpdDate", DateTime.Now);
                            cmd.Parameters.AddWithValue("@DocMat", doc.MatDoc);
                            cmd.Parameters.AddWithValue("@EMessage", "Z_CONFIRM_PICKING_GOODS_ISSUE : " + ws_res.EMessage);
                            conn.Open();
                            int resultError = cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                    
                }

            }else{

                if (ws_res.EMessage.Contains("saved"))
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
                        cmd.Parameters.AddWithValue("@StgeLoc", SLoc);
                        cmd.Parameters.AddWithValue("@MoveType", "");
                        cmd.Parameters.AddWithValue("@Plant", "");

                        cmd.Parameters.AddWithValue("@Custid", "");
                        cmd.Parameters.AddWithValue("@Kanban", "");
                        cmd.Parameters.AddWithValue("@StockDate", Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")));
                        cmd.Parameters.AddWithValue("@UpdDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@DocMat", "");
                        cmd.Parameters.AddWithValue("@EMessage", "Z_CONFIRM_PICKING_GOODS_ISSUE : " + ws_res.EMessage);
                        cmd.Parameters.AddWithValue("@DoNo", "");
                        conn.Open();

                        int resultseccess = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand(sqlErrorLog_Gi, conn))
                    {
                        cmd.Parameters.AddWithValue("@RefdocNo", RefdocNo);
                        cmd.Parameters.AddWithValue("@Batch", "");
                        cmd.Parameters.AddWithValue("@EntryQnt", 0);
                        cmd.Parameters.AddWithValue("@EntryUom", "");
                        cmd.Parameters.AddWithValue("@FacNo", "");
                        cmd.Parameters.AddWithValue("@Material", PoAndDo);
                        cmd.Parameters.AddWithValue("@StgeLoc", SLoc);
                        cmd.Parameters.AddWithValue("@MoveType", "");
                        cmd.Parameters.AddWithValue("@Plant", "");

                        cmd.Parameters.AddWithValue("@Custid", "");
                        cmd.Parameters.AddWithValue("@Kanban", "");
                        cmd.Parameters.AddWithValue("@StockDate", Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")));
                        cmd.Parameters.AddWithValue("@UpdDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@DocMat", "");
                        cmd.Parameters.AddWithValue("@EMessage", "Z_CONFIRM_PICKING_GOODS_ISSUE : " + ws_res.EMessage);
                        conn.Open();
                        int resultError = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
        }
    }
}

