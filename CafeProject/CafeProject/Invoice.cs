using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CafeProject
{
    public class Invoice
    {
        public Invoice(int invoiceID, DateTime invoiceDate, bool shipped, string customerName, string customerAddress, string customerEmail)
        {
            this.invoiceID = invoiceID;
            this.invoiceDate = invoiceDate;
            this.shipped = shipped;
            this.customerName = customerName;
            this.customerAddress = customerAddress;
            this.customerEmail = customerEmail;

        }

        public int invoiceID { get; set; }

        public DateTime invoiceDate { get; set; }

        public bool shipped { get; set; }

        public string customerName { get; set; }

        public string customerAddress { get; set; }

        public string customerEmail { get; set; }

        public override string ToString() => $"{invoiceID,5} {customerName,-25} {customerEmail,-15} {shipped,-20}";

    }
}