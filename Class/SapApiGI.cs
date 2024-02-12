namespace SapApiGI.Class
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "Z_CONFIRM_PICKING_GOODS_ISSUE_BND", Namespace = "urn:sap-com:document:sap:soap:functions:mc-style")]
    public partial class Z_CONFIRM_PICKING_GOODS_ISSUE_SRV : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback ZConfirmPickingGoodsIssueOperationCompleted;

        private bool useDefaultCredentialsSetExplicitly;

        //http://tkcprdci:8000/sap/bc/srt/rfc/sap/z_confirm_picking_goods_issue/900/z_confirm_picking_goods_issue_sr/z_confirm_picking_goods_issue_bnd
        /// <remarks/>
        public Z_CONFIRM_PICKING_GOODS_ISSUE_SRV()
        {
            // this.Url = global::WebServices.Properties.Settings.Default.WebServices_PRD_Z_CONFIRM_PICKING_GOODS_ISSUE_Z_CONFIRM_PICKING_GOODS_ISSUE_SRV;
            // this.Url = "http://tkcprdci:8000/sap/bc/srt/rfc/sap/z_confirm_picking_goods_issue/900/z_confirm_picking_goods_issue_sr/z_confirm_picking_goods_issue_bnd"
            this.Url = "http://172.18.1.35:8000/sap/bc/srt/rfc/sap/z_confirm_picking_goods_issue/400/z_confirm_picking_goods_issue_sr/z_confirm_picking_goods_issue_srv";
            if ((this.IsLocalFileSystemWebService(this.Url) == true))
            {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else
            {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        public new string Url
        {
            get
            {
                return base.Url;
            }
            set
            {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true)
                            && (this.useDefaultCredentialsSetExplicitly == false))
                            && (this.IsLocalFileSystemWebService(value) == false)))
                {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }

        public new bool UseDefaultCredentials
        {
            get
            {
                return base.UseDefaultCredentials;
            }
            set
            {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        /// <remarks/>
        public event ZConfirmPickingGoodsIssueCompletedEventHandler ZConfirmPickingGoodsIssueCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:sap-com:document:sap:soap:functions:mc-style:Z_CONFIRM_PICKING_GOODS_ISSUE:ZC" +
            "onfirmPickingGoodsIssueRequest", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("ZConfirmPickingGoodsIssueResponse", Namespace = "urn:sap-com:document:sap:soap:functions:mc-style")]
        public ZConfirmPickingGoodsIssueResponse ZConfirmPickingGoodsIssue([System.Xml.Serialization.XmlElementAttribute("ZConfirmPickingGoodsIssue", Namespace = "urn:sap-com:document:sap:soap:functions:mc-style")] ZConfirmPickingGoodsIssue ZConfirmPickingGoodsIssue1)
        {
            object[] results = this.Invoke("ZConfirmPickingGoodsIssue", new object[] {
                    ZConfirmPickingGoodsIssue1});
            return ((ZConfirmPickingGoodsIssueResponse)(results[0]));
        }

        /// <remarks/>
        public void ZConfirmPickingGoodsIssueAsync(ZConfirmPickingGoodsIssue ZConfirmPickingGoodsIssue1)
        {
            this.ZConfirmPickingGoodsIssueAsync(ZConfirmPickingGoodsIssue1, null);
        }

        /// <remarks/>
        public void ZConfirmPickingGoodsIssueAsync(ZConfirmPickingGoodsIssue ZConfirmPickingGoodsIssue1, object userState)
        {
            if ((this.ZConfirmPickingGoodsIssueOperationCompleted == null))
            {
                this.ZConfirmPickingGoodsIssueOperationCompleted = new System.Threading.SendOrPostCallback(this.OnZConfirmPickingGoodsIssueOperationCompleted);
            }
            this.InvokeAsync("ZConfirmPickingGoodsIssue", new object[] {
                    ZConfirmPickingGoodsIssue1}, this.ZConfirmPickingGoodsIssueOperationCompleted, userState);
        }

        private void OnZConfirmPickingGoodsIssueOperationCompleted(object arg)
        {
            if ((this.ZConfirmPickingGoodsIssueCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ZConfirmPickingGoodsIssueCompleted(this, new ZConfirmPickingGoodsIssueCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        private bool IsLocalFileSystemWebService(string url)
        {
            if (((url == null)
                        || (url == string.Empty)))
            {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024)
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0)))
            {
                return true;
            }
            return false;
        }
    }
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:sap-com:document:sap:soap:functions:mc-style")]
    public partial class ZConfirmPickingGoodsIssue
    {

        private string iDoNumberField;

        private string iPoNumberField;

        private string iStgeLocField;

        private ZsgmDetail1[] itDetailField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IDoNumber
        {
            get
            {
                return this.iDoNumberField;
            }
            set
            {
                this.iDoNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IPoNumber
        {
            get
            {
                return this.iPoNumberField;
            }
            set
            {
                this.iPoNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IStgeLoc
        {
            get
            {
                return this.iStgeLocField;
            }
            set
            {
                this.iStgeLocField = value;
            }
        }
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public ZsgmDetail1[] ItDetail
        {
            get
            {
                return this.itDetailField;
            }
            set
            {
                this.itDetailField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ZConfirmPickingGoodsIssueCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal ZConfirmPickingGoodsIssueCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ZConfirmPickingGoodsIssueResponse Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((ZConfirmPickingGoodsIssueResponse)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:sap-com:document:sap:soap:functions:mc-style")]
    public partial class Bapi2017GmHeadRet
    {

        private string matDocField;

        private string docYearField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MatDoc
        {
            get
            {
                return this.matDocField;
            }
            set
            {
                this.matDocField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DocYear
        {
            get
            {
                return this.docYearField;
            }
            set
            {
                this.docYearField = value;
            }
        }
    }
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void ZConfirmPickingGoodsIssueCompletedEventHandler(object sender, ZConfirmPickingGoodsIssueCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:sap-com:document:sap:soap:functions:mc-style")]
    public partial class ZConfirmPickingGoodsIssueResponse
    {

        private Bapi2017GmHeadRet eMaterailDocField;

        private string eMessageField;


        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Bapi2017GmHeadRet EMaterailDoc
        {
            get
            {
                return this.eMaterailDocField;
            }
            set
            {
                this.eMaterailDocField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EMessage
        {
            get
            {
                return this.eMessageField;
            }
            set
            {
                this.eMessageField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:sap-com:document:sap:soap:functions:mc-style")]
    public partial class ZsgmDetail1
    {

        private string materialField;

        private string plantField;

        private string stgeLocField;

        private string batchField;

        private string moveTypeField;

        private decimal entryQntField;

        private string entryUomField;

        private string itemTextField;

        private string grRcptField;

        private string unloadPtField;

        private string facNoField;

        private string refDocYrField;

        private string refDocField;

        private string refDocItField;

        private string movePlantField;

        private string moveStlocField;

        private string moveBatchField;

        private string soldToField;

        private string custidField;

        private string kanbanField;

        private string amountField;

        private string errorField;

        private string IDoNumberField;

        private string IPoNumberField;

        private string IStgeLocField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Material
        {
            get
            {
                return this.materialField;
            }
            set
            {
                this.materialField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Plant
        {
            get
            {
                return this.plantField;
            }
            set
            {
                this.plantField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string StgeLoc
        {
            get
            {
                return this.stgeLocField;
            }
            set
            {
                this.stgeLocField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Batch
        {
            get
            {
                return this.batchField;
            }
            set
            {
                this.batchField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MoveType
        {
            get
            {
                return this.moveTypeField;
            }
            set
            {
                this.moveTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal EntryQnt
        {
            get
            {
                return this.entryQntField;
            }
            set
            {
                this.entryQntField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EntryUom
        {
            get
            {
                return this.entryUomField;
            }
            set
            {
                this.entryUomField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ItemText
        {
            get
            {
                return this.itemTextField;
            }
            set
            {
                this.itemTextField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string GrRcpt
        {
            get
            {
                return this.grRcptField;
            }
            set
            {
                this.grRcptField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string UnloadPt
        {
            get
            {
                return this.unloadPtField;
            }
            set
            {
                this.unloadPtField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FacNo
        {
            get
            {
                return this.facNoField;
            }
            set
            {
                this.facNoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RefDocYr
        {
            get
            {
                return this.refDocYrField;
            }
            set
            {
                this.refDocYrField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RefDoc
        {
            get
            {
                return this.refDocField;
            }
            set
            {
                this.refDocField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RefDocIt
        {
            get
            {
                return this.refDocItField;
            }
            set
            {
                this.refDocItField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MovePlant
        {
            get
            {
                return this.movePlantField;
            }
            set
            {
                this.movePlantField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MoveStloc
        {
            get
            {
                return this.moveStlocField;
            }
            set
            {
                this.moveStlocField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MoveBatch
        {
            get
            {
                return this.moveBatchField;
            }
            set
            {
                this.moveBatchField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SoldTo
        {
            get
            {
                return this.soldToField;
            }
            set
            {
                this.soldToField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Custid
        {
            get
            {
                return this.custidField;
            }
            set
            {
                this.custidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Kanban
        {
            get
            {
                return this.kanbanField;
            }
            set
            {
                this.kanbanField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Error
        {
            get
            {
                return this.errorField;
            }
            set
            {
                this.errorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IPoNumber
        {
            get
            {
                return this.IPoNumberField;
            }
            set
            {
                this.IPoNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IDoNumber
        {
            get
            {
                return this.IDoNumberField;
            }
            set
            {
                this.IDoNumberField = value;
            }
        }
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IStgeLoc
        {
            get
            {
                return this.IStgeLocField;
            }
            set
            {
                this.IStgeLocField = value;
            }
        }
    }
}
