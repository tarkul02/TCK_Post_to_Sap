using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAP_Batch_GR_TR.Models
{
    public partial class Results
    {

        private bool statusField;

        private string messageField;

        private string message2Field;

        private string message3Field;

        /// <remarks/>
        public bool status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public string message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
            }
        }

        /// <remarks/>
        public string message2
        {
            get
            {
                return this.message2Field;
            }
            set
            {
                this.message2Field = value;
            }
        }

        /// <remarks/>
        public string message3
        {
            get
            {
                return this.message3Field;
            }
            set
            {
                this.message3Field = value;
            }
        }
    }
}
