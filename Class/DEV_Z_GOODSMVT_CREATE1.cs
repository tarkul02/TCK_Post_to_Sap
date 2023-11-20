using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV_Z_GOODSMVT_CREATE1.Class
{
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;


    /// <remarks/>
    // CODEGEN: The optional WSDL extension element 'Policy' from namespace 'http://schemas.xmlsoap.org/ws/2004/09/policy' was not handled.
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "Z_GOODSMVT_CREATE1_BND", Namespace = "urn:sap-com:document:sap:soap:functions:mc-style")]
    public partial class Z_GOODSMVT_CREATE1_SRV : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback ZGoodsmvtCreate1OperationCompleted;

        private bool useDefaultCredentialsSetExplicitly;

        /// <remarks/>
        /// 
        //[global::System.Configuration.ApplicationScopedSettingAttribute()]
        //[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        //[global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        //[global::System.Configuration.DefaultSettingValueAttribute("http://172.18.1.35:8000/sap/bc/srt/rfc/sap/z_goodsmvt_create1/400/z_goodsmvt_create1_srv/z_goodsmvt_create1_bnd")]
        //public string WebServices_DEV_Z_GOODSMVT_CREATE1_Z_GOODSMVT_CREATE1_SRV
        //{
        //    get
        //    {
        //        Console.WriteLine("DefaultSettingValueAttribute");
        //        Console.WriteLine((string)("WebServices_DEV_Z_GOODSMVT_CREATE1_Z_GOODSMVT_CREATE1_SRV"));
        //        return ((string)("WebServices_DEV_Z_GOODSMVT_CREATE1_Z_GOODSMVT_CREATE1_SRV"));
        //    }
        //}

        public Z_GOODSMVT_CREATE1_SRV()
        {

            this.Url = "http://172.18.1.35:8000/sap/bc/srt/rfc/sap/z_goodsmvt_create1/400/z_goodsmvt_create1_srv/z_goodsmvt_create1_bnd";
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
        public event ZGoodsmvtCreate1CompletedEventHandler ZGoodsmvtCreate1Completed;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:sap-com:document:sap:soap:functions:mc-style:Z_GOODSMVT_CREATE1:ZGoodsmvtCrea" +
            "te1Request", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("ZGoodsmvtCreate1Response", Namespace = "urn:sap-com:document:sap:soap:functions:mc-style")]
        //public ZGoodsmvtCreate1Response ZGoodsmvtCreate1([System.Xml.Serialization.XmlElementAttribute("ZGoodsmvtCreate1", Namespace = "urn:sap-com:document:sap:soap:functions:mc-style")] ZGoodsmvtCreate1 ZGoodsmvtCreate11)
        //{
        public void ZGoodsmvtCreate1([System.Xml.Serialization.XmlElementAttribute("ZGoodsmvtCreate1", Namespace = "urn:sap-com:document:sap:soap:functions:mc-style")] ZGoodsmvtCreate1 ZGoodsmvtCreate11)
        {
            Console.WriteLine("ZGoodsmvtCreate1 start");
            object[] results = this.Invoke("ZGoodsmvtCreate1", new object[] {
                        ZGoodsmvtCreate11});
            Console.WriteLine("ws_res results: " + results);
            // return ((ZGoodsmvtCreate1Response)(results[0]));
        }

        /// <remarks/>
        public void ZGoodsmvtCreate1Async(ZGoodsmvtCreate1 ZGoodsmvtCreate11)
        {
            Console.WriteLine("start ZGoodsmvtCreate1Async");
            this.ZGoodsmvtCreate1Async(ZGoodsmvtCreate11, null);
            Console.WriteLine("end ZGoodsmvtCreate1Async");
        }

        /// <remarks/>
        public void ZGoodsmvtCreate1Async(ZGoodsmvtCreate1 ZGoodsmvtCreate11, object userState)
        {
            Console.WriteLine("start ZGoodsmvtCreate1Async");
            if ((this.ZGoodsmvtCreate1OperationCompleted == null))
            {
                Console.WriteLine("start if ZGoodsmvtCreate1Async");
                this.ZGoodsmvtCreate1OperationCompleted = new System.Threading.SendOrPostCallback(this.OnZGoodsmvtCreate1OperationCompleted);
            }
            Console.WriteLine("end ZGoodsmvtCreate1Async");
            this.InvokeAsync("ZGoodsmvtCreate1", new object[] {
                        ZGoodsmvtCreate11}, this.ZGoodsmvtCreate1OperationCompleted, userState);
            Console.WriteLine("start InvokeAsync");
        }

        private void OnZGoodsmvtCreate1OperationCompleted(object arg)
        {
            Console.WriteLine("start OnZGoodsmvtCreate1OperationCompleted");
            if ((this.ZGoodsmvtCreate1Completed != null))
            {
                Console.WriteLine("start if OnZGoodsmvtCreate1OperationCompleted");
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ZGoodsmvtCreate1Completed(this, new ZGoodsmvtCreate1CompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
            Console.WriteLine("end if OnZGoodsmvtCreate1OperationCompleted");
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        private bool IsLocalFileSystemWebService(string url)
        {
            Console.WriteLine("start wsUri IsLocalFileSystemWebService");

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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:sap-com:document:sap:soap:functions:mc-style")]
    public partial class ZGoodsmvtCreate1
    {

        private Bapi2017GmCode iGoodsmvtCodeField;

        private string iTestrunField;

        private ZsgmHeader isHeaderField;

        private ZsgmDetail1[] itDetailField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Bapi2017GmCode IGoodsmvtCode
        {
            get
            {
                return this.iGoodsmvtCodeField;
            }
            set
            {
                this.iGoodsmvtCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ITestrun
        {
            get
            {
                return this.iTestrunField;
            }
            set
            {
                this.iTestrunField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ZsgmHeader IsHeader
        {
            get
            {
                return this.isHeaderField;
            }
            set
            {
                this.isHeaderField = value;
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:sap-com:document:sap:soap:functions:mc-style")]
    public partial class Bapi2017GmCode
    {

        private string gmCodeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string GmCode
        {
            get
            {
                return this.gmCodeField;
            }
            set
            {
                this.gmCodeField = value;
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
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:sap-com:document:sap:soap:functions:mc-style")]
    public partial class ZsgmHeader
    {

        private string pstngDateField;

        private string docDateField;

        private string refDocNoField;

        private string headerTxtField;

        private string billOfLadingField;

        private string grGiSlipNoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PstngDate
        {
            get
            {
                return this.pstngDateField;
            }
            set
            {
                this.pstngDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DocDate
        {
            get
            {
                return this.docDateField;
            }
            set
            {
                this.docDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RefDocNo
        {
            get
            {
                return this.refDocNoField;
            }
            set
            {
                this.refDocNoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string HeaderTxt
        {
            get
            {
                return this.headerTxtField;
            }
            set
            {
                this.headerTxtField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string BillOfLading
        {
            get
            {
                return this.billOfLadingField;
            }
            set
            {
                this.billOfLadingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string GrGiSlipNo
        {
            get
            {
                return this.grGiSlipNoField;
            }
            set
            {
                this.grGiSlipNoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:sap-com:document:sap:soap:functions:mc-style")]
    public partial class ZGoodsmvtCreate1Response
    {

        private Bapi2017GmHeadRet eMaterailDocField;

        private string eMessageField;

        private ZsgmDetail1[] itDetailField;

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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void ZGoodsmvtCreate1CompletedEventHandler(object sender, ZGoodsmvtCreate1CompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ZGoodsmvtCreate1CompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal ZGoodsmvtCreate1CompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ZGoodsmvtCreate1Response Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((ZGoodsmvtCreate1Response)(this.results[0]));
            }
        }
    }
}
