using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CafeProject
{
    public class InvoiceItem
    {
        public InvoiceItem(int itemID, int invoiceID, string itemName, string itemDescription, decimal itemPrice, int itemQuantity)
        {
            this.itemID = itemID;
            this.invoiceID = invoiceID;
            this.itemName = itemName;
            this.itemDescription = itemDescription;
            this.itemPrice = itemPrice;
            this.itemQuantity = itemQuantity;
            // this.itemTotalPrice = itemTotalPrice;

        }

        public int itemID { get; set; }

        public int invoiceID { get; set; }


        public string itemName { get; set; }

        public string itemDescription { get; set; }

        public decimal itemPrice { get; set; }


        public int itemQuantity { get; set; }


        public override string ToString() => $"{itemID,5}  {itemName,-25} {itemDescription,-15} {itemPrice,-20} {itemQuantity,-20}";
    }

}
