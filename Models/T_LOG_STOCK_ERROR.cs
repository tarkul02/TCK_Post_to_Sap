using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAP_Batch_GR_TR.Models
{
    public class T_LOG_STOCK_ERROR
    {
        public string MatNo { get; set; }
        public string CustID { get; set; }
        public string FacNo { get; set; }
        public string Plant { get; set; }
        public string SLoc { get; set; }
        public int MvmntType { get; set; }
        public string PostDate { get; set; }
        public string PostTime { get; set; }
        public Nullable<int> QRQty { get; set; }
        public string HeaderText { get; set; }
        public int Action { get; set; }
    }
}
