using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostSap_GR_TR.Class
{
    public class T_LOG_ADD_STOCK
    {
        public string Batch { get; internal set; }
        public int EntryQnt { get; internal set; }
        public string EntryUom { get; internal set; }
        public object FacNo { get; internal set; }
        public string Material { get; internal set; }
        public string StgeLoc { get; internal set; }
        public string MoveType { get; internal set; }
        public object Plant { get; internal set; }
        public string Custid { get; internal set; }
        public string Kanban { get; internal set; }
        public DateTime UpdDate { get; internal set; }
        public DateTime StockDate { get; internal set; }
        public string DocMat { get; internal set; }
    }
}
